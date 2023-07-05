using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class LogicaJugador : MonoBehaviour
{
    public MonoBehaviour[] codigosQueIgnorar;
    private PhotonView photonView;

    //public MeshRenderer flameRender;
    //public MeshRenderer bodyRender;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if(!photonView.IsMine)
        {
            foreach (var codigo in codigosQueIgnorar)
            {
                codigo.enabled = false;
            }
        }
    }

}
