using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class JoinGame : MonoBehaviour
{

    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    private NetworkManager networkManager;
    string filterName = "";

    string inputPassword = "";

    void Start()
    {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList()
    {
        ClearRoomList();
        Debug.Log(filterName);
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }

        networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
        status.text = "Searching...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        status.text = "";
       
        if (!success || matchList == null)
        {
            status.text = "Couldn't get room list.";
            return;
        }

        if (filterName != "")
        {
            foreach (MatchInfoSnapshot match in matchList)
            {
                if (match.name == filterName)
                {
                    GameObject _roomListItemGO = Instantiate(roomListItemPrefab);
                    _roomListItemGO.transform.SetParent(roomListParent);

                    RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();
                    if (_roomListItem != null)
                    {
                        _roomListItem.Setup(match, JoinRoom);
                    }


                    // as well as setting up a callback function that will join the game.

                    roomList.Add(_roomListItemGO);
                }
            }
            filterName = "";
        }
        else
        {
            foreach (MatchInfoSnapshot match in matchList)
            {
                GameObject _roomListItemGO = Instantiate(roomListItemPrefab);
                _roomListItemGO.transform.SetParent(roomListParent);

                RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();
                if (_roomListItem != null)
                {
                    _roomListItem.Setup(match, JoinRoom);
                }


                // as well as setting up a callback function that will join the game.

                roomList.Add(_roomListItemGO);
            }
        }


        if (roomList.Count == 0)
        {
            status.text = "No rooms at the moment.";
        }
    }

    void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

    MatchInfoSnapshot matchPlaceHolder;

    public void JoinRoom(MatchInfoSnapshot _match)
    {
        if (_match.isPrivate)
        {
            GameObject.FindGameObjectWithTag("GameSpawner").GetComponent<ButtonSpawner>().ActivateAuthenticationUI();
            matchPlaceHolder = _match;
        }
        else
        {
            networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
            GameObject.Find("Menus").SetActive(false);
        }
    }

    public void InputPassword(string pass)
    {
        inputPassword = pass;
    }

    public void VerifyPassword()
    {
        networkManager.matchMaker.JoinMatch(matchPlaceHolder.networkId, inputPassword, "", "", 0, 0, networkManager.OnMatchJoined);
        GameObject.Find("Menus").SetActive(false);
    }

    public void SearchRoom()
    {
        ClearRoomList();
        filterName = GameObject.Find("SearchInput").GetComponent<InputField>().text;
        RefreshRoomList();
    }
}