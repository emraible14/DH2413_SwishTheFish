using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketMessage
{
  public SocketManager.MessageTypes type { get; private set; }
  
  public object data { get; private set; }
}
