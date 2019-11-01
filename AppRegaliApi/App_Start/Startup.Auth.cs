using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using AppRegaliApi.Providers;
using AppRegaliApi.Models;
using Microsoft.Owin.Security.Facebook;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Web.WebPages.OAuth;
using Namespace.Helpers;
using System.Security.Claims;

namespace AppRegaliApi
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            //app.UseCookieAuthentication(new CookieAuthenticationOptions());
            //app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);


            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
            };


           

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //var facebookOption = new FacebookAuthenticationOptions()
            //{
            //    AppId = "971997736480952",
            //    AppSecret = "483348891fbc0f94cf3a6e40cdbbaf1d",
            //    BackchannelHttpHandler = new FacebookBackChannelHandler(),
            //    UserInformationEndpoint = "https://graph.facebook.com/v2.4/me/?fields=id,email,name"
            //};

            //facebookOption.Scope.Add("email");

            //app.UseFacebookAuthentication(facebookOption);

            var options = new FacebookAuthenticationOptions
            {
                AppId = "971997736480952",
                AppSecret = "483348891fbc0f94cf3a6e40cdbbaf1d",

                Provider = new FacebookProvider
                {
                    OnAuthenticated = async context =>
                    {
                        foreach (var x in context.User)
                        {
                            if (x.Key == "birthday")
                            {
                                context.Identity.AddClaim(new Claim("dateofbirth", x.Value.ToString()));
                            }
                            else
                            {
                                context.Identity.AddClaim(new Claim(x.Key, x.Value.ToString()));
                            }
                        }
                        context.Identity.AddClaim(new Claim("fb_accecctoken", context.AccessToken));

                       
                        await Task.FromResult(context);
                    }


                },
                UserInformationEndpoint = "https://graph.facebook.com/v2.5/me?fields=id,name,email,first_name,last_name,location,birthday,picture.width(500).height(500)",
            };
            options.Scope.Add("public_profile");
            options.Scope.Add("email");
            options.Scope.Add("user_birthday");
            options.Scope.Add("user_location");
            //options.Scope.Add("picture");
            options.Fields.Add("birthday");
            options.Fields.Add("picture.width(500).height(500)");

            app.UseFacebookAuthentication(options);

            //OAuthWebSecurity.RegisterClient(
            //    new MyFacebookClient(
            //        appId: "971997736480952",
            //        appSecret: "483348891fbc0f94cf3a6e40cdbbaf1d"),
            //    "facebook", null
            //);

            //app.UseFacebookAuthentication(
            //    appId: "971997736480952",
            //    appSecret: "483348891fbc0f94cf3a6e40cdbbaf1d")
            //    ;

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }


    }
}
