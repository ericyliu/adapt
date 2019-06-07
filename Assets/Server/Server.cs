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
  ServerCharacter character = new ServerCharacter();


  public void StartServer()
  {
    Debug.Log("Starting the server");
    Thread thread = new Thread(new ThreadStart(this.ReceiveConnection));
    thread.Start();
  }

  void ReceiveConnection()
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
      new Thread(new ThreadStart(this.ListenForClientControlState)).Start();
      new Thread(new ThreadStart(this.PublishCharacterState)).Start();

    }
    catch (Exception e)
    {
      if (handler != null)
      {
        handler.Shutdown(SocketShutdown.Both);
        handler.Close();
      }
    }
  }

  void ListenForClientControlState()
  {
    while (true)
    {
      if (this.handler == null)
      {
        break;
      }
      byte[] bytes = new byte[1024];
      this.handler.Receive(bytes);
      var clientControlState = new ClientControlState(bytes);
      character.HandleMovement(clientControlState);

    }
  }

  void PublishCharacterState()
  {
    while (true)
    {
      if (this.handler == null)
      {
        break;
      }
      this.handler.Send(character.ToByteArray());
      Thread.Sleep(10);
    }
  }
}

public class ServerCharacter
{
  public Vector2 position = Vector2.zero;
  Vector3 playerCameraUp = new Vector3(0.3f, 0.7f, 0.6f);
  Vector3 terrainUp = new Vector3(0f, 1f, 0f);
  DateTime lastUpdate;
  float speed = 3f;

  public ServerCharacter()
  {
    this.position = new Vector2(5f, 5f);
  }

  public ServerCharacter(byte[] raw)
  {
    this.position.x = BitConverter.ToInt16(raw, 0) / 100f;
    this.position.y = BitConverter.ToInt16(raw, 2) / 100f;
  }

  public void HandleMovement(ClientControlState clientControlState)
  {
    if (this.lastUpdate == null)
    {
      this.lastUpdate = DateTime.Now;
      return;
    }
    float timePassed = (DateTime.Now - this.lastUpdate).Milliseconds / 1000f;
    Vector3 forward = Vector3.ProjectOnPlane(this.playerCameraUp, this.terrainUp).normalized * this.speed * timePassed;
    Vector2 direction = Vector2.zero;
    if (clientControlState.moveUp)
    {
      direction += new Vector2(forward.x, forward.z);
    }
    if (clientControlState.moveLeft)
    {
      direction += new Vector2(-forward.z, forward.x);
    }
    if (clientControlState.moveDown)
    {
      direction += new Vector2(-forward.x, -forward.z);
    }
    if (clientControlState.moveRight)
    {
      direction += new Vector2(forward.z, -forward.x);
    }
    this.position += direction;
    this.lastUpdate = DateTime.Now;
  }

  public byte[] ToByteArray()
  {
    byte[] bytes = new byte[4];
    Array.Copy(this.FloatToByte(this.position.x), 0, bytes, 0, 2);
    Array.Copy(this.FloatToByte(this.position.y), 0, bytes, 2, 2);
    return bytes;
  }

  public override string ToString()
  {
    return this.position.ToString();
  }

  byte[] FloatToByte(float f)
  {
    return BitConverter.GetBytes(Convert.ToInt16(f * 100f));
  }

}