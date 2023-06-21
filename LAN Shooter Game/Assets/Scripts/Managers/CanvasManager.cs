using UnityEngine;
using Unity.Netcode;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    public MultiplayerSetup multiplayerSetup;
    public TMP_InputField ipInputField;
    
    public void StartHost()
    {
        multiplayerSetup.hostIp = multiplayerSetup.LocalIPAddress();
        multiplayerSetup.ApplyIp();
        NetworkManager.Singleton.StartHost();
    }
    public void StartClient()
    {
        multiplayerSetup.hostIp = ipInputField.text;
        multiplayerSetup.ApplyIp();
        NetworkManager.Singleton.StartClient();
    }
    public void StartServer()
    {
        multiplayerSetup.hostIp = multiplayerSetup.LocalIPAddress();
        multiplayerSetup.ApplyIp();
        NetworkManager.Singleton.StartServer();
    }
}
