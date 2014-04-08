using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cascade
{
    class TcpObject
    {
        TcpClient client;
        NetworkStream stream;

        public bool Connected
        {
            get
            {
                return client.Connected;
            }
        }
        public TcpObject()
        {
            client = new TcpClient();
        }
        public TcpObject(string ip, int port)
            :this()
        {
            Connect(ip, port);
        }
        public void Connect(string ip, int port)
        {
            client.Connect(ip, port);
            stream = client.GetStream();
            client.ReceiveBufferSize = 64;
        }
        public void Write(string value)
        {
            Write(Encoding.ASCII.GetBytes(value));
        }
        public void Write(byte[] buffer, int offset, int length)
        {
            if (stream != null && stream.CanWrite)
            {
                stream.Write(buffer, offset, length);
            }
        }
        public void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
        }
        public void Close()
        {
            try
            {
                client.Close();
            }
            catch
            {

            }
        }
        public byte[] Read()
        {
            byte[] bytes = new byte[64];
            stream.Read(bytes, 0, 64);
            return bytes;
        }
        public string ReadString(Encoding encoding)
        {
            return encoding.GetString(Read());
        }
    }
}
