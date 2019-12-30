using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AppRegaliApi.Models;
using Microsoft.AspNet.Identity;

namespace AppRegaliApi.Controllers
{

    [Authorize]
    [RoutePrefix("api/Evento")]
    public class EventoController : ApiController
    {
        private DbDataContext dbDataContext = new DbDataContext();

        #region "Categorie"

        [HttpGet]
        [Route("Categorie")]
        public async Task<List<EventoCategoria>> GetLookupEventoCategoria()
        {
            List<EventoCategoria> categorie = await dbDataContext.EventoCategoria.ToListAsync();
            return categorie;
        }

        #endregion

        #region "Eventi"

        //GET: api/Evento/EventoById/?id=1
        //dato un id, restituisce l'evento. l'oggetto restituito è piatto: nella risposta non sono compresi gli oggetti figli
        [HttpGet]
        [Route("EventoById")]
        [ResponseType(typeof(EventoDtoOutput))]
        public async Task<IHttpActionResult> GetEventoByIdAsync(Guid Id)
        {
            Evento evento = null;

            //Controllo se Id è valorizzato.
            if (Id != Guid.Empty)
            {
                evento = await dbDataContext.Evento
                    .Include(x => x.Regalo)
                    .Include(x => x.Regalo.Select(y => y.ImmagineRegalo))
                    .Include(x => x.ImmagineEvento)
                    .Include(x => x.EventoCategoria)
                    .SingleOrDefaultAsync(x => x.Id == Id);
            }

            return Ok(EventoMapper.EventoToEventoDto(evento));
        }

        [HttpGet]
        [Route("EventiCurrentUser")]
        [ResponseType(typeof(List<EventoDtoOutput>))]
        public async Task<IHttpActionResult> GetEventoCurrentUser()
        {
            Guid currentUser = new Guid(User.Identity.GetUserId());
            List<Evento> eventi = await dbDataContext.Evento
                           .Include(x => x.ImmagineEvento)
                           .Where(x => x.IdUtenteCreazione == currentUser)
                           .OrderBy(x => x.DataEvento)
                           .ToListAsync();

            return Ok(EventoMapper.EventoToEventoDtoList(eventi));
        }

        // GET: api/Evento/EventiByIdUtenteIdCategoria?idUtente=1&idCategoria=2
        //dato un id utente e una categoria, restituisce tutti gli eventi di quell'utente.
        //l'oggetto restituito è piatto: nella risposta non sono compresi gli oggetti figli
        [HttpGet]
        [Route("EventiAmiciFiltered")]
        public async Task<List<EventoDtoOutput>> GetEventiByidUtente(int pageNumber, int pageSize, string IdUtente = null, string IdCategoria = null)
        {
            //Ottengo la lista degli amici
            List<Guid> idAmici = await AmiciUtility.GetIdAmiciOfUser(new Guid(User.Identity.GetUserId()));

            //Ottengo gli eventi
            //List<Evento> eventi = await dbDataContext.Evento
            //               .Include(x => x.ImmagineEvento)
            //               .Where(x => (idAmici.Contains(x.IdUtenteCreazione)
            //                         & (IdUtente == null || x.IdUtenteCreazione.ToString() == IdUtente))
            //                         & (IdCategoria == null || x.IdCategoriaEvento.ToString() == IdCategoria))
            //               .OrderBy(x => x.DataEvento)
            //               //.Skip(pageSize * (pageNumber - 1))
            //               //.Take(pageSize)
            //               .ToListAsync();

            //return EventoMapper.EventoToEventoDtoList(eventi);

            //Ottengo gli eventi
            //List<Evento> eventi
            List<EventoDtoOutput> eventiDto = await dbDataContext.Evento
                           
                           .Join(dbDataContext.UserInfo, // the source table of the inner join
                                     evento => evento.IdUtenteCreazione,        // Select the primary key (the first part of the "on" clause in an sql "join" statement)
                                     userInfo => userInfo.IdAspNetUser,   // Select the foreign key (the second part of the "on" clause)
                                     (evento, userInfo) => new { Evento = evento, UserInfo = userInfo }) // selection
                                  .Where(eventoAndUserInfo => (idAmici.Contains(eventoAndUserInfo.Evento.IdUtenteCreazione)
                                                            & (IdUtente == null || eventoAndUserInfo.Evento.IdUtenteCreazione.ToString() == IdUtente)
                                                            & (IdCategoria == null || eventoAndUserInfo.Evento.IdCategoriaEvento.ToString() == IdCategoria)
                                                            & (eventoAndUserInfo.Evento.DataEvento >= DateTime.Today) ) //PER ORA SI VISUALIZZANO SOLO EVENTI FUTURI
                                   )   // where statement
                            .Include(eventoAndUserInfo => eventoAndUserInfo.Evento.ImmagineEvento)
                           //.OrderBy(eventoAndUserInfo => eventoAndUserInfo.Evento.DataEvento)
                           .OrderByDescending(eventoAndUserInfo => eventoAndUserInfo.Evento.DataCreazione)
                            .Skip(pageSize * (pageNumber - 1))
                            .Take(pageSize)
                            .Select(join => new EventoDtoOutput()
                            {
                                Id = join.Evento.Id.ToString(),
                                Cancellato = join.Evento.Cancellato,
                                DataEvento = join.Evento.DataEvento,
                                Descrizione = join.Evento.Descrizione,
                                IdCategoriaEvento = join.Evento.IdCategoriaEvento,
                                DataCreazione = join.Evento.DataCreazione,
                                DataModifica = join.Evento.DataModifica,
                                IdImmagineEvento = join.Evento.IdImmagineEvento.ToString(),
                                Titolo = join.Evento.Titolo,
                                ImmagineEvento = join.Evento.ImmagineEvento.Immagine,
                                NomeUserCreatoreEvento = join.UserInfo.Nome,
                                CognomeUserCreatoreEvento = join.UserInfo.Cognome,
                                ImmagineUserCreatoreEvento = join.UserInfo.FotoProfilo
                            })
                           .ToListAsync();

            //return EventoMapper.EventoToEventoDtoList(eventi);
            return eventiDto;
        }

