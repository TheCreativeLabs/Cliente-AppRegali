using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using AppRegaliApi.Models;
using AppRegaliApi.Providers;
using AppRegaliApi.Results;
using System.Linq;
using System.Web.Script.Serialization;
using AppRegaliApi.Utility;
using System.Net;

namespace AppRegaliApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
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

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(), //externalLogin.Email, 
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            Authentication.SignOut(DefaultAuthenticationTypes.ExternalBearer);
            Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            
            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider,this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                // IEnumerable<Claim> claims = externalLogin.Claims;
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                //IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterUserBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            //registro l'utente nella tabella UserInfo del db DATA
            DbDataContext dbDataContext = new DbDataContext();

            UserInfo userInfo = new UserInfo()
            {
                Cognome = model.Surname,
                Nome = model.Name,
                IdAspNetUser = new Guid(user.Id),
                Id = new Guid(),
                FotoProfilo = model.ImmagineProfilo,
                DataDiNascita = model.DataNascita
            };

            dbDataContext.UserInfo.Add(userInfo);
            dbDataContext.SaveChanges();

            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
          
            var callbackUrl = Url.Link("Default", new { Controller = "Api/Account", Action = "ConfirmEmail", UserId = user.Id, Code = code });
            
            await EmailService.SendAsync(user.Email,
               "Confirm your account",
               "Please confirm your account by clicking this link: <a href=\""
                                               + callbackUrl + "\">link</a>");

            return Ok();
        }

        //Get api/Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        [Route("ConfirmEmail")]
        public async Task<IHttpActionResult> ConfirmEmail(string UserId, string Code)
        {
            if (String.IsNullOrEmpty(UserId) || String.IsNullOrEmpty(Code))
            {
                return BadRequest(ModelState);
            }

            var result = await UserManager.ConfirmEmailAsync(UserId,Code);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        //GET api/Account/RestorePassword
        [HttpGet]
        [AllowAnonymous]
        [Route("RestorePassword")]
        public async Task<IHttpActionResult> RestorePassword(string Email)
        {
            if (String.IsNullOrEmpty(Email))
            {
                return BadRequest(ModelState);
            }

            List<ApplicationUser> listaUtenti = UserManager.Users.Where(users => users.Email == Email).ToList();

            if (listaUtenti != null && listaUtenti.Count > 0)
            {
              ApplicationUser user =  listaUtenti[0];
                string userId = user.Id;
                string  code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                string newPassword = System.Web.Security.Membership.GeneratePassword(8, 2);
                var result = UserManager.ResetPassword(userId, code, newPassword);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
                //FIXME TESTO
                await EmailService.SendAsync(user.Email,
                 "Password reset",
                 "La tua nuova password è:" + newPassword);
                }
            
            return Ok();
        }


        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identity = (ClaimsIdentity)User.Identity;
            var nome = identity.Claims.FirstOrDefault(x => x.Type == "first_name");
            var cognome = identity.Claims.FirstOrDefault(x => x.Type == "last_name");
            var datiPicture = identity.Claims.FirstOrDefault(x => x.Type == "picture");
            var dataDiNascitaString = identity.Claims.FirstOrDefault(x => x.Type == "dateofbirth");

            //var info = await Authentication.GetExternalLoginInfoAsync();
            var info = await AuthenticationManager_GetExternalLoginInfoAsync_WithExternalBearer();

            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            DbDataContext dbDataContext = new DbDataContext();
            //Registra l'utente dentro la tabella UserInfo del db Data prendendo i valori dai Claims
            UserInfo userInfo = new UserInfo()
            {
                Cognome = cognome != null ? cognome.Value : null,
                Nome = nome != null ? nome.Value : null,
                IdAspNetUser = new Guid(user.Id),
                Id = Guid.NewGuid()
            };
            //se è presente, setto data di nascita
            if (dataDiNascitaString != null) {
                userInfo.DataDiNascita = (DateTime.ParseExact(dataDiNascitaString.Value, "MM/dd/yyyy",
                                            System.Globalization.CultureInfo.InvariantCulture));
            }
            //se è presente, setta url della foto
            if (datiPicture != null)
            {
                //JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                //FacebookImageData imageData = (FacebookImageData)json_serializer.DeserializeObject(datiPicture.Value);
                //userInfo.PhotoUrl = imageData.data.url;
                datiPicture.Value.IndexOf("url\": \"");
                var url = datiPicture.Value.Substring(datiPicture.Value.IndexOf("url\": \"")+7);
                url = url.Substring(0, url.IndexOf("\""));
                userInfo.PhotoUrl = url;
                using (var webClient = new WebClient())
                {
                    byte[] imageBytes = webClient.DownloadData(url);
                    userInfo.FotoProfilo = imageBytes;
                }

            }


            dbDataContext.UserInfo.Add(userInfo);
            dbDataContext.SaveChanges();


            return Ok();
        }

        // GET api/Account/UserInfo
        //[HttpGet]
        //[Route("UserDetail")]
        //public UserInfoDto GetUserDetail()
        //{
        //    DbDataContext dbDataContext = new DbDataContext();
        //    UserInfoMapper userInfoMapper = new UserInfoMapper();
        //    UserInfo userInfo = dbDataContext.UserInfo.Where(user => user.IdAspNetUser.ToString() == User.Identity.GetUserId()).FirstOrDefault();
        //    UserInfoDto userInfoDto = UserInfoMapper.UserInfoToUserInfoDto(userInfo, UserManager.GetEmail(User.Identity.GetUserId()));

        //    return userInfoDto;
        //}


        


        private async Task<ExternalLoginInfo> AuthenticationManager_GetExternalLoginInfoAsync_WithExternalBearer()
        {
            ExternalLoginInfo loginInfo = null;

            var result = await Authentication.AuthenticateAsync(DefaultAuthenticationTypes.ExternalBearer);

            if (result != null && result.Identity != null)
            {
                var idClaim = result.Identity.FindFirst(ClaimTypes.NameIdentifier);
                if (idClaim != null)
                {
                    loginInfo = new ExternalLoginInfo()
                    {
                        DefaultUserName = result.Identity.Name == null ? "" : result.Identity.Name.Replace(" ", ""),
                        Login = new UserLoginInfo(idClaim.Issuer, idClaim.Value)
                    };
                }
            }
            return loginInfo;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
        
        public class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public IList<Claim> Claims { get; private set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

               claims = claims.Concat(Claims).ToList();

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Email),
                    Email = identity.FindFirstValue(ClaimTypes.Email),
                    Claims = identity.Claims.ToList()
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion

        #region MetodiCustom


        [HttpPut]
        [Route("UpdateUser")]
        public async Task<IHttpActionResult> UpdateUser(UpdateUserBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string idUser = User.Identity.GetUserId();
            ApplicationDbContext applicationDbContext = new ApplicationDbContext();
            List<ApplicationUser> applicationUsers = applicationDbContext.Users.Where(x => x.Id == idUser).ToList();

            if (!applicationUsers.Any())
            {
                return NotFound();
            }

            DbDataContext dbDataContext = new DbDataContext();
            List<UserInfo> userInfos = dbDataContext.UserInfo.Where(x => x.IdAspNetUser == new Guid(idUser)).ToList();

            //Controllo se c'è il record dentro dbData
            if (!userInfos.Any())
            {
                return NotFound();
            }

            userInfos[0].Nome = model.Name;
            userInfos[0].Cognome = model.Surname;
            userInfos[0].DataDiNascita = (model.DataNascita ?? null);
            userInfos[0].FotoProfilo = (model.ImmagineProfilo ?? null);
            dbDataContext.SaveChanges();

            //TODO: devo modificare anche la Email?

            return Ok();
        }

        #endregion
    }
}
