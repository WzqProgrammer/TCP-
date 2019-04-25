using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.server;
namespace GameServer.controller
{
    abstract class BaseController
    {
        protected RequestCode requestCode = RequestCode.None;
        public virtual string DefaultHandle(string data,Client client,Server server) { return null; }

        public RequestCode RequestCode
        {
            get { return requestCode; }
        }
    }
}
