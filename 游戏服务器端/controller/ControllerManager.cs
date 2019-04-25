using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Reflection;
using GameServer.server;
namespace GameServer.controller
{
    class ControllerManager
    {
        private Dictionary<RequestCode, BaseController> controllerDict = new Dictionary<RequestCode, BaseController>();
        private Server server;
        public ControllerManager(Server server)
        {
            this.server = server;
            Init();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
            DefaultController defaultController = new DefaultController();
            controllerDict.Add(defaultController.RequestCode, defaultController);
            controllerDict.Add(RequestCode.User, new UserController());
            controllerDict.Add(RequestCode.Room, new RoomController());
            controllerDict.Add(RequestCode.Game, new GameController());
        }
        /// <summary>
        /// 处理客户端的请求
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="actionCode"></param>
        /// <param name="data"></param>
        public void HandleRequest(RequestCode requestCode,ActionCode actionCode,string data,Client client)
        {
            BaseController controller;
            bool isGet = controllerDict.TryGetValue(requestCode, out controller);
            if (isGet == false)
                Console.WriteLine("无法得到["+requestCode+"]所对应的controller，无法处理请求");
            string methodName = Enum.GetName(typeof(ActionCode), actionCode);//将枚举类型转换为方法名
            MethodInfo mi = controller.GetType().GetMethod(methodName);//利用反射机制调用方法
            if(mi==null)
            {
                Console.WriteLine("[警告]在controller[" + controller.GetType() + "中没有对应的处理方法");
                return;
            }
            object[] parameters = { data,client,server};
            object o = mi.Invoke(controller,parameters);//controller对象调用这个方法，方法的参数是数组parameters里面的值
            if(o==null||string.IsNullOrEmpty(o as string))
            {
                return;
            }
            server.SendResponse(client, actionCode,o as string);
        }
    }
}
