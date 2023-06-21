using QFSW.QC;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Net.Sockets;
using Unity.Multiplayer.Tools.NetStatsMonitor;

public static class ClipboardUtility
{
    public static void CopyToClipboard(string s)
    {
        GUIUtility.systemCopyBuffer = s;
    }
}

[CommandPrefix("multiplayer.")]
public class MultiplayerSetup : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;
    [SerializeField] RuntimeNetStatsMonitor netStats;
    [SerializeField, Command("network-stats", "Toggles the Network Stats Monitior")] bool netStatsState;
    public string hostIp = "0.0.0.0";
    [SerializeField] ushort port = 23000;

    private void Update()
    {
        switch (netStatsState)
        {
            case true:
                netStats.Visible = true;
                break;
            case false:
                netStats.Visible = false;
                break;
        }
    }

    [Command("apply-ip", "Applies provided IP address to the multiplayer settings. Leave ip blank for this device's IP address. Enter host's IP address to connect to the host's server.")]
    public void ApplyIp(string ip = "0.0.0.0")
    {
        if (ip == "0.0.0.0") ip = LocalIPAddress();

        UnityTransport ut = networkManager.GetComponent<UnityTransport>();
        ut.ConnectionData.Address = ip;
        ut.ConnectionData.Port = port;
    }

    [Command("get-ipv4-address", "Gets this device's IPv4 address and copies it to your clipboard")]
    public string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "0.0.0.0";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        ClipboardUtility.CopyToClipboard(localIP);
        Debug.Log("Copied IP to the clipboard.");
        return localIP;
    }

    [Command("start-host", "Starts up a Host. Acts as both a Server and a Client. Automatically uses this device's IPv4 address which cannot be changed.")]
    void StartHost()
    {
        ApplyIp();
        NetworkManager.Singleton.StartHost();
    }
    [Command("start-client", "Starts up a Client that will connect to a Server with the matching IP. You can get the IP of the host by pressing F1 on the host's version of the game and type \'multiplayer.get-ipv4-address\'")]
    void StartClient(string ip = "127.0.0.1")
    {
        ApplyIp(ip);
        NetworkManager.Singleton.StartClient();
    }
    [Command("start-server", "Starts a Server. Automatically uses this device's IPv4 address which cannot be changed.")]
    void StartServer()
    {
        ApplyIp();
        NetworkManager.Singleton.StartServer();
    }
    [Command("shutdown", "Stops a running Host, Client, or Server. Only use this if multiplayer.disconnect doesn't work.")]
    void Shutdown()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
