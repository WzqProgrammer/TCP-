using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.model
{
    class User
    {
        private int id;
        private string username;
        private string password;

        public User(int id,string username,string password)
        {
            this.id = id;
            this.username = username;
            this.password = password;
        }
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }
        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        public string Password
        {   get { return password; }
            set { password = value; }
        }

    }
}