        // PUT: api/Evento/EventoUpdate/1
        [HttpPut]
        [Route("EventoUpdate/{IdEvento:Guid}")]
        [ResponseType(typeof(EventoDtoOutput))]
        public async Task<IHttpActionResult> UpdateEvento([FromUri]Guid IdEvento, [FromBody]EventoDtoInput Evento)
        {
            //Controllo che i parametri siano valorizzati
            if (Evento == null || !ModelState.IsValid || (IdEvento == null || IdEvento == Guid.Empty))
            {
                return BadRequest(ModelState);
            }

            //Cerco l'evento
            Evento evento = await dbDataContext.Evento.Include(x => x.ImmagineEvento).Where(x => x.Id == IdEvento).FirstAsync();
            if (evento == null)
            {
                return NotFound();
            }

            //Modifico l'evento
            evento.Titolo = Evento.Titolo;
            evento.Descrizione = Evento.Descrizione;
            evento.DataModifica = DateTime.Now;
            evento.DataEvento = Evento.DataEvento;
            evento.Cancellato = Evento.Cancellato;
            //L'immagine esiste sempre
            evento.ImmagineEvento.Immagine = Evento.ImmagineEvento;

            try
            {
                //Salvo le modifiche sul DB.
                await dbDataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventoExists(evento.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return StatusCode(HttpStatusCode.NoContent);
            return Ok(EventoMapper.EventoToEventoDto(evento));
                //CreatedAtRoute("UpdateEvento", new { id = evento.Id }, evento);
        }

        // POST: api/Evento/EventoCreate
        [HttpPost]
        [Route("EventoCreate", Name = "EventoCreate")]
        [ResponseType(typeof(EventoDtoOutput))]
        public async Task<IHttpActionResult> InserisciEvento(EventoDtoInput Evento)
        {
            //Controllo se il modello è valido
            if (Evento == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Creo l'immagine
            ImmagineEvento immagineEvento = new ImmagineEvento()
            {
                Id = new Guid(),
                Immagine = Evento.ImmagineEvento
            };

            //Salvo l'immagine sul DB
            dbDataContext.ImmagineEvento.Add(immagineEvento);

            //Creo l'Evento
            Evento evento = new Evento()
            {
                Id = new Guid(),
                Titolo = Evento.Titolo,
                Descrizione = Evento.Descrizione,
                IdUtenteCreazione = new Guid(User.Identity.GetUserId()),
                DataCreazione = DateTime.Now,
                DataModifica = DateTime.Now,
                DataEvento = Evento.DataEvento,
                Cancellato = false,
                IdImmagineEvento = immagineEvento.Id,
                IdCategoriaEvento = Evento.IdCategoriaEvento
            };

            //Salvo l'evento sul DB
            dbDataContext.Evento.Add(evento);

            try
            {
                await dbDataContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EventoExists(evento.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(EventoMapper.EventoToEventoDto(evento));
            //In questo modo mi da un 201 da FE, e si blocca l'esecuizione: come si può risolvere?
            //CreatedAtRoute("EventoCreate", new { id = evento.Id }, evento);
        }

        // DELETE: api/Evento/EventoDelete/5
        [HttpDelete]
        [Route("EventoDelete/{id}")]
        [ResponseType(typeof(EventoDtoOutput))]
        public async Task<IHttpActionResult> DeleteEvento(Guid Id)
        {
            Evento evento = await dbDataContext.Evento
                                                .Include(x => x.Regalo)
                                                .Include(x => x.Regalo.Select(y => y.ImmagineRegalo))
                                                .Include(x => x.ImmagineEvento)
                                                .FirstOrDefaultAsync(x => x.Id == Id);
            if (evento == null)
            {
                return NotFound();
            }

            if (evento.ImmagineEvento != null)
            {
                dbDataContext.ImmagineEvento.Remove(evento.ImmagineEvento);
            }

            if (evento.Regalo != null)
            {
                List<Guid> guidRegali = new List<Guid>();
                foreach (Regalo reg in evento.Regalo)
                {
                    //guidRegali.Add(reg.Id);
                    if (reg.ImmagineRegalo != null)
                    {
                        dbDataContext.ImmagineRegalo.Remove(reg.ImmagineRegalo);
                    }
                }

                dbDataContext.Regalo.RemoveRange(evento.Regalo);
            }

            dbDataContext.Evento.Remove(evento);
            dbDataContext.SaveChanges();

            return Ok();
        }

        #endregion

        #region "Regali"

        private async Task<ImmagineRegalo> createOrUpdateImmagineRegalo(byte[] Immagine, string IdImmagineRegalo)
        {

            ImmagineRegalo immRegalo;
            if (IdImmagineRegalo != null)
            {
                immRegalo = await dbDataContext.ImmagineRegalo.FindAsync(new Guid(IdImmagineRegalo));
                dbDataContext.ImmagineRegalo.Attach(immRegalo);
                dbDataContext.Entry(immRegalo).State = EntityState.Modified;
            }
            else
            {
                immRegalo = new ImmagineRegalo();
                dbDataContext.ImmagineRegalo.Attach(immRegalo);
                dbDataContext.Entry(immRegalo).State = EntityState.Added;
            }
            immRegalo.Immagine = Immagine;
            return immRegalo;
        }

        // GET: api/Evento/RegaloById/?id=5
        //dato un id, restituisce il regalo. l'oggetto restituito è piatto: nella risposta non sono compresi gli oggetti figli
        [HttpGet]
        [Route("RegaloById")]
        [ResponseType(typeof(RegaloDtoOutput))]
        public async Task<IHttpActionResult> GetRegaloById(Guid Id)
        {
            Regalo regalo = null;

            //Controllo se Id è valorizzato.
            if (Id != Guid.Empty)
            {
                regalo = await dbDataContext.Regalo.Include(x => x.ImmagineRegalo).SingleOrDefaultAsync(x => x.Id == Id);
            }
            return Ok(RegaloMapper.RegaloToRegaloDto(regalo));
        }

        // GET: api/Evento/RegaliByIdEvento/?IdEvento=5
        //dato un id categoria, restituisce tutti gli eventi di quella categoria.
        //l'oggetto restituito è piatto: nella risposta non sono compresi gli oggetti figli
        [HttpGet]
        [Route("RegaliByIdEvento")]
        public async Task<List<RegaloDtoOutput>> GetRegaliByIdEvento(Guid IdEvento)
        {
            List<Regalo> regali = await dbDataContext.Regalo.Where(x => x.IdEvento == IdEvento).ToListAsync();
            return RegaloMapper.RegaloToRegaloDtoList(regali);
        }

        // PUT: api/Evento/RegaloUpdate
        //FIXME verificare come si comporta con i figli
        [HttpPut]
        [Route("RegaloUpdate/{IdRegalo:Guid}")]
        [ResponseType(typeof(RegaloDtoOutput))]
        public async Task<IHttpActionResult> UpdateRegalo([FromUri]Guid IdRegalo, [FromBody]RegaloDtoInput RegaloDto)
        {
            //Controllo che i parametri siano valorizzati
            if (RegaloDto == null || !ModelState.IsValid || (IdRegalo == null || IdRegalo == Guid.Empty))
            {
                return BadRequest(ModelState);
            }

            //Cerco il regalo
            Regalo regalo = await dbDataContext.Regalo.Include(x => x.ImmagineRegalo).Where(x => x.Id == IdRegalo).FirstAsync();

            //Modifico il regalo

            regalo.Cancellato = RegaloDto.Cancellato;
            regalo.Descrizione = RegaloDto.Descrizione;
            regalo.ImportoCollezionato = RegaloDto.ImportoCollezionato;
            regalo.Prezzo = RegaloDto.Prezzo;
            regalo.Titolo = RegaloDto.Titolo;


            if (RegaloDto.ImmagineRegalo != null)
            {
                ImmagineRegalo immagineRegalo = new ImmagineRegalo()
                {
                    Id = (regalo.IdImmagineRegalo.HasValue ? regalo.IdImmagineRegalo.Value : new Guid()),
                    Immagine = RegaloDto.ImmagineRegalo
                };

                regalo.ImmagineRegalo = immagineRegalo;
            }


            //regalo.ImmagineRegalo.Immagine = RegaloDto.ImmagineRegalo;
            //await createOrUpdateImmagineRegalo(RegaloDto.ImmagineRegalo, regalo.IdImmagineRegalo.ToString());

            //Regalo regalo = RegaloMapper.RegaloDtoToRegalo(dto);
            ////FIXME
            //regalo.ImmagineRegalo = null;
            //regalo.Evento = null;
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //if (regalo.Id != null)
            //{
            //    ImmagineRegalo immRegalo = await createOrUpdateImmagineRegalo(dto.ImmagineRegalo, dto.IdImmagineRegalo);
            //    regalo.ImmagineRegalo = immRegalo;
            //    dbDataContext.Regalo.Attach(regalo);
            //    dbDataContext.Entry(regalo).State = EntityState.Modified;
            //}

            try
            {
                //Salvo le modifiche sul DB.
                dbDataContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventoExists(regalo.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(RegaloMapper.RegaloToRegaloDto(regalo));
        }

        // POST: api/Evento/RegaloCreate
        [HttpPost]
        [Route("RegaloCreate", Name = "RegaloCreate")]
        [ResponseType(typeof(RegaloDtoOutput))]
        public async Task<IHttpActionResult> InserisciRegalo(RegaloDtoInput Dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            //Regalo regalo = new Regalo();
            //regalo = RegaloMapper.RegaloDtoInputToRegalo(Dto, regalo);
            Regalo regalo = new Regalo()
            {
                Id = new Guid(),
                Cancellato = Dto.Cancellato,
                Descrizione = Dto.Descrizione,
                IdEvento = Dto.IdEvento,
                Prezzo = Dto.Prezzo,
                Titolo = Dto.Titolo,
                ImportoCollezionato = 0
            };


            if (Dto.ImmagineRegalo != null)
            {
                regalo.ImmagineRegalo = await createOrUpdateImmagineRegalo(Dto.ImmagineRegalo, null);
            }



            dbDataContext.Regalo.Add(regalo);

            try
            {
                dbDataContext.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RegaloExists(regalo.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(RegaloMapper.RegaloToRegaloDto(regalo));
        }

        // DELETE: api/Evento/RegaloDelete/5
        [HttpDelete]
        [Route("RegaloDelete/{id}")]
        [ResponseType(typeof(RegaloDtoOutput))]
        public async Task<IHttpActionResult> DeleteRegalo(Guid id)
        {
            Regalo regalo = await dbDataContext.Regalo.Include(x => x.ImmagineRegalo).SingleOrDefaultAsync(x => x.Id == id);
            if (regalo == null)
            {
                return NotFound();
            }

            if (regalo.ImmagineRegalo != null)
            {
                dbDataContext.ImmagineRegalo.Remove(regalo.ImmagineRegalo);
            }
            dbDataContext.Regalo.Remove(regalo);
            dbDataContext.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("PartecipazioneRegaloCreate", Name = "PartecipazioneRegaloCreate")]
        [ResponseType(typeof(RegaloUserPartecipazione))]
        public async Task<IHttpActionResult> InserisciPartecipazioneRegalo(Guid IdRegalo, double Importo, bool Anonimo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Regalo regalo = await dbDataContext.Regalo.FirstOrDefaultAsync(x => x.Id == IdRegalo);
            if (regalo == null)
            {
                return NotFound();
            }

            if (regalo.Prezzo <= regalo.ImportoCollezionato)
            {
                throw new Exception("PRICE_REACHED"); //gestire eccezione: non si può più partecipare perchè il regalo è già al completo
            }

            RegaloUserPartecipazione partecipazione = new RegaloUserPartecipazione()
            {
                Id = Guid.NewGuid(),
                IdRegalo = IdRegalo,
                Importo = Importo,
                Anonimo = Anonimo,
                IdUser = new Guid(User.Identity.GetUserId())
            };

            regalo.ImportoCollezionato = regalo.ImportoCollezionato + Importo;


            dbDataContext.RegaloUserPartecipazione.Add(partecipazione);


            try
            {
                await dbDataContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
            return Ok(partecipazione);
        }

        [HttpGet]
        [Route("PartecipazioniRegalo")]
        public async Task<PartecipazioneDtoOutput> GetPartecipazioniRegalo(Guid IdRegalo)
        {
            DateTime dataEvento = await dbDataContext.Regalo.Include(x => x.Evento)
                                            .Where(x => x.Id == IdRegalo)
                                            .Select(x => x.Evento.DataEvento).FirstOrDefaultAsync();

            List<UtentePartecipazioneDtoOutput> partecipanti = await dbDataContext.RegaloUserPartecipazione
                                    .Join(dbDataContext.UserInfo, // the source table of the inner join
                                     partecipazione => partecipazione.IdUser,        // Select the primary key (the first part of the "on" clause in an sql "join" statement)
                                     userInfo => userInfo.IdAspNetUser,   // Select the foreign key (the second part of the "on" clause)
                                     (partecipazione, userInfo) => new { RegaloUserPartecipazione = partecipazione, UserInfo = userInfo }) // selection
                                  .Where(partecipazioneAndUserInfo =>
                                                            (partecipazioneAndUserInfo.RegaloUserPartecipazione.IdRegalo == IdRegalo)
                                   )
                                                .Select(partecipazioneAndUserInfo => new UtentePartecipazioneDtoOutput()
                                                {
                                                    IdRegalo = partecipazioneAndUserInfo.RegaloUserPartecipazione.IdRegalo,
                                                    IdUserPartecipante = partecipazioneAndUserInfo.RegaloUserPartecipazione.IdUser,
                                                    CognomePartecipante = partecipazioneAndUserInfo.UserInfo.Cognome,
                                                    NomePartecipante = partecipazioneAndUserInfo.UserInfo.Nome,
                                                    Anonimo = partecipazioneAndUserInfo.RegaloUserPartecipazione.Anonimo
                                                }).Distinct()
                                                .ToListAsync();


            int anonimi = 0;
            if (dataEvento.Date > DateTime.Today) //Evento futuro
            {
                anonimi = partecipanti.Where(x => x.Anonimo).Count();
                partecipanti.RemoveAll(x => x.Anonimo);

            }

            PartecipazioneDtoOutput res = new PartecipazioneDtoOutput();
            res.NumeroAnonimi = anonimi;
            res.UtentiPartecipanti = partecipanti;

             return res;
        }


        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbDataContext.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EventoExists(Guid id)
        {
            return dbDataContext.Evento.Count(e => e.Id == id) > 0;
        }

        private bool RegaloExists(Guid id)
        {
            return dbDataContext.Regalo.Count(e => e.Id == id) > 0;
        }
    }
}
