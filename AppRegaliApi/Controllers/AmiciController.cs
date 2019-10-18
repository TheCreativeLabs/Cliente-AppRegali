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

        // GET: api/Evento/EventiCurrentUser
        //restituisce gli eventi dell'utente corrente.
        //restituisce una lista piatta di eventi: nella risposta non sono compresi gli oggetti figli
        [HttpGet]
        [Route("AmiciCurrentUser")]
        public async Task<List<UserInfo>> GetAmiciiOfCurrentUser()
        {
            Guid currentUserId = new Guid(User.Identity.GetUserId());
            //await dbDataContext.UserAmicizia.Where(x => (x.IdUserDestinatario == currentUserId) && (x.Accettato));
            //List<UserInfo> amici = await dbDataContext.UserInfo.Where(x => x.IdUtenteCreazione == currentUserId).ToListAsync();
            //return amici;
            //fixme
            return null;
        }


        
    }
}
