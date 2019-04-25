using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
namespace GameServer.server
{
    class Message
    {
        byte[] data = new byte[1024];
        private int startIndex = 0;

        public byte[] Data
        {
            get { return data; }
        }
        public int StartIndex
        {
            get { return startIndex; }
        }
        public int RemainSize       //缓冲区剩余空间大小
        {
            get { return data.Length - startIndex; }
        }
        //public void AddCount(int count)
        //{
        //    startIndex += count;
        //}
        /// <summary>
        /// 解析数据
        /// </summary>
        public void ReadMessage(int newDataAmount,Action<RequestCode,ActionCode,string> processDataCallBack)
        {
            startIndex += newDataAmount;
            while (true)
            {
                if (startIndex <= 4) return;
                int count = BitConverter.ToInt32(data, 0);  //从数据中读取前4个字节,count的值就是包前端表示包中数据长度的值
                if (startIndex - 4 >= count)        //startIndex-4 表示的是接收到的真正的数据长度,该值大于等于count表示接收到的数据是完整的
                {
                    //string s = Encoding.UTF8.GetString(data, 4, count);
                    //Console.WriteLine("解析出来一条数据:" + s);
                    RequestCode requestCode = (RequestCode)BitConverter.ToInt32(data, 4);
                    ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 8);
                    string s = Encoding.UTF8.GetString(data, 12, count-8);//requestCode和actionCode共占用了8个字节
                    processDataCallBack(requestCode, actionCode, s);
                    Array.Copy(data, count + 4, data, 0, startIndex - 4 - count);//将未读取的数据前移
                    startIndex -= (count + 4);
                }
                else
                    break;
            }
        }
        public static byte[] PackData(ActionCode actionCode,string data)//将数据组合成一个数组
        {
            byte[] requestCodeBytes = BitConverter.GetBytes((int)actionCode);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            int dataAmount = requestCodeBytes.Length + dataBytes.Length;
            byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
            byte[] newBytes = dataAmountBytes.Concat(requestCodeBytes).ToArray<byte>();
            return newBytes.Concat(dataBytes).ToArray<byte>();
        }
    }
}
