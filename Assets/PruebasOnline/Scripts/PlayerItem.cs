using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviourPunCallbacks
{

    public TextMeshProUGUI playerName;

    //public Image backgroundImage;
    //public Color highlightColor;

    public GameObject leftArrowButton;
    public GameObject rightArrowButton;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    public Image playerAvatar;
    public Sprite[] avatars;
    public AnimationClip clips;
    public Player player;

    CreateAndJoinRooms manager;

    private void Awake()
    {
        manager = FindObjectOfType<CreateAndJoinRooms>();
    }

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
        if (PhotonNetwork.IsMasterClient)
        {
            playerProperties["playerAvatar"] = 0;
        }
        else
        {
            playerProperties["playerAvatar"] = 1;
        }

        PhotonNetwork.SetPlayerCustomProperties(playerProperties);

    }

    public void ApplyLocalChanges()
    {
        //backgroundImage.color = highlightColor;
        leftArrowButton.SetActive(false);
        rightArrowButton.SetActive(false);
    }
   
    public void OnClickLeftArrow()
    {
        if((int)playerProperties["playerAvatar"] == 0)
        {
            playerProperties["playerAvatar"] = avatars.Length + 1;

            
        }
        else
        {
            playerProperties["playerAvatar"] = avatars.Length + 1;
        }
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
       
    }

    public void OnClickRightArrow()
    {
        if ((int)playerProperties["playerAvatar"] == 1)
        {
            playerProperties["playerAvatar"] = 0;

        }
        else
        {
            playerProperties["playerAvatar"] = (int)playerProperties["playerAvatar"] + 1;

        }
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }
    }

    void UpdatePlayerItem(Player player)
    {
        if(player.CustomProperties.ContainsKey("playerAvatar"))
        {
            playerAvatar.sprite = avatars[(int)player.CustomProperties["playerAvatar"]];
            playerProperties["playerAvatar"] = (int)player.CustomProperties["playerAvatar"];
        }
        else
        {
            playerProperties["playerAvatar"] = 0;
        }


        manager.CheckPlayersSkin(player);
    }
}
