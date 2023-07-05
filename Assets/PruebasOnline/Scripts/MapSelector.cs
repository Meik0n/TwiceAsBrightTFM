using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MapSelector : MonoBehaviour
{


    public void PlayMapSelected(string mapName)
    {
        PhotonNetwork.LoadLevel(mapName);
    }
}
