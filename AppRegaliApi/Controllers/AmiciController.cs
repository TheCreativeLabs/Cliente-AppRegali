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
        public async Task<IHttpActionResult> GetUserInfoByIdUsersAsync(Guid idUser)
        {
            UserInfo userInfo = dbDataContext.UserInfo.SingleOrDefault(x => x.IdAspNetUser == idUser);
            string mail = await UserManager.GetEmailAsync(idUser.ToString());
            //FIXME inserire anche immagine da Facebook 
            //ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            UserInfoDto dto = userInfoMapper.UserInfoToUserInfoDto(userInfo, mail);
            return Ok(userInfo);
        }
        
        [HttpGet]
        [Route("CurrentUserInfo")]
        [ResponseType(typeof(UserInfo))]
        public async Task<IHttpActionResult> GetCurrentUserInfoAsync() 
        {
            return await GetUserInfoByIdUsersAsync(new Guid(User.Identity.GetUserId()));
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

            List<Guid> idAmici = await AmiciUtility.GetIdAmiciOfUser(currentUserId);

            List<UserInfo> amici = await dbDataContext.UserInfo.Where(x => idAmici.Contains(x.IdAspNetUser)).ToListAsync();
            return amici;
        }

        //[HttpPost]
        //[Route("AmiciziaCreate/{idDestinatario}", Name = "AmiciziaCreate")]
        //[ResponseType(typeof(Evento))]
        //public IHttpActionResult InserisciAmicizia([FromUri]String idDestinatario)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    UserAmicizia amicizia = new UserAmicizia();
        //    amicizia.IdUserDestinatario = idDestinatario;
        //    amicizia.IdUserRichiedente = User.Identity.

        //    dbDataContext.ImmagineEvento.Attach(evento.ImmagineEvento);
        //    dbDataContext.Entry(evento.ImmagineEvento).State = EntityState.Added;
        //    dbDataContext.Evento.Add(evento);

        //    try
        //    {
        //        dbDataContext.SaveChanges();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (EventoExists(evento.Id))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtRoute("EventoCreate", new { id = evento.Id }, evento);
        //}

        //todo crea amicizia, rimuovi amicizia, get stato amicizia




    }
}
