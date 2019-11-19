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
using static AppRegaliApi.UserInfoDto;

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

            Guid idUtenteCorrente = new Guid(User.Identity.GetUserId());

            //se idUtenteCorrente == idUser, sto restituendo il profilo dell'utente corrente quindi non c'è nessuna relazione da stabilire: metto ME e restituisco
            if (idUser == idUtenteCorrente)
            {
                dto.Relation = UserRelation.ME;
                return Ok(dto);
            }

            //recupero la relazione tra current e utente con idUser
            UserAmicizia userAmicizia = await dbDataContext.UserAmicizia
                                                            .Where(x => (x.IdUserDestinatario == idUser && x.IdUserRichiedente == idUtenteCorrente) ||
                                                                        (x.IdUserDestinatario == idUtenteCorrente && x.IdUserRichiedente == idUser)
                                                                  ).FirstOrDefaultAsync();
            //se la relazione è null non sono amici
            if (userAmicizia == null)
            {
                dto.Relation = UserRelation.STRANGER;
            }
            //se nella relazione accettato = true i due utente sono già in contatto (sono amici)
            else if (userAmicizia.Accettato == true)
            {
                dto.Relation = UserRelation.CONTACT;
            }
            //se accettato = false e destinatario = current, richiesta in entrata
            else if (userAmicizia.Accettato == false && userAmicizia.IdUserDestinatario == idUtenteCorrente)
            {
                dto.Relation = UserRelation.REQUEST_IN;
            }
            //se accettato = false e richiedente = current, richiesta in uscita
            else if (userAmicizia.Accettato == false && userAmicizia.IdUserRichiedente == idUtenteCorrente)
            {
                dto.Relation = UserRelation.REQUEST_OUT;
            }
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

        [HttpGet]
        [Route("RichiesteCurrentUser")]
        public async Task<List<UserInfoDto>> GetRichiesteAmiciziaOfCurrentUser()
        {
            Guid currentUserId = new Guid(User.Identity.GetUserId());

            List<Guid> idRichieste = await AmiciUtility.GedIdRichiesteOfUser(currentUserId);

            List<UserInfo> utentiRichieste = await dbDataContext.UserInfo.Where(x => idRichieste.Contains(x.IdAspNetUser)).ToListAsync();
            return UserInfoMapper.UserInfoToUserInfoDtoList(utentiRichieste);
        }


        //FIXME attenzione, ignoreCase ha senso in lingue come itaiano e inglese ma non ha senso in lingue come il turco.
        //in caso di introduzione di altre lingue, fixare
        //https://stackoverflow.com/questions/444798/case-insensitive-containsstring
        [HttpGet]
        [Route("RicercaUtenti")]
        public async Task<List<UserInfoDto>> GetRicercaUtenti(string chiave = null)
        {
            List<UserInfoDto> utentiCercati = dbDataContext.UserInfo
                                                    .Where(x => ((string)(x.Nome + " " + x.Cognome).ToLower()).Contains(chiave.ToLower()))
                                                    .Select(x =>
                                                    new UserInfoDto() { Cognome = x.Cognome,
                                                                        Nome = x.Nome,
                                                                        DataDiNascita = x.DataDiNascita,
                                                                        FotoProfilo = x.FotoProfilo,
                                                                        IdAspNetUser = x.IdAspNetUser,
                                                                        PhotoUrl = x.PhotoUrl
                                                                       }
                                                    //UserInfoMapper.UserInfoToUserInfoDto(x, null))
                                                    )
                                                    .ToList();
            //return UserInfoMapper.UserInfoToUserInfoDtoList(utentiRichieste);
            return utentiCercati;
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
