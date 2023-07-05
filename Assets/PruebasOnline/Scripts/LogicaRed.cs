using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LogicaRed : MonoBehaviourPunCallbacks
{
    public static LogicaRed instancia;

    private void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        RoomJoined();
    }

   

    public void RoomJoined()
    {
        PhotonNetwork.Instantiate("PhotonPlayer", new Vector3(Random.Range(4, -4), 1, Random.Range(-14, -7)), Quaternion.identity);
    }
}
