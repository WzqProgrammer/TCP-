using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.server;
using GameServer.dao;
using GameServer.model;
namespace GameServer.controller
{
    class UserController : BaseController
    {
        private UserDao userDao = new UserDao();
        private ResultDao resDao = new ResultDao();
        public UserController()
        {
            requestCode = RequestCode.User;
        }

        public string Login(string data, Client client,Server server)
        {
            string[] strs = data.Split(',');
            User user = userDao.VerifyUser(client.MySqlConnection, strs[0], strs[1]);
            if (user == null)
            {
                //Enum.GetName(typeof(ReturnCode), ReturnCode.Fail);
                return ((int)(ReturnCode.Fail)).ToString();
            }
            else
            {
                Result res = resDao.GetResultByUserId(client.MySqlConnection, user.Id);
                client.SetUserData(user, res);
                return string.Format("{0},{1},{2},{3}", ((int)(ReturnCode.Success)).ToString(), user.Username, res.TotalCount, res.WinCount);
            }
        }
        public string Register(string data, Client client, Server server)
        {
            string[] strs = data.Split(',');
            string username = strs[0];
            string password = strs[1];
            bool res = userDao.GetUserByUsername(client.MySqlConnection, username);
            if(res) //找到了要注册的用户名，注册失败
            {
                return ((int)ReturnCode.Fail).ToString();
            }
            userDao.AddUser(client.MySqlConnection, username, password);
            return ((int)ReturnCode.Success).ToString();
        }
    }
}
