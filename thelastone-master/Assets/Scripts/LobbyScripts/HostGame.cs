using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour
{

    [SerializeField]
    private uint roomSize = 2;

    private string roomName;
    string roomPassword = "";

    private NetworkManager networkManager;

    public Text WaitingText;
    public GameObject disconnectButton, backButton;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName(string _name)
    {
        roomName = _name;
    }

    public void SetRoomPassword(string _pass)
    {
        roomPassword = _pass;
    }

    public void CreateRoom()
    {
        if (roomName != "" && roomName != null)
        {
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, roomPassword, "", "", 0, 0, networkManager.OnMatchCreate);
            GameObject.Find("Menus").SetActive(false);
            backButton.SetActive(false);
            disconnectButton.SetActive(true);
            WaitingText.text = "Waiting for an opponent...";
        }
    }

    public void ReactivateMenu()
    {
        WaitingText.text = "Disconnected!";
        Application.LoadLevel("Game");
    }
}