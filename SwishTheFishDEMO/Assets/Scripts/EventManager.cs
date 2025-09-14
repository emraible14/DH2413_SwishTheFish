using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public enum Event
    {
        AddFish,
        ConnectSocket,
        CloseSocket,
        SocketConnect,
        SocketClose,
        SurfaceTouched,
    }
     
    public delegate void AddFishAction();
    public static event AddFishAction OnFishAdded;

    public delegate void ConnectSocketAction();
    public static event ConnectSocketAction ConnectSocket;

    public delegate void CloseSocketAction();
    public static event CloseSocketAction CloseSocket;
    
    public delegate void SocketConnectAction();
    public static event SocketConnectAction OnSocketConnect;

    public delegate void SocketCloseAction();
    public static event SocketCloseAction OnSocketClosed;

    public delegate void SurfaceTouchedAction(byte[] information);
    public static event SurfaceTouchedAction OnSurfaceTouched;

    public static void Dispatch(Event eventType)
    {
        switch (eventType)
        {
            case Event.AddFish:
                if (OnFishAdded == null) return;
                OnFishAdded();
                break;
            case Event.ConnectSocket:
                if (ConnectSocket == null) return;
                ConnectSocket();
                break;
            case Event.CloseSocket:
                if (CloseSocket == null) return;
                CloseSocket();
                break;
            case Event.SocketConnect:
                if (OnSocketConnect == null) return;
                OnSocketConnect();
                break;
            case Event.SocketClose:
                if (OnSocketClosed == null) return;
                OnSocketClosed();
                break;
            case Event.SurfaceTouched:
                if (OnSurfaceTouched == null) return;
                OnSurfaceTouched(information);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
        }
    }
}
