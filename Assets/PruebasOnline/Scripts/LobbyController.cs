using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobbyController : MonoBehaviourPunCallbacks
{
    string playerName;
    #region prueba1
    public bool blueSelected;
    public bool orangeSelected;

    public GameObject blueButtonNormal;
    public GameObject blueButtonSelected;
    public GameObject orangeButtonNormal;
    public GameObject orangeButtonSelected;

    [PunRPC]
    public void BlueSelected()
    {
        blueSelected = true;
        blueButtonNormal.SetActive(false);
        blueButtonSelected.SetActive(true);
        photonView.RPC(nameof(BlueSelected), RpcTarget.Others);
    }

    [PunRPC]
    public void OrangeSelected()
    {
        orangeSelected = true;
        orangeButtonNormal.SetActive(false);
        orangeButtonSelected.SetActive(true);
        photonView.RPC(nameof(OrangeSelected), RpcTarget.Others);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("EscenaPruebaOnline");
        //LogicaRed logicaRed = FindObjectOfType<LogicaRed>();
    }
    #endregion

    private void Start()
    {
        
    }

    public void SetPlayerInfo(Player _player)
    {
        playerName = _player.NickName;
    }

    public void ApplyLocalChanges()
    {

    }
}
