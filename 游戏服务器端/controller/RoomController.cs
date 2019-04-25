using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.server;

namespace GameServer.controller
{
    class RoomController: BaseController
    {
        public RoomController()
        {
            requestCode = RequestCode.Room;
        }
        public string CreateRoom(string data, Client client, Server server)
        {
            server.CreateRoom(client);
            return ((int)ReturnCode.Success).ToString() + "," + ((int)RoleType.Blue).ToString();
        }
        public string ListRoom(string data, Client client, Server server)
        {
            StringBuilder sb = new StringBuilder();
            foreach(Room room in server.GetRoomList())
            {
                if(room.IsWaitingJoin())
                {
                    sb.Append(room.GetHouseOwnerData() + "|");
                }
            }
            if(sb.Length == 0)
            {
                sb.Append("0");
            }
            else
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
        public string JoinRoom(string data, Client client, Server server)
        {
            int id = int.Parse(data);
            Room room = server.GetRoomById(id);
            if(room == null)
            {
                return ((int)ReturnCode.NotFound).ToString();
            }
            else if(room.IsWaitingJoin() == false)
            {
                return ((int)ReturnCode.Fail).ToString();
            }
            else
            {
                room.AddClient(client);
                string roomData = room.GetRoomData();//returnCode,roleType-房间的玩家数据
                room.BroadcastMessage(client, ActionCode.UpdateRoom, roomData);
                return ((int)ReturnCode.Success).ToString() + "," + ((int)RoleType.Red).ToString() + "-" + roomData;
            }
        }
        public string QuitRoom(string data, Client client, Server server)
        {
            Room room = client.Room;
            bool isHouseOwner = client.IsHouseOwner();
            if(isHouseOwner)    //是房主
            {
                room.BroadcastMessage(client, ActionCode.QuitRoom, ((int)ReturnCode.Success).ToString());   //让非房主退出
                room.Close();
                return ((int)ReturnCode.Success).ToString();
            }
            else   
            {
                client.Room.RemoveClient(client);
                room.BroadcastMessage(client, ActionCode.UpdateRoom, room.GetRoomData());
                return ((int)ReturnCode.Success).ToString();
            }
        }
        public void ShowTimer(string data, Client client, Server server)
        {
            client.Room.StartTimer();
        }
    }
}
