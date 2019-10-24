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
        private EventoMapper eventoMapper = new EventoMapper();
        private RegaloMapper regaloMapper = new RegaloMapper();

        [HttpGet]
        [Route("LookupCategorie")]
        public async Task<List<EventoCategoria>> GetLookupEventoCategoria()
        {
            List<EventoCategoria> categorie = await dbDataContext.EventoCategoria.ToListAsync();
            return categorie;
        }

        // GET: api/Evento/Eventi
        //restituisce una lista piatta di eventi: nella risposta non sono compresi gli oggetti figli
        [HttpGet]
        [Route("Eventi")]
        public async Task<List<EventoDto>> GetEventi()
        {
            List<Evento> eventi = await dbDataContext.Evento.ToListAsync();
            return eventoMapper.EventoToEventoDtoList(eventi);
        }

        // GET: api/Evento/EventoById/5
        //dato un id, restituisce l'evento. l'oggetto restituito è piatto: nella risposta non sono compresi gli oggetti figli
        [HttpGet]
        [Route("EventoById/{id}")]
        [ResponseType(typeof(EventoDto))]
        public async Task<IHttpActionResult> GetEventoByIdAsync(Guid id)
        {
            Evento evento = await dbDataContext.Evento.Include(x => x.Regalo).Include(x => x.ImmagineEvento).SingleOrDefaultAsync(x => x.Id == id);

            return Ok(eventoMapper.EventoToEventoDto(evento));
        }

        // GET: api/Evento/EventiByIdUtenteIdCategoria?idUtente=1&idCategoria=2
        //dato un id utente e una categoria, restituisce tutti gli eventi di quell'utente.
        //l'oggetto restituito è piatto: nella risposta non sono compresi gli oggetti figli

            //fixme solo amici
        [HttpGet]
        [Route("EventiByIdUtenteIdCategoria/{idUtente?}/{idCategoria?}")]
        public async Task<List<EventoDto>> GetEventiByidUtente([FromUri]String idUtente=null, [FromUri]String idCategoria =null)
        {
            List<Guid> idAmici = await AmiciUtility.GetIdAmiciOfUser(new Guid(User.Identity.GetUserId()));

            IQueryable<Evento> query = dbDataContext.Evento.Include(x => x.ImmagineEvento);
            query = query.Where(x => idAmici.Contains(x.IdUtenteCreazione));
            if (idUtente != null)
            {
                Guid guidUtente = new Guid(idUtente);
                Expression<Func<Evento, bool>> idUtenteMatch = c => c.IdUtenteCreazione == guidUtente;
                query = query.Where(idUtenteMatch);
            }
            if (idCategoria != null)
            {
                Guid guidCategoria = new Guid(idCategoria);
                Expression<Func<Evento, bool>> idCategoriaMatch = c => c.IdCategoriaEvento == guidCategoria;
                //metto la condizione sulla categoria in AND
                query = query.Where(idCategoriaMatch);
            }
            query = query.Where(x => x.DataEvento >= DateTime.Now).OrderBy(x => x.DataEvento);

            List<Evento> eventi = await query.ToListAsync();
            return eventoMapper.EventoToEventoDtoList(eventi);
        }

        // PUT: api/Evento/EventoUpdate
        //FIXME verificare come si comporta se un evento ha già dei regalil
        [HttpPut]
        [Route("EventoUpdate")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateEvento(EventoDto dto)
        {
            Evento evento = eventoMapper.EventoDtoToEvento(dto, new Guid(dto.Id));
            evento.EventoCategoria = null;
            //dbDataContext.Entry(evento.EventoCategoria).State = EntityState.Unchanged;
            evento.ImmagineEvento = null;
            //dbDataContext.Entry(evento.ImmagineEvento).State = EntityState.Unchanged;
            evento.Regalo = null;
            //dbDataContext.Entry(evento.Regalo).State = EntityState.Unchanged;//FIXME FARE FOREACH e  verificare che non li cancelli
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (evento.Id != null) {
                dbDataContext.Evento.Attach(evento);
                dbDataContext.Entry(evento).State = EntityState.Modified;
            }

            try
            {
                dbDataContext.SaveChanges();
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

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Evento/EventoCreate
        [HttpPost]
        [Route("EventoCreate", Name = "EventoCreate")]
        [ResponseType(typeof(Evento))]
        public IHttpActionResult InserisciEvento(EventoDto eventoDto)
        {
            Evento evento = eventoMapper.EventoDtoToEvento(eventoDto, new Guid(User.Identity.GetUserId()));
            evento.DataCreazione = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            dbDataContext.ImmagineEvento.Attach(evento.ImmagineEvento);
            dbDataContext.Entry(evento.ImmagineEvento).State = EntityState.Added;
            dbDataContext.Evento.Add(evento);

            try
            {
                dbDataContext.SaveChanges();
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

            return CreatedAtRoute("EventoCreate", new { id = evento.Id }, evento);
        }

        // DELETE: api/Evento/EventoDelete/5
        [HttpDelete]
        [Route("EventoDelete/{id}")]
        [ResponseType(typeof(EventoDto))]
        public IHttpActionResult DeleteEvento(Guid id)
        {
            Evento evento = dbDataContext.Evento.Find(id);
            if (evento == null)
            {
                return NotFound();
            }

            dbDataContext.Evento.Remove(evento);
            dbDataContext.SaveChanges();

            return Ok(eventoMapper.EventoToEventoDto(evento));
        }

        //-------------------------------------------------------
        //-----------------inizio API regali---------------------
        //-------------------------------------------------------

        // GET: api/Evento/RegaloById/5
        //dato un id, restituisce il regalo. l'oggetto restituito è piatto: nella risposta non sono compresi gli oggetti figli
        [HttpGet]
        [Route("RegaloById/{id}")]
        [ResponseType(typeof(RegaloDto))]
        public IHttpActionResult GetRegaloById(Guid id)
        {
            Regalo regalo = dbDataContext.Regalo.Find(id);

            return Ok(regaloMapper.RegaloToRegaloDto(regalo));
        }

        // GET: api/Evento/RegaliByIdEvento/5
        //dato un id categoria, restituisce tutti gli eventi di quella categoria.
        //l'oggetto restituito è piatto: nella risposta non sono compresi gli oggetti figli
        [HttpGet]
        [Route("RegaliByIdEvento/{idEvento}")]
        public async Task<List<RegaloDto>> GetRegaliByIdUtente(Guid idEvento)
        {
            List<Regalo> regali = await dbDataContext.Regalo.Where(x => x.IdEvento == idEvento).ToListAsync();
            return regaloMapper.RegaloToRegaloDtoList(regali);
        }

        // PUT: api/Evento/RegaloUpdate
        //FIXME verificare come si comporta con i figli
        [HttpPut]
        [Route("RegaloUpdate")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateRegalo(Regalo regalo)
        {
            //FIXME
            regalo.ImmagineRegalo = null;
            regalo.Evento = null;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (regalo.Id != null)
            {
                dbDataContext.Regalo.Attach(regalo);
                dbDataContext.Entry(regalo).State = EntityState.Modified;
            }

            try
            {
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

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Evento/RegaloCreate
        [HttpPost]
        [Route("RegaloCreate", Name = "RegaloCreate")]
        [ResponseType(typeof(Evento))]
        public IHttpActionResult InserisciRegalo(RegaloDto dto)
        {
            Regalo regalo = regaloMapper.RegaloDtoToRegalo(dto);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

            return CreatedAtRoute("RegaloCreate", new { id = regalo.Id }, regalo);
        }

        // DELETE: api/Evento/RegaloDelete/5
        [HttpDelete]
        [Route("RegaloDelete/{id}")]
        [ResponseType(typeof(Evento))]
        public IHttpActionResult DeleteRegalo(Guid id)
        {
            Regalo regalo = dbDataContext.Regalo.Find(id);
            if (regalo == null)
            {
                return NotFound();
            }

            dbDataContext.Regalo.Remove(regalo);
            dbDataContext.SaveChanges();

            return Ok(regalo);
        }

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
