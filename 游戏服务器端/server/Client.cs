using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Common;
using MySql.Data.MySqlClient;
using GameServer.tool;
using GameServer.model;
namespace GameServer.server
{
    class Client
    {
        private Socket clientSocket;
        private Server server;
        private Message message = new Message();
        private MySqlConnection mysqlConn;
        private User user;
        private Result result;
        private Room room;

        public int Hp
        {
            get;
            set;
        }
        public Client() { }
        public Client(Socket clientSocket,Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            mysqlConn = ConnHelper.Connect();
        }
        public User User
        {
            set { user = value; }

        }
        public Result Result
        {
            set { result = value; }
        }
        public Room Room
        {
            set
            {
                room = value;
            }
            get
            {
                return room;
            }
        }
        public void Start()
        {
            if (clientSocket == null || clientSocket.Connected == false) return;
            clientSocket.BeginReceive(message.Data, message.StartIndex, message.RemainSize, SocketFlags.None, ReceiveCallBack, null);
        }

        public MySqlConnection MySqlConnection
        {
            get
            {
                return this.mysqlConn;
            }
        }
        public void SetUserData(User user, Result result)
        {
            this.user = user;
            this.result = result;
        }
        public string GetUserData()
        {
            return user.Id + "," + user.Username + "," + result.TotalCount + "," + result.WinCount;
        }
        public void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                if (clientSocket == null || clientSocket.Connected == false) return;
                int count = clientSocket.EndReceive(ar);
                if (count == 0)//断开连接
                {
                    Console.WriteLine(clientSocket.ToString() + "断开连接");
                    Close();
                }
                message.ReadMessage(count,OnProcessMessage);
                Start();
            }
            catch (Exception e)
            {
                Close();
                Console.WriteLine(e);
            }
        }
        private void Close()
        {
            ConnHelper.CloseConnection(mysqlConn);
            if (clientSocket != null)
                clientSocket.Close();
            if (room != null)
            {
                room.QuitRoom(this);
            }
            server.RemoveClient(this);  //移除自身
        }
        private void OnProcessMessage(RequestCode requestCode,ActionCode actionCode,string data)
        {
            server.HandleRequest(requestCode, actionCode, data, this);
        }
        public void Send(ActionCode actionCode,string data)
        {
            try
            {
                byte[] bytes = Message.PackData(actionCode, data);
                clientSocket.Send(bytes);
            }catch(Exception e)
            {
                Console.WriteLine("无法发送消息：" + e);
            }
            
            
        }
        public int GetUserId()
        {
            return user.Id;
        }
        public bool IsHouseOwner()
        {
            return room.IsHouseOwner(this);
        }
        public bool TakeDamage(int damage)
        {
            Hp -= damage;
            Hp = Math.Max(Hp, 0);
            if (Hp <= 0)
                return true;
            else
                return false;
        }
        public bool IsDie()
        {
            return Hp <= 0;
        }
    }
}
