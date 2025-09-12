using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private BoidManager boidManager;

    [SerializeField] private TextMeshProUGUI fishAmountText;
    [SerializeField] private TextMeshProUGUI socketStatus;
    [SerializeField] private Button connectButton;
    
    void OnEnable()
    {
        EventManager.OnFishAdded += UpdateText;
        EventManager.OnSocketConnect += UpdateSocketText;
        EventManager.OnSocketClosed += UpdateSocketTextClosed;
        connectButton.onClick.AddListener(ConnectSocket);
    }


    private void OnDisable()
    {
        EventManager.OnFishAdded -= UpdateText;
        EventManager.OnSocketConnect -= UpdateSocketText;
        EventManager.OnSocketClosed -= UpdateSocketTextClosed;
    }
    
    private void ConnectSocket()
    {
        EventManager.Dispatch(SocketManager.Connected
            ? EventManager.Event.CloseSocket
            : EventManager.Event.ConnectSocket);
    }

    void UpdateSocketText()
    {
        socketStatus.text = "Connected";
        connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Disconnect";
    }

    void UpdateSocketTextClosed()
    {
        socketStatus.text = "Not connected";
        connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Connect";
    }

    void UpdateText()
    {
        fishAmountText.text = boidManager.GetNumBoids().ToString() + " fishes";
    }
}
