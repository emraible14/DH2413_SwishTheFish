using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;


public class TableManager : MonoBehaviour
{
    private EventManager eventManager;
    private UdpClient client;

    // We will be listening to the Surface on localhost
    private IPEndPoint remoteEndpoint;
    private Thread listenerThread;

    private object queueLock = new object();

    private void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
    }

    void Listen()
    {
        Debug.Log("UDP Listener started...");
        while (true)
        {
            try
            {
                byte[] receivedBytes = client.Receive(ref remoteEndpoint);

                if (receivedBytes.Length > 0)
                {

                    // if in here means screen interacted with so we take action
                    Debug.Log("TableManager");
                    EventManager.Dispatch(EventManager.Event.SurfaceTouched, receivedBytes);

                }

            }
            catch (Exception error)
            {
                Debug.LogError(error.ToString());
            }
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        remoteEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3333);
        client = new UdpClient(remoteEndpoint);

        // Run the listener on a separate thread...
        ThreadStart threadStarter = new ThreadStart(Listen);
        listenerThread = new Thread(threadStarter);
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    void OnApplicationQuit()
    {
        Debug.Log("CLosing COnnection");
        listenerThread.Abort();
        client.Close();
    }
}
