using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class WallManager : MonoBehaviour
{
    //Simple Walls
    public List<GameObject> SimpleRedWalls = new List<GameObject>();
    public List<GameObject> SimpleBlueWalls = new List<GameObject>();

    //Cross Walls
    public List<GameObject> CrossRedWalls = new List<GameObject>();
    public List<GameObject> CrossBlueWalls = new List<GameObject>();
    
    [Header("EndWalls")]
    public GameObject RedEndWall;
    public GameObject BlueEndWall;
    
    [Header("EndFlames")]
    public GameObject RedEndFlame;
    public GameObject BlueEndFlame;
    
    [Header("EndFloor")]
    public GameObject End;
    
    [Header("Panels")]
    public GameObject EndPanel;
    public GameObject StopPanel;
    public GameObject DeadPanel;

    [HideInInspector]
    public bool RedOnEndPlatform = false;
    [HideInInspector]
    public bool BlueOnEndPlatform = false;

    private bool RedEnd = false;
    private bool BlueEnd = false;
    private bool BothEnds = false;

    PhotonView pV;

    public static WallManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        EndPanel.SetActive(false);
        StopPanel.SetActive(false);
        DeadPanel.SetActive(false); 
        Time.timeScale = 0;

        pV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (RedEnd == true && BlueEnd == true)
        {
            if(BothEnds == false)
            {
                BothEnds = true;
                EndFloor();
            }
        }

        if (RedOnEndPlatform == true && BlueOnEndPlatform == true)
        {
            Ending();
        }
    }

    public void ChangeSimpleRedWalls()
    {
        foreach (GameObject G in SimpleRedWalls)
        {
            G.GetComponent<WallsMove>().StartCoroutine(G.GetComponent<WallsMove>().MoveWall());
        }
    }

    public void ChangeSimpleBlueWalls()
    {
        foreach (GameObject G in SimpleBlueWalls)
        {
            G.GetComponent<WallsMove>().StartCoroutine(G.GetComponent<WallsMove>().MoveWall());
        }
    }

    public void ChangeCrossRedWalls()
    {
        foreach (GameObject G in CrossRedWalls)
        {
            G.GetComponent<WallsRotate>().StartCoroutine(G.GetComponent<WallsRotate>().RotateWall(Vector3.up, 90));
        }
    }
    
    public void ChangeCrossBlueWalls()
    {
        foreach (GameObject G in CrossBlueWalls)
        {
            G.GetComponent<WallsRotate>().StartCoroutine(G.GetComponent<WallsRotate>().RotateWall(Vector3.up, 90));
        }
    }

    public void ChangeRedEndWall()
    {
        if (RedEnd == false)
        {
            RedEndWall.transform.GetComponent<WallsMove>().StartCoroutine(RedEndWall.transform.GetComponent<WallsMove>().MoveWall());
            RedEnd = true;
        }
    }

    public void ChangeBlueEndWall()
    {
        if (BlueEnd == false)
        {
            BlueEndWall.transform.GetComponent<WallsMove>().StartCoroutine(BlueEndWall.transform.GetComponent<WallsMove>().MoveWall());
            BlueEnd = true;
        }
    }

    public void EndFloor()
    {
        End.transform.GetComponent<WallsMove>().StartCoroutine(End.transform.GetComponent<WallsMove>().MoveWall());
    }

    public void Ending()
    {
        pV.RPC("ActivateEnding", RpcTarget.AllViaServer);
    }

    [PunRPC]
    void ActivateEnding()
    {
        EndPanel.SetActive(true);
    }

    

    public void StartGame()
    {
        Time.timeScale = 1;
    }

    public void StopGame()
    {
        StopPanel.SetActive(true);
       // Time.timeScale = 0;
    }

    public void Dead()
    {
        DeadPanel.SetActive(true);
        //Time.timeScale = 0;
    }

    public void GoToMainMenu()
    {
        pV.RPC("MainMenu", RpcTarget.AllViaServer);
    }

    [PunRPC]
    void MainMenu()
    {
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.InRoom && PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToNextLvl()
    {
        pV.RPC("NextLevel", RpcTarget.AllViaServer);
    }

    [PunRPC]
    void NextLevel()
    {
        switch(SceneManager.GetActiveScene().name)
        {
            case "Tutorial":
                PhotonNetwork.LoadLevel("Lvl1");
                break;

            case "Level1":
                PhotonNetwork.LoadLevel("Lvl2");
                break;

            case "Level2":
                PhotonNetwork.LoadLevel("Lvl3");
                break;
        }
    }
}