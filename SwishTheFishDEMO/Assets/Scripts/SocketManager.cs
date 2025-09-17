using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;

public class SocketManager : MonoBehaviour
{
  private WebSocket _websocket;

  public static string IPAddress = "192.168.0.194:3001";

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
    Debug.Log("Attempting to connect to: " + IPAddress);
    await _websocket.Connect();
  }

  private async void CloseSocket(object data)
  {
    await _websocket.Close();
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

  // Start is called before the first frame update
  private async void Start()
  {
    _websocket = new WebSocket("ws://" + IPAddress);

    _websocket.OnOpen += OnSocketConnect;

    _websocket.OnError += (e) =>
    {
      Debug.Log("Error! " + e);
    };

    _websocket.OnClose += OnSocketClose;

    _websocket.OnMessage += (bytes) =>
    {
      Debug.Log("OnMessage!");
      Debug.Log(bytes);

      // getting the message as a string
      var message = System.Text.Encoding.UTF8.GetString(bytes);
      var messageObject = JsonUtility.FromJson<SocketMessage>(message);
      if (messageObject.type == MessageTypes.FishAdded)
      {
        EventManager.Dispatch(new CustomEvent(EventManager.EventType.AddFish, messageObject.data));
      }
      Debug.Log("OnMessage! " + message);
    };

    // Keep sending messages at every 0.3s
    // InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

    // waiting for messages
  }

  private void Update()
  {
    #if !UNITY_WEBGL || UNITY_EDITOR
      if (_websocket.State == WebSocketState.Open)
      {
        // Sending bytes
        // websocket.Send(bytes);
      }
      _websocket.DispatchMessageQueue();
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
    await _websocket.Close();
  }

}