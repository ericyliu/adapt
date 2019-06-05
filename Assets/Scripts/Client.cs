using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
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

    Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    try
    {
      client.Connect(remoteEp);
      Debug.Log("Client Sending: 1000");
      byte[] msg = BitConverter.GetBytes(1000);
      client.Send(msg);
      client.Receive(bytes);
      Debug.Log("Client Received: " + BitConverter.ToInt32(bytes, 0));
      Debug.Log("Client Sending: 9999");
      msg = BitConverter.GetBytes(9999);
      client.Send(msg);
      client.Receive(bytes);
      Debug.Log("Client Received: " + BitConverter.ToInt32(bytes, 0));
      client.Shutdown(SocketShutdown.Both);
      client.Close();
    }
    catch (Exception e)
    {
      if (e.Message == "No connection could be made because the target machine actively refused it.")
      {
        Debug.Log("Reconnecting in 1s");
        StartCoroutine(this.Connect());
      }
      Debug.Log("Exception: " + e.ToString());
    }
  }
}
