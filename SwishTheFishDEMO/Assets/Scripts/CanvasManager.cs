using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private KeyCode showSettingsKey = KeyCode.F;
    [SerializeField] private BoidManager boidManager;

    [SerializeField] private TextMeshProUGUI fishAmountText;
    [SerializeField] private TextMeshProUGUI socketStatus;
    [SerializeField] private Button connectButton;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private Button quitButton;

    private string ipAddressInput = "dh2413-swishthefish.onrender.com";
    private Canvas canvasComponent;
    
    void OnEnable()
    {
        EventManager.OnFishAdded += UpdateText;
        EventManager.OnSocketConnect += UpdateSocketText;
        EventManager.OnSocketClosed += UpdateSocketTextClosed;
        connectButton.onClick.AddListener(ConnectSocket);
        ipInputField.onEndEdit.AddListener(IPAddressEditEnd);

        fishAmountText.text = FindObjectOfType<School>().GetNumFish() + " fish";
        
        quitButton.onClick.AddListener(QuitButtonClicked);
    }


    private void OnDisable()
    {
        EventManager.OnFishAdded -= UpdateText;
        EventManager.OnSocketConnect -= UpdateSocketText;
        EventManager.OnSocketClosed -= UpdateSocketTextClosed;
        connectButton.onClick.RemoveListener(ConnectSocket);
        ipInputField.onEndEdit.RemoveListener(IPAddressEditEnd);
        quitButton.onClick.RemoveListener(QuitButtonClicked);
    }

    private void Start()
    {
        ipInputField.text = ipAddressInput;
        canvasComponent = GetComponent<Canvas>();
    }

    private void ConnectSocket()
    {
        var eEvent = SocketManager.Connected
            ? new CustomEvent(EventManager.EventType.CloseSocket, null)
            : new CustomEvent(EventManager.EventType.ConnectSocket, ipAddressInput);
        
        EventManager.Dispatch(eEvent);
    }

    private void QuitButtonClicked()
    {
        Application.Quit();
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

    private void Update()
    {
        if (Input.GetKeyUp(showSettingsKey))
        {
            if (canvasComponent.enabled)
            {
                canvasComponent.enabled = false;
            }
            else
            {
                canvasComponent.enabled = true;
            }
        }
    }
}
