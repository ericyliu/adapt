using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Server
{
  Socket handler;

  public void StartServer()
  {
    Debug.Log("Starting the server");
    Thread thread = new Thread(new ThreadStart(this.ListenAndRespond));
    thread.Start();
  }

  void ListenAndRespond()
  {
    IPHostEntry host = Dns.GetHostEntry("localhost");
    IPAddress ipAddress = host.AddressList[0];
    IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
    try
    {
      Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
      listener.Bind(localEndPoint);
      listener.Listen(10);
      Debug.Log("Waiting for a connection...");
      this.handler = listener.Accept();
      Debug.Log("Connection Received");
      byte[] bytes = null;
      while (true)
      {
        bytes = new byte[1024];
        handler.Receive(bytes);
        Debug.Log("Server Recieved: " + BitConverter.ToInt32(bytes, 0));
        handler.Send(bytes);
        if (BitConverter.ToInt32(bytes, 0) == 9999)
        {
          break;
        }
      }
      Debug.Log("Connection closed");
      handler.Shutdown(SocketShutdown.Both);
      handler.Close();
    }
    catch (Exception e)
    {
      Debug.Log(e);
      Console.WriteLine(e);
      if (handler != null)
      {
        handler.Shutdown(SocketShutdown.Both);
        handler.Close();
      }
    }
  }
}
