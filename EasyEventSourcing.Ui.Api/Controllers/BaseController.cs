using System;
using System.Web.Http;
using static EasyEventSourcing.Application.Bootstrapper;

namespace EasyEventSourcing.Ui.Api.Controllers
{
    public class BaseController : ApiController
    {
        private Guid? clientId;

        public PretendApplication App { get; }

        public BaseController()
        {
            App = Bootstrap();
        }

        public Guid ClientId
        {
            get
            {
                if (clientId == null)
                {
                    clientId = App.GetClientId(Request.Headers.UserAgent.ToString());
                    App.GetCartId(clientId.Value);
                }

                return clientId.Value;
            }
        }
    }
}
