using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetServer
{
  Local,
  Server,
}

public class Game : MonoBehaviour
{
  public TargetServer targetServer = TargetServer.Local;

  // Start is called before the first frame update
  void Start()
  {
    if (targetServer == TargetServer.Local) (new Server()).StartServer();
  }

  // Update is called once per frame
  void Update()
  {

  }
}
