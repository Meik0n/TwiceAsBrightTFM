using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BluePlayer : MonoBehaviour
{
    PhotonView photonView;

    [HideInInspector]
    public bool flagSimple = true;
    [HideInInspector]
    public bool flagCross = true;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        SimpleWalls();
        CrossWalls();
        StopGame();
    }

    private void SimpleWalls()
    {
        if (Input.GetKeyDown(KeyCode.H) && flagSimple)
        {
            photonView.RPC("ActivateSimpleWalls", RpcTarget.AllViaServer);
            StartCoroutine(ContadorSimple());
        }
    }

    [PunRPC]
    void ActivateSimpleWalls()
    {
        WallManager.Instance.ChangeSimpleBlueWalls();
    }

    private void CrossWalls()
    {
        if (Input.GetKeyDown(KeyCode.N) && flagCross)
        {
            photonView.RPC("ActivateCrossWalls", RpcTarget.AllViaServer);
            StartCoroutine(ContadorCross());
        }
    }

    IEnumerator ContadorSimple()
    {
        flagSimple = false;
        yield return new WaitForSeconds(1.5f);
        flagSimple = true;
    }
    
    IEnumerator ContadorCross()
    {
        flagCross = false;
        yield return new WaitForSeconds(1.5f);
        flagCross = true;
    }


    [PunRPC]
    void ActivateCrossWalls()
    {
        WallManager.Instance.ChangeCrossBlueWalls();
    }

    private void StopGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            photonView.RPC("ActivateStopGame", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    void ActivateStopGame()
    {
        WallManager.Instance.StopGame();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "BlueEnd":
                photonView.RPC("ActivateChangeBlueEndWall", RpcTarget.AllViaServer);
                Destroy(WallManager.Instance.BlueEndFlame);
                break;
            case "Ending":
                WallManager.Instance.BlueOnEndPlatform = true;
                break;
            case "Enemy":
                photonView.RPC("ActivateDead", RpcTarget.AllViaServer);
                break;
        }
    }

    [PunRPC]
    void ActivateChangeBlueEndWall()
    {
        WallManager.Instance.ChangeBlueEndWall();
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Ending":
                WallManager.Instance.BlueOnEndPlatform = false;
                break;
        }
    }

    [PunRPC]
    void ActivateDead()
    {
        WallManager.Instance.Dead();
    }
}
