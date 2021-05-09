using UnityEngine;
using System.Collections;

public class roomManager : Photon.MonoBehaviour
{
    public string versionNum = "0.1";
    public string roomName = "Enter room number...";
    public GameObject playerPrefab;
    public string playerName = "Player1";
    public bool isConnected = false;
    public bool isInRoom = false;
    public int kD;

    [SerializeField]
    private byte maxPlayers = 4;

    void Awake()
    {
        PhotonNetwork.automaticallySyncScene = true;
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("CONNECTED!");

        PhotonNetwork.JoinLobby();
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random room");


    }
    public void Start()
    {
        roomName = "Room " + Random.Range(0, 999);
        playerName = "Player " + Random.Range(0, 999);
        if (playerPrefab == null)
        {
            Debug.LogError("missing playerPrefab reference. Please set it up in roomManager");
        }
        else
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
        }
        PhotonNetwork.ConnectUsingSettings(versionNum);
        isConnected = true;
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("kills") >= 1)
        {
            kD = PlayerPrefs.GetInt("kills") / PlayerPrefs.GetInt("deaths");
        }
        else
        {
            kD = 0;
        }
        PhotonNetwork.player.SetScore(PlayerPrefs.GetInt("kills"));
    }

    public void OnJoinedRoom()
    {
        Debug.Log("Now this client is in a room");
        PhotonNetwork.playerName = playerName;


        isConnected = false;
        isInRoom = true;
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        var spawnPosition = new Vector3(Random.Range(-38, 70), 15, Random.Range(95, 300));
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity, 0);
        playerPrefab.GetComponent<RigidBodyFPSWalker>().enabled = true;
        playerPrefab.GetComponent<RigidBodyFPSWalker>().FPSCam.SetActive(true);
    }

    void OnGUI()
    {

        if (isConnected)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 250, Screen.height / 2 - 250, 500, 500));
            playerName = GUILayout.TextField(playerName);
            roomName = GUILayout.TextField(roomName);

            if (GUILayout.Button("Create"))
            {
                PhotonNetwork.JoinOrCreateRoom(roomName, null, null);
            }

            foreach (RoomInfo game in PhotonNetwork.GetRoomList())
            {
                if (GUILayout.Button(game.Name + " " + game.PlayerCount + "/" + game.MaxPlayers))
                {
                    PhotonNetwork.JoinOrCreateRoom(game.Name, null, null);
                }
            }
            GUILayout.EndArea();
        }
    }
}
