using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
  public PlayerController playerController;
  Socket client;

  // Start is called before the first frame update
  void Start()
  {
    StartCoroutine(this.Connect());
  }

  // Update is called once per frame
  void Update()
  {

  }

  IEnumerator Connect()
  {
    yield return new WaitForSeconds(1);
    byte[] bytes = new byte[1024];

    IPHostEntry host = Dns.GetHostEntry("localhost");
    IPAddress ipAddress = host.AddressList[0];
    IPEndPoint remoteEp = new IPEndPoint(ipAddress, 11000);

    client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    try
    {
      client.Connect(remoteEp);
      Debug.Log("Successfully connected to server");
      StartCoroutine(this.SendClientControlState());
    }
    catch (Exception e)
    {
      if (e.Message == "No connection could be made because the target machine actively refused it.")
      {
        Debug.Log("Reconnecting in 1s");
        StartCoroutine(this.Connect());
      }
      Debug.Log("Exception: " + e.ToString());
      if (client != null)
      {
        client.Shutdown(SocketShutdown.Both);
        client.Close();
      }
    }
  }

  IEnumerator SendClientControlState()
  {
    byte[] msg = playerController.clientControlState.toByteArray();
    client.Send(msg);
    yield return new WaitForSeconds(.050F);
    StartCoroutine(SendClientControlState());
  }
}

public class ClientControlState
{
  public bool moveLeft = false;
  public bool moveRight = false;
  public bool moveUp = false;
  public bool moveDown = false;

  public ClientControlState(bool moveLeft = false, bool moveRight = false, bool moveUp = false, bool moveDown = false)
  {
    this.moveLeft = moveLeft;
    this.moveRight = moveRight;
    this.moveUp = moveUp;
    this.moveDown = moveDown;
  }

  public ClientControlState(byte[] raw)
  {
    this.moveLeft = (raw[0] & 1) == 1 ? true : false;
    this.moveRight = ((raw[0] >> 1) & 1) == 1 ? true : false;
    this.moveUp = ((raw[0] >> 2) & 1) == 1 ? true : false;
    this.moveDown = ((raw[0] >> 3) & 1) == 1 ? true : false;
  }

  public byte[] toByteArray()
  {
    var array = new byte[1];
    byte b = 0;
    if (this.moveLeft) b++;
    if (this.moveRight) b += 2;
    if (this.moveUp) b += 4;
    if (this.moveDown) b += 8;
    array[0] = b;
    return array;
  }

  public override string ToString()
  {
    return "[" +
      this.moveLeft + "," +
      this.moveRight + "," +
      this.moveUp + "," +
      this.moveDown +
    "]";
  }
}