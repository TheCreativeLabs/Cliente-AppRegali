using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using AppRegaliApi.Models;

namespace AppRegaliApi.Controllers
{
    public class EventoController : ApiController
    {
        private DbDataContext dbDataContext = new DbDataContext();

        public class ModelloEvento{
            public string titolo { get; set; }
        }

        public class ModelloTest
        {
            public List<ModelloEvento> Eventi { get; set; }
        }


    // GET: api/Evento
    [HttpGet]
        public IEnumerable<Evento> GetEventi()
        {
            List<Evento> eventi = dbDataContext.Evento.ToList();
            return eventi;
        }

        // GET: api/Evento/5
        [ResponseType(typeof(Evento))]
        public IHttpActionResult GetEventoById(Guid id)
        {
            Evento evento = dbDataContext.Evento.Find(id);
            if (evento == null)
            {
                return NotFound();
            }

            return Ok(evento);
        }

        // PUT: api/Evento/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEvento(Guid id, Evento evento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != evento.Id)
            {
                return BadRequest();
            }

            dbDataContext.Evento.Add(evento);

            try
            {
                dbDataContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventoExists(id))
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

        // POST: api/Evento
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