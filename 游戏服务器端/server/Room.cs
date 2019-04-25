using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Common;

namespace GameServer.server
{
    enum RoomState
    {
        WaitingJoin,
        WaitingBattle,
        Battle,
        End
    }
    class Room
    {
        private const int MAX_HP = 200;
        private List<Client> clientRoom = new List<Client>();
        private RoomState state = RoomState.WaitingJoin;
        private Server server;
        public Room(Server server)
        {
            this.server = server;
        }
        public bool IsWaitingJoin()
        {
            return state == RoomState.WaitingJoin;
        }
        public void AddClient(Client client)
        {
            client.Hp = MAX_HP;
            clientRoom.Add(client);
            client.Room = this;
            if(clientRoom.Count >= 2)
            {
                state = RoomState.WaitingBattle;
            }
        }
        //得到房主
        public string GetHouseOwnerData()
        {
            return clientRoom[0].GetUserData();
        }
        //关闭房间
        
        public int GetId()
        {
            if(clientRoom.Count > 0)
            {
                return clientRoom[0].GetUserId();
            }
            return -1;
        }
        public string GetRoomData()
        {
            StringBuilder sb = new StringBuilder();
            foreach(Client client in clientRoom)
            {
                sb.Append(client.GetUserData() + "|");
            }
            if(sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
        public void BroadcastMessage(Client excludeClient, ActionCode actionCode, string data)
        {
            foreach(Client client in clientRoom)
            {
                if(client != excludeClient)
                {
                    server.SendResponse(client, actionCode, data);
                }
            }
        }
        public bool IsHouseOwner(Client client)
        {
            return client == clientRoom[0];
        }
        public void RemoveClient(Client client)
        {
            client.Room = null;
            clientRoom.Remove(client);
            if(clientRoom.Count >= 2)
            {
                state = RoomState.WaitingBattle;
            }
            else
            {
                state = RoomState.WaitingJoin;
            }
        }
        public void Close()
        {
            foreach(Client client in clientRoom)
            {
                client.Room = null;
            }
            server.RemoveRoom(this);
        }
        public void QuitRoom(Client client)
        {
            if (client == clientRoom[0])
            {
                server.RemoveRoom(this);
            }
            else
                clientRoom.Remove(client);
        }
        public void StartTimer()
        {
            new Thread(RunTimer).Start();
        }
        private void RunTimer()
        {
            for(int i = 3; i > 0; i--)
            {
                BroadcastMessage(null, ActionCode.ShowTimer, i.ToString());
                Thread.Sleep(1000);
            }
            BroadcastMessage(null, ActionCode.StartPlay, "r");
        }
        public void TakeDamage(int damage, Client excludeClient)
        {
            bool isDie = false;
            foreach (Client client in clientRoom)
            {
                if (client != excludeClient)
                {
                    if (client.TakeDamage(damage))
                    {
                        isDie = true;
                    }
                }
            }
            if (isDie == false)
                return;
            //如果其中一个角色死亡，要结束游戏
            foreach(Client client in clientRoom)
            {
                if(client.IsDie())
                {
                    client.Send(ActionCode.GameOver, ((int)ReturnCode.Fail).ToString());
                }
                else
                {
                    client.Send(ActionCode.GameOver, ((int)ReturnCode.Success).ToString());
                }
            }
            Close();
        }
    }
}
