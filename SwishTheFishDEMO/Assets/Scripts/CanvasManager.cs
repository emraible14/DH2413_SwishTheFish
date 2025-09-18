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
    [SerializeField] private TMP_InputField ipInputField;

    private string ipAddressInput = "localhost:3001";
    
    void OnEnable()
    {
        EventManager.OnFishAdded += UpdateText;
        EventManager.OnSocketConnect += UpdateSocketText;
        EventManager.OnSocketClosed += UpdateSocketTextClosed;
        connectButton.onClick.AddListener(ConnectSocket);
        ipInputField.onEndEdit.AddListener(IPAddressEditEnd);

        fishAmountText.text = FindObjectOfType<School>().GetNumFish() + " fish";
    }


    private void OnDisable()
    {
        EventManager.OnFishAdded -= UpdateText;
        EventManager.OnSocketConnect -= UpdateSocketText;
        EventManager.OnSocketClosed -= UpdateSocketTextClosed;
        connectButton.onClick.RemoveListener(ConnectSocket);
        ipInputField.onEndEdit.RemoveListener(IPAddressEditEnd);
    }

    private void Start()
    {
        ipInputField.text = ipAddressInput;
    }

    private void ConnectSocket()
    {
        var eEvent = SocketManager.Connected
            ? new CustomEvent(EventManager.EventType.CloseSocket, null)
            : new CustomEvent(EventManager.EventType.ConnectSocket, ipAddressInput);
        
        EventManager.Dispatch(eEvent);
    }

    void UpdateSocketText(object data)
    {
        socketStatus.text = "Connected";
        connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Disconnect";
    }

    void UpdateSocketTextClosed(object data)
    {
        socketStatus.text = "Not connected";
        connectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Connect";
    }

    void UpdateText(object data)
    {
        fishAmountText.text = boidManager.GetNumBoids() + " fish";
    }

    void IPAddressEditEnd(string ipAddress)
    {
        Debug.Log(ipAddress);
        ipAddressInput = ipAddress;
    }
}
