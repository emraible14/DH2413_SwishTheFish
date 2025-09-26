using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;

public class SocketManager : MonoBehaviour
{
  private WebSocket _websocket;

  public enum MessageTypes
  {
    FishAdded,
    RemoveFish,
  }
  
  public static bool Connected { get; private set; }

  private void OnEnable()
  {
    EventManager.ConnectSocket += ConnectSocket;
    EventManager.CloseSocket += CloseSocket;
  }
  
  private void OnDisable()
  {
    EventManager.ConnectSocket -= ConnectSocket;
    EventManager.CloseSocket -= CloseSocket;
  }

  private async void ConnectSocket(object data)
  {
    var ipAddress = (String)data;
    Debug.Log("Attempting to connect to: " + ipAddress);
    await InitSocket(ipAddress).Connect();
  }

  private async void CloseSocket(object data)
  {
    await _websocket.Close();
    _websocket = null;
  }

  private void OnSocketConnect()
  {
    Debug.Log("Connection open!");
    Connected = true;
    EventManager.Dispatch(new CustomEvent(EventManager.EventType.SocketConnect, null));
    _websocket.SendText("authenticate");

  }

  private void OnSocketClose(WebSocketCloseCode code)
  {
    Debug.Log("Connection closed!");
    Connected = false;
    EventManager.Dispatch(new CustomEvent(EventManager.EventType.SocketClose, null));
  }
  
  private WebSocket InitSocket(string ipAddress)
  {
    _websocket = new WebSocket("ws://" + ipAddress);

    _websocket.OnOpen += OnSocketConnect;

    _websocket.OnError += (e) => { Debug.Log("Error! " + e); };

    _websocket.OnClose += OnSocketClose;

    _websocket.OnMessage += (bytes) =>
    {
      // getting the message as a string
      var message = System.Text.Encoding.UTF8.GetString(bytes);
      Debug.Log("OnMessage! " + message);

      var messageObject = JsonUtility.FromJson<SocketMessage>(message);
      if (messageObject.type == MessageTypes.FishAdded)
      {
        Debug.Log("Data! " + messageObject.data);
        EventManager.Dispatch(new CustomEvent(EventManager.EventType.AddFish, messageObject.data));
      }
    };

    return _websocket;
  }

  private void Update()
  {
    #if !UNITY_WEBGL || UNITY_EDITOR
      if (_websocket != null && _websocket.State == WebSocketState.Open)
      {
        // Sending bytes
        // websocket.Send(bytes);
      _websocket.DispatchMessageQueue();
      }
    #endif
  }

  async void SendWebSocketMessage()
  {
    if (_websocket.State == WebSocketState.Open)
    {
      // Sending bytes
      await _websocket.Send(new byte[] { 10, 20, 30 });

      // Sending plain text
      await _websocket.SendText("plain text message");
    }
  }

  private async void OnApplicationQuit()
  {
    if (_websocket == null) return;
    await _websocket.Close();
  }

}