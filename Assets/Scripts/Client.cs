using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
  public PlayerController playerController;
  public Game game;
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

    IPHostEntry host = Dns.GetHostEntry(game.targetServer == TargetServer.Local ? "localhost" : "104.196.10.129");
    IPAddress ipAddress = host.AddressList[0];
    IPEndPoint remoteEp = new IPEndPoint(ipAddress, 11000);

    client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    try
    {
      Debug.Log("Connecting to server...");
      client.Connect(remoteEp);
      Debug.Log("Successfully connected to server");
      StartCoroutine(this.SendClientControlState());
      StartCoroutine(this.ReceiveCharacterState());
    }
    catch (Exception e)
    {
      if (e.Message.IndexOf("No connection could be made because the target machine actively refused it.") > -1)
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
    yield return new WaitForSeconds(.01f);
    StartCoroutine(SendClientControlState());
  }

  IEnumerator ReceiveCharacterState()
  {
    while (true)
    {
      if (this.client == null)
      {
        break;
      }
      byte[] bytes = new byte[1024];
      this.client.Receive(bytes);
      ServerCharacter character = new ServerCharacter(bytes);
      this.playerController.UpdateFromServer(character);
      yield return new WaitForSeconds(.01f);
    }
  }
}