using Frictionless;
using NativeWebSocket;
using SolPlay.Scripts.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SocketStatusWidget : MonoBehaviour
{
    public TextMeshProUGUI StatusText;
    public Button ReconnectButton;

    private void Awake()
    {
        ReconnectButton.onClick.AddListener(OnReconnectClicked);
    }

    private void OnReconnectClicked()
    {
        var socketService = ServiceFactory.Resolve<SolPlayWebSocketService>();
        StartCoroutine(socketService.Reconnect());
    }

    void Update()
    {
        var socketService = ServiceFactory.Resolve<SolPlayWebSocketService>();
        if (socketService != null)
        {
            StatusText.text = "Socket: " + socketService.GetState().ToString();
            ReconnectButton.gameObject.SetActive(socketService.GetState() == WebSocketState.Closed);
        }
    }
}
