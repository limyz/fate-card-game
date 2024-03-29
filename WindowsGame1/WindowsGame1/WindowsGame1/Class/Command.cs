﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;

namespace WindowsGame1
{
    [Serializable]
    public class Command
    {
        public CommandCode Command_Code;
        public object Data1;
        public object Data2;
        public object Data3;
        public object Data4;
        public object Data5;
        public string Message;
        public int Value_Int;
        public float Value_Float;
        public DateTime Value_Datetime;

        #region Constructors
        public Command()
        { }
        public Command(CommandCode Command_Code)
        {
            this.Command_Code = Command_Code;
        }
        public Command(CommandCode Command_Code, object Data1)
        {
            this.Command_Code = Command_Code;
            this.Data1 = Data1;
        }
        public Command(CommandCode Command_Code, object Data1, object Data2)
        {
            this.Command_Code = Command_Code;
            this.Data1 = Data1;
            this.Data2 = Data2;
        }
        public Command(CommandCode Command_Code, object Data1, object Data2, object Data3)
        {
            this.Command_Code = Command_Code;
            this.Data1 = Data1;
            this.Data2 = Data2;
            this.Data3 = Data3;
        }
        public Command(CommandCode Command_Code, object Data1, object Data2, object Data3, object Data4, object Data5)
        {
            this.Command_Code = Command_Code;
            this.Data1 = Data1;
            this.Data2 = Data2;
            this.Data3 = Data3;
            this.Data4 = Data4;
            this.Data5 = Data5;
        }
        public Command(CommandCode Command_Code, int Value_Int)
        {
            this.Command_Code = Command_Code;
            this.Value_Int = Value_Int;
        }
        public Command(CommandCode Command_Code, float Value_Float)
        {
            this.Command_Code = Command_Code;
            this.Value_Float = Value_Float;
        }
        public Command(CommandCode Command_Code, string Message)
        {
            this.Command_Code = Command_Code;
            this.Message = Message;
        }
        public Command(CommandCode Command_Code, DateTime Value_Datetime)
        {
            this.Command_Code = Command_Code;
            this.Value_Datetime = Value_Datetime;
        }
        public Command(byte[] bytes)
        {
            BinaryFormatter bformatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(bytes);
            Command temp = (Command)bformatter.Deserialize(stream);

            this.Command_Code = temp.Command_Code;
            this.Data1 = temp.Data1;
            this.Data2 = temp.Data2;
            this.Data3 = temp.Data3;
            this.Data4 = temp.Data4;
            this.Data5 = temp.Data5;
            this.Message = temp.Message;
            this.Value_Int = temp.Value_Int;
            this.Value_Float = temp.Value_Float;
            this.Value_Datetime = temp.Value_Datetime;
        }
        #endregion

        public byte[] Serialize()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Serialize(stream, this);
            return stream.ToArray();
        }

        public void Deserialize(byte[] bytes)
        {
            BinaryFormatter bformatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(bytes);
            Command temp = (Command)bformatter.Deserialize(stream);

            this.Command_Code = temp.Command_Code;
            this.Data1 = temp.Data1;
            this.Data2 = temp.Data2;
            this.Data3 = temp.Data3;
            this.Data4 = temp.Data4;
            this.Data5 = temp.Data5;
            this.Message = temp.Message;
            this.Value_Int = temp.Value_Int;
            this.Value_Float = temp.Value_Float;
            this.Value_Datetime = temp.Value_Datetime;
        }

        public void SendData(String ip, int port)
        {
            NetworkStream networkStream;
            TcpClient tcpClient;
            try
            {
                byte[] data = this.Serialize();
                tcpClient = new TcpClient(ip, port);
                networkStream = tcpClient.GetStream();
                networkStream.Write(data, 0, data.Length);
                tcpClient.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //MessageBox.Show(ex.Message, "Can not send info");
            }
        }
    }
}
