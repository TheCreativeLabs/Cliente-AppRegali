using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AppRegaliApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using static AppRegaliApi.Controllers.AccountController;

namespace AppRegaliApi.Controllers
{
    
    [Authorize]
    [RoutePrefix("api/Amici")]
    public class AmiciController : ApiController
    {
        private DbDataContext dbDataContext = new DbDataContext();
        private ApplicationUserManager _userManager;
        UserInfoMapper userInfoMapper = new UserInfoMapper();

        public AmiciController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpGet]
        [Route("UserInfoByIdUser/{idUser}")]
        [ResponseType(typeof(UserInfoDto))]
        public async Task<IHttpActionResult> GetUserInfoByIdUsers(Guid idUser)
        {
            UserInfo userInfo = await dbDataContext.UserInfo.SingleOrDefaultAsync(x => x.IdAspNetUser == idUser);
            return Ok(userInfo);
        }
        
        [HttpGet]
        [Route("CurrentUserInfo")]
        [ResponseType(typeof(UserInfo))]
        public IHttpActionResult GetCurrentUserInfo() 
        {

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            externalLogin.

            return GetUserInfoByIdUsers(new Guid(User.Identity.GetUserId()));
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

        //todo crea amicizia, rimuovi amicizia, get stato amicizia



        
    }
}
