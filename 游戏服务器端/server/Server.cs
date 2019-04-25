using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using GameServer.controller;

namespace GameServer.server
{
    class Server
    {
        private IPEndPoint ipEndPoint;
        private Socket serverSocket;
        private List<Client> clientList = new List<Client>();
        private ControllerManager controllerManager;
        private List<Room> roomList = new List<Room>();
        public Server()
        {

        }
        public Server(string ipStr,int port)
        {
            controllerManager = new ControllerManager(this);
            this.SetIpAndPort(ipStr, port);
        }
        public void SetIpAndPort(string ipStr, int port)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
        }
        public void Start()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipEndPoint);
            serverSocket.Listen(0); //等待队列的长度为无限大
            Console.WriteLine("等待连接...");
            serverSocket.BeginAccept(AcceptCallBack, null);
        }
        private void AcceptCallBack(IAsyncResult ar)
        {
            //Socket serverSocket = ar.AsyncState as Socket;
            Socket clientSocket = serverSocket.EndAccept(ar);
            Client client = new Client(clientSocket,this);
            Console.WriteLine("有新的连接:" + clientSocket.ToString());
            client.Start();
            clientList.Add(client);
            serverSocket.BeginAccept(AcceptCallBack, null);
        }
        public void RemoveClient(Client client)
        {
            lock(clientList)    //可能有多个客户端需要移除，因此需要加锁
            {
                clientList.Remove(client);
            }
        }
        public void SendResponse(Client client, ActionCode actionCode, string data)
        {
            client.Send(actionCode, data);
        }
        public void HandleRequest(RequestCode requestCode, ActionCode actionCode, string data, Client client)
        {
            controllerManager.HandleRequest(requestCode, actionCode, data, client);
        }
        public void CreateRoom(Client client)
        {
            Room room = new Room(this);
            room.AddClient(client);
            roomList.Add(room);
        }
        public void RemoveRoom(Room room)
        {
            if(roomList != null && room != null)
            {
                roomList.Remove(room);
            }
        }
        public List<Room> GetRoomList()
        {
            return roomList;
        }
        public Room GetRoomById(int id)
        {
            foreach(Room room in roomList)
            {
                if (room.GetId() == id)
                    return room;
            }
            return null;
        }
    }
}
