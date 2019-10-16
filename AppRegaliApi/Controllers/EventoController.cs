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
        //FIXME togliere la ricorsione
        [HttpGet]
        [Route("Eventi")]
        public async Task<List<Evento>> GetEventi()
        {
            // List<Evento> eventi = await dbDataContext.Evento.ToListAsync();

            List<Evento> eventi = await dbDataContext.Evento.ToListAsync();
            return eventi;
        }

        [HttpGet]
        [Route("EventiCurrentUser")]
        //FIXME togliere la ricorsione
        public async Task<List<Evento>> GetEventiOfCurrentUser()
        {
            Guid currentUserId = new Guid(User.Identity.GetUserId());
            List<Evento> eventi = await dbDataContext.Evento.Where(x => x.IdUtenteCreazione.ToString().ToUpper().Equals(currentUserId.ToString().ToUpper(),StringComparison.OrdinalIgnoreCase)).ToListAsync();
            return eventi;
        }

        // GET: api/Evento/EventoById/5
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

        // PUT: api/Evento/5
        //FIXME DOVREBBE SSERE UPDATE MNA FA INSERT
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEvento(Evento evento)
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

            /*if (id != evento.Id)
            {
                return BadRequest();
            }*/

            //sdbDataContext.Evento.
            dbDataContext.Evento.Add(evento);

            try
            {
                dbDataContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                /*if (!EventoExists(id))
                {
                    return NotFound();
                }
                else
                {*/
                    throw;
                //}
            }



            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Evento
        //FIXME
        [ResponseType(typeof(Evento))]
        public IHttpActionResult PostEvento(Evento evento)
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

            return CreatedAtRoute("DefaultApi", new { id = evento.Id }, evento);
        }

        // DELETE: api/Evento/5
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