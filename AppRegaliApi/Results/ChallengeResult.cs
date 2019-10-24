using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace AppRegaliApi.Results
{
    public class ChallengeResult : IHttpActionResult
    {
        public ChallengeResult(string loginProvider, ApiController controller)
        {
            LoginProvider = loginProvider;
            Request = controller.Request;
        }

        public string LoginProvider { get; set; }
        public HttpRequestMessage Request { get; set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            Request.GetOwinContext().Authentication.Challenge(LoginProvider);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = Request;
            return Task.FromResult(response);
        }
    }


    //internal class ChallengeResult : HttpUnauthorizedResult
    //{
    //    private const string XsrfKey = "xsrfkey";


    //    public ChallengeResult(string provider, string redirectUri)
    //        : this(provider, redirectUri, null, false)
    //    {
    //    }

    //    public ChallengeResult(string provider, string redirectUri, string userId, bool isRerequest)
    //    {
    //        LoginProvider = provider;
    //        RedirectUri = redirectUri;
    //        UserId = userId;
    //        IsRerequest = isRerequest;
    //    }

    //    public string LoginProvider { get; set; }
    //    public string RedirectUri { get; set; }
    //    public string UserId { get; set; }
    //    public bool IsRerequest { get; set; }

    //    public override void ExecuteResult(ControllerContext context)
    //    {
    //        var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
    //        if (UserId != null)
    //        {
    //            properties.Dictionary[XsrfKey] = UserId;
    //        }
    //        if (IsRerequest)
    //        {
    //            properties.Dictionary["auth_type"] = "rerequest";
    //        }
    //        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
    //    }
    //}
}
