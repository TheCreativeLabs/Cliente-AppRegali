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
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        public AmiciController()
        {
        }

        public AmiciController(ApplicationUserManager userManager)
        {
        }

        [HttpGet]
        [Route("UserInfoByIdUser/{idUser}")]
        [ResponseType(typeof(UserInfoDto))]
        public async Task<IHttpActionResult> GetUserInfoByIdUsersAsync(Guid idUser)
        {
            UserInfo userInfo = await dbDataContext.UserInfo.SingleOrDefaultAsync(x => x.IdAspNetUser == idUser);
            string email = await dbContext.Users.Where(x => x.Id == idUser.ToString()).Select(x => x.Email).FirstOrDefaultAsync();

            //FIXME inserire anche immagine da Facebook 
            //ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            UserInfoDto dto = UserInfoMapper.UserInfoToUserInfoDto(userInfo, email);
            return Ok(dto);
        }
        
        [HttpGet]
        [Route("CurrentUserInfo")]
        [ResponseType(typeof(UserInfoDto))]
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
        public async Task<List<UserInfoDto>> GetAmiciOfCurrentUser()
        {
            Guid currentUserId = new Guid(User.Identity.GetUserId());

            List<Guid> idAmici = await AmiciUtility.GetIdAmiciOfUser(currentUserId);

            List<UserInfo> amici = await dbDataContext.UserInfo.Where(x => idAmici.Contains(x.IdAspNetUser)).ToListAsync();
            //var query =
            //           from info in dbDataContext.UserInfo
            //           join user in dbContext.Users on info.IdAspNetUser equals user.Id
            //           where idAmici.Contains(info.IdAspNetUser)
            //           select new UserInfoDto { Nome = info.Nome, 
            //                                    Cognome = info.Cognome,
            //                                    FotoProfilo = info.FotoProfilo,
            //                                    Email = user.Email};
            return UserInfoMapper.UserInfoToUserInfoDtoList(amici);
        }

        [HttpPost]
        [Route("AmiciziaCreate", Name = "AmiciziaCreate")]
        [ResponseType(typeof(UserAmicizia))]
        public IHttpActionResult InserisciAmicizia(String idDestinatario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            UserAmicizia amicizia = new UserAmicizia();
            amicizia.IdUserDestinatario = new Guid(idDestinatario);
            amicizia.IdUserRichiedente = new Guid(User.Identity.GetUserId());
            amicizia.Accettato = false;
            dbDataContext.UserAmicizia.Add(amicizia);

            try
            {
                dbDataContext.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (UserAmiciziaExists(amicizia.IdUserDestinatario, amicizia.IdUserRichiedente))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(amicizia); ;
        }

        [HttpPut]
        [Route("AmiciziaAccetta")]
        [ResponseType(typeof(UserAmicizia))]
        public async Task<IHttpActionResult> AccettaAmicizia(String idAmico)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Guid guidCurrentUser = new Guid(User.Identity.GetUserId());
            UserAmicizia amicizia = await dbDataContext.UserAmicizia.Where(x => x.IdUserRichiedente == new Guid(idAmico) && x.IdUserDestinatario == guidCurrentUser).FirstAsync();
            if (amicizia == null)
            {
                return NotFound();
            }
            amicizia.Accettato = true;

            try
            {
                dbDataContext.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (!UserAmiciziaExists(amicizia.IdUserDestinatario, amicizia.IdUserRichiedente))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("AmiciziaCreate",
                                   new { IdUserDestinatario = amicizia.IdUserDestinatario, IdUserRichiedente = amicizia.IdUserRichiedente },
                                   amicizia);
        }

        //todo get stato amicizia


        [HttpDelete]
        [Route("AmiciziaDeleteOrDeny", Name = "AmiciziaDeleteOrDeny")]
        [ResponseType(typeof(UserAmicizia))]
        public async Task<IHttpActionResult> AmiciziaDeleteOrDeny(Guid IdAmico)
        {
            if (IdAmico == null || IdAmico == Guid.Empty || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Guid guidCurrentUser = new Guid(User.Identity.GetUserId());

            UserAmicizia userAmicizia = await dbDataContext.UserAmicizia
                                                .FirstOrDefaultAsync(x => (
                                                                            (x.IdUserRichiedente == guidCurrentUser && x.IdUserDestinatario == IdAmico)
                                                                            || (x.IdUserRichiedente == IdAmico && x.IdUserDestinatario == guidCurrentUser)
                                                                          )
                                                                    );

            if (userAmicizia == null)
            {
                return NotFound();
            }

            try
            {
                dbDataContext.UserAmicizia.Remove(userAmicizia);
                dbDataContext.SaveChanges();
            }
            catch
            {
                throw;
            }

            return Ok();

        }

        private bool UserAmiciziaExists(Guid IdUserDestinatario, Guid IdUserRichiedente)
        {
            return dbDataContext.UserAmicizia.Count(
                e => e.IdUserDestinatario == IdUserDestinatario && e.IdUserRichiedente == IdUserRichiedente
                ) > 0;
        }


    }
}
