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
    [RoutePrefix("api/Amici")]
    public class AmiciController : ApiController
    {
        private DbDataContext dbDataContext = new DbDataContext();

        [HttpGet]
        [Route("UserInfoByIdUser/{idUser}")]
        [ResponseType(typeof(UserInfo))]
        public IHttpActionResult GetUserInfoByIdUsera(Guid idUser)
        {
            UserInfo userInfo = dbDataContext.UserInfo.SingleOrDefault(x => x.IdAspNetUser == idUser);
            return Ok(userInfo);
        }


        //FIXME TESTARE
        // GET: api/Amici/AmiciCurrentUser
        //restituisce gli amici dell'utente corrente.
        /// <summary>
        /// Trova gli amici dell'utente corrente, ovvero gli utenti che hanno accettato l'amicizia.
        /// Tra gli amici che hanno accettato sono inclusi sia gli amici che hanno fatto richiesta sia gli amici che hanno ricevuto richiesta
        /// </summary>
        /// <returns>List<UserInfo> : amici dell'utente corrente</returns>
        [HttpGet]
        [Route("AmiciCurrentUser")]
        public async Task<List<UserInfo>> GetAmiciOfCurrentUser()
        {
            Guid currentUserId = new Guid(User.Identity.GetUserId());
            //Per trovare tutti gli amici del current, devo considerare tutte le righe di UserAmicizia 
            //sia nel caso in cui current è destinatario che nel caso in cui current è richiedente

            //current è destinatario, gli amici sono richiedenti
            List<Guid> idAmiciRichiedenti = await dbDataContext.UserAmicizia
                                                    .Where(x => ((x.IdUserDestinatario == currentUserId) && (x.Accettato)))
                                                    .Select(x => x.IdUserRichiedente)
                                                    .ToListAsync();

            //current è richiedente, gli amici sono destinatari
            List<Guid> idAmiciDestinatari = await dbDataContext.UserAmicizia
                                                    .Where(x => ((x.IdUserRichiedente == currentUserId) && (x.Accettato)))
                                                    .Select(x => x.IdUserDestinatario)
                                                    .ToListAsync();

            List<Guid> idAmiciAll = idAmiciRichiedenti.Union(idAmiciDestinatari).ToList();

            List<UserInfo> amici = await dbDataContext.UserInfo.Where(x => idAmiciAll.Contains(x.IdAspNetUser)).ToListAsync();
            return amici;
        }



        
    }
}
