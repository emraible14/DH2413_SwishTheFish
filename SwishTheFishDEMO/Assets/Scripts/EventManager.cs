using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public enum EventType
    {
        AddFish,
        ConnectSocket,
        CloseSocket,
        SocketConnect,
        SocketClose,
        SurfaceTouched,
    }
     
    public delegate void AddFishAction(object data);
    public static event AddFishAction OnFishAdded;

    public delegate void ConnectSocketAction(object data);
    public static event ConnectSocketAction ConnectSocket;

    public delegate void CloseSocketAction(object data);
    public static event CloseSocketAction CloseSocket;
    
    public delegate void SocketConnectAction(object data);
    public static event SocketConnectAction OnSocketConnect;

    public delegate void SocketCloseAction(object data);
    public static event SocketCloseAction OnSocketClosed;

    public delegate void SurfaceTouchedAction(object data);
    public static event SurfaceTouchedAction OnSurfaceTouched;

    public static void Dispatch(CustomEvent eEvent)
    {
        switch (eEvent.type)
        {
            case EventType.AddFish:
                if (OnFishAdded == null) return;
                OnFishAdded(eEvent.data);
                break;
            case EventType.ConnectSocket:
                if (ConnectSocket == null) return;
                ConnectSocket(eEvent.data);
                break;
            case EventType.CloseSocket:
                if (CloseSocket == null) return;
                CloseSocket(eEvent.data);
                break;
            case EventType.SocketConnect:
                if (OnSocketConnect == null) return;
                OnSocketConnect(eEvent.data);
                break;
            case EventType.SocketClose:
                if (OnSocketClosed == null) return;
                OnSocketClosed(eEvent.data);
                break;
            case EventType.SurfaceTouched:
                if (OnSurfaceTouched == null) return;
                OnSurfaceTouched(eEvent.data);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(eEvent.type), eEvent.type, null);
        }
    }
}
