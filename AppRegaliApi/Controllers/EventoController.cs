using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
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

        // GET: api/Evento/Eventi
        //restituisce una lista piatta di eventi: nella risposta non sono compresi gli oggetti figli
        [HttpGet]
        [Route("Eventi")]
        public async Task<List<Evento>> GetEventi()
        {
            List<Evento> eventi = await dbDataContext.Evento.ToListAsync();
            return eventi;
        }

        // GET: api/Evento/EventiCurrentUser
        //restituisce gli eventi dell'utente corrente.
        //restituisce una lista piatta di eventi: nella risposta non sono compresi gli oggetti figli
        [HttpGet]
        [Route("EventiCurrentUser")]
        public async Task<List<Evento>> GetEventiOfCurrentUser()
        {
            Guid currentUserId = new Guid(User.Identity.GetUserId());
            List<Evento> eventi = await dbDataContext.Evento.Where(x => x.IdUtenteCreazione.ToString().ToUpper().Equals(currentUserId.ToString().ToUpper(),StringComparison.OrdinalIgnoreCase)).ToListAsync();
            return eventi;
        }

        // GET: api/Evento/EventoById/5
        //dato un id, restituisce l'evento. l'oggetto restituito è piatto: nella risposta non sono compresi gli oggetti figli
        [HttpGet]
        [Route("EventoById/{id}")]
        [ResponseType(typeof(Evento))]
        public IHttpActionResult GetEventoById(Guid id)
        {
            //Guid guid = new Guid(id);
            Evento evento = dbDataContext.Evento.Find(id);

            if (evento == null)
            {
                return NotFound();
            }

            return Ok(evento);
        }

        // GET: api/Evento/EventoByIdCategoria/5
        //dato un id categoria, restituisce tutti gli eventi di quella categoria.
        //l'oggetto restituito è piatto: nella risposta non sono compresi gli oggetti figli
        [HttpGet]
        [Route("EventiByIdCategoria/{idCategoria}")]
        public async Task<List<Evento>> GetEventiByIdCategoria(Guid idCategoria)
        {
            List<Evento> eventi = await dbDataContext.Evento.Where(x => x.IdCategoriaEvento == idCategoria).ToListAsync();
            return eventi;
        }

        // GET: api/Evento/EventoByIdCategoria/5
        //dato un id categoria, restituisce tutti gli eventi di quella categoria.
        //l'oggetto restituito è piatto: nella risposta non sono compresi gli oggetti figli
        [HttpGet]
        [Route("EventiByIdUtente/{idUtente}")]
        public async Task<List<Evento>> GetEventiByidUtente(Guid idUtente)
        {
            List<Evento> eventi = await dbDataContext.Evento.Where(x => x.IdUtenteCreazione == idUtente).ToListAsync();
            return eventi;
        }

        // PUT: api/Evento/EventoUpdate
        //FIXME verificare come si comporta se un evento ha già dei regalil
        [HttpPut]
        [Route("EventoUpdate")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateEvento(Evento evento)
        {
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
        public IHttpActionResult InserisciEvento(Evento evento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

        // DELETE: api/Evento/5
        [HttpDelete]
        [Route("EventoDelete/{id}")]
        [ResponseType(typeof(Evento))]
        public IHttpActionResult DeleteEvento(Guid id)
        {
            Evento evento = dbDataContext.Evento.Find(id);
            if (evento == null)
            {
                return NotFound();
            }

            dbDataContext.Evento.Remove(evento);
            dbDataContext.SaveChanges();

            return Ok(evento);
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
    }
}