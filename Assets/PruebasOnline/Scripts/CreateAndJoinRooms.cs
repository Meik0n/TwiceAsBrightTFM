using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public GameObject homePanel;
    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public TextMeshProUGUI roomName;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObject;

    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;

    public List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    public GameObject panelMapSelector;
    public GameObject chooseMapButton;

    //public List<int> playersAvatar = new List<int>();
    int player1Avatar;
    int player2Avatar;
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        

        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }

        homePanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
        panelMapSelector.SetActive(false);
        Screen.fullScreen = true;
    }

    public void OpenPanelLobby()
    {
        homePanel.SetActive(false);
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al servidor");
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
            if(PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
        
        

    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text, new Photon.Realtime.RoomOptions() { MaxPlayers = 2, BroadcastPropsChangeToAll = true } );
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }


    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        UpdateplayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
        
    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach(RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in list)
        {
           RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateplayerList();
    }

    [PunRPC]
    void UpdateplayerList()
    {
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            playerItemsList.Add(newPlayerItem);

            if(player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyLocalChanges();
            }
            else
            {
                newPlayerItem.rightArrowButton.SetActive(false);
                newPlayerItem.leftArrowButton.SetActive(false);
            }


        }

        
    }

    public void CheckPlayersSkin(Player newPlayer)
    {
        if(newPlayer.CustomProperties["playerAvatar"]!= null)
        {
            if(newPlayer.IsMasterClient)
            {
                player1Avatar = (int)newPlayer.CustomProperties["playerAvatar"];
            }
            else
            {
                player2Avatar = (int)newPlayer.CustomProperties["playerAvatar"];
            }
        }
       
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateplayerList();
    }

    [PunRPC]
    void DisableChangePlayer()
    {
        foreach (PlayerItem player in playerItemsList)
        {
            player.rightArrowButton.SetActive(false);
            player.leftArrowButton.SetActive(false);
        }
    }
    private void Update()
    {
        
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {

            chooseMapButton.SetActive(true);
            chooseMapButton.GetComponent<Button>().enabled = false;
            if(player1Avatar != player2Avatar)
            {
                chooseMapButton.GetComponent<Button>().enabled = true;
            }
            else
            {
                print("los jugadores no pueden ser iguales");
            }
        }
        else
        {
            chooseMapButton.SetActive(false);
        }
    }

    public void OnClickChooseMapButton()
    {
        //photonView.RPC("DisableChangePlayer", RpcTarget.AllViaServer);
        panelMapSelector.SetActive(true);
        roomPanel.SetActive(false);
        
    }

    public void BackToRoom()
    {
        panelMapSelector.SetActive(false);
        roomPanel.SetActive(true);
    }

    public void BackToHome()
    {
        lobbyPanel.SetActive(false);
        homePanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
