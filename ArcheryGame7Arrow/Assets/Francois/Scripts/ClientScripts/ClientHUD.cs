using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using Mirror;
using UnityEngine.SceneManagement;

public class ClientHUD : MonoBehaviour
{

    public GameObject connectToServer, disConnect, addressPanel;
    public UIInput NguiportText, NguiipText, NguipasswordText;

    private NetworkManager manager;
    private TelepathyTransport _telepathyTransport;
    private float connectingTimer, connectionFaileTimer;
    private bool connected;

    // Use this for initialization
    void Start()
    {
        if (!manager)
            manager = GetComponent<NetworkManager>();
        if (!_telepathyTransport)
            _telepathyTransport = GetComponent<TelepathyTransport>();

        //checking if we have saved server info.
        if (PlayerPrefs.HasKey("nwPortC"))
        {
            _telepathyTransport.port = (ushort) Convert.ToInt32(PlayerPrefs.GetString("nwPortC"));
            NguiportText.text = PlayerPrefs.GetString("nwPortC");
        }
        if (PlayerPrefs.HasKey("IPAddressC"))
        {
            manager.networkAddress = PlayerPrefs.GetString("IPAddressC");
            NguiipText.text = PlayerPrefs.GetString("IPAddressC");
        }
    }

    void Update()
    {
        if (!connected)
        {
            //shows the failed to connect message after a certain time waiting to connect.
            if (connectingTimer > 0)
                connectingTimer -= Time.deltaTime;
            else
            {
               // manager.StopClient();
                if (connectionFaileTimer > 0)
                    connectionFaileTimer -= Time.deltaTime;
            }
        }
        if (connected)
        {
        }
    }

    public void ConnectToServer()
    {
        if (NguiipText.text != "" && NguiportText.text != "")//is the information filled in ?.
        {
            connected = false;
            connectingTimer = 8;//how long we try to connect until the fail message appears.
            connectionFaileTimer = 2;//how long the fail message is showing.
            manager.networkAddress = NguiipText.text;
            _telepathyTransport.port = (ushort) Convert.ToInt32(NguiportText.text);
            PlayerPrefs.SetString("IPAddressC", NguiipText.text);//saving the filled in ip.
            PlayerPrefs.SetString("nwPortC", NguiportText.text);//saving the filled in port.
            SceneManager.LoadScene(manager.onlineScene);
            manager.StartClient();
           // manager.StartClient();
        }
    }

    //called by the CustomNetworkManager.
    public void ConnectSuccses()
    {
        connected = true;
        if (disConnect != null)
         disConnect.SetActive(true);
        if (connectToServer != null)
          connectToServer.SetActive(false);
        if (addressPanel != null)
          addressPanel.SetActive(false);
        //menuCam.SetActive(false);   //if your player has a camera on him this one should be turned off when entering the game/lobby.
    }

    public void ButtonDisConnect()
    {
        DisConnect(false);
    }

    public void DisConnect(bool showMessage)
    {
        if (connectToServer != null)
            connectToServer.SetActive(true);
        if (disConnect != null)
            disConnect.SetActive(false);
        if (addressPanel != null)
            addressPanel.SetActive(true);
        //menuCam.SetActive(true);  //turn the camera on again when returning to menu scene.
        manager.StopClient();
    }
}
