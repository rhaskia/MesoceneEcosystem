using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using DiscordPresence;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Launcher for multiplayer
//Most voids are self explanatory
public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomName;
    [SerializeField] TextMeshProUGUI errorText;
    [SerializeField] TextMeshProUGUI roomNameText;

    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItem;

    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItem;

    [SerializeField] GameObject startGameButton;

    [SerializeField] GameObject noConnection;

    [SerializeField] string[] adject;
    [SerializeField] string[] dinosaurs;

    [SerializeField] CreatureSelector selector;
    [SerializeField] TMP_InputField nickname;
    [SerializeField] Toggle fs;

    //Singleton
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        PhotonNetwork.ConnectUsingSettings();
        PresenceManager.UpdatePresence(detail: "In Menu", largeKey: "mesocene_logo_may_2021", state: "");

        if (PlayerPrefs.GetString("FullScreen") == "") PlayerPrefs.SetString("FullScreen", "True");
        fs.isOn = System.Convert.ToBoolean(PlayerPrefs.GetString("FullScreen"));
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        MenuManager.Instance.OpenMenu("Title");

        if (PlayerPrefs.GetString("Nickname") != "")
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("Nickname");
            nickname.text = PlayerPrefs.GetString("Nickname");
        }
        else PhotonNetwork.NickName = "Player#" + Random.Range(10, 99);

        selector.current = PlayerPrefs.GetInt("Creature") - selector.offset;
    }

    public override void OnLeftLobby()
    {
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void ReloadClient()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomName.text)) return;

        PhotonNetwork.CreateRoom(roomName.text);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("Room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        PresenceManager.UpdatePresence(detail: "In Room: " + PhotonNetwork.CurrentRoom.Name);

        foreach (Transform child in playerListContent)
        {
            Destroy(child);
        }

        foreach (var player in PhotonNetwork.PlayerList)
        {
            Instantiate(playerListItem, playerListContent).GetComponent<PlayerListItem>().SetUp(player);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MenuManager.Instance.OpenMenu("Error");
        errorText.text = "Room Creation Failed: \n" + message;
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("Title");
        PresenceManager.UpdatePresence(detail: "In Menu");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void SaveOptions()
    {
        if (nickname.text != "")
        {
            PlayerPrefs.SetString("Nickname", nickname.text);
            PhotonNetwork.NickName = PlayerPrefs.GetString("Nickname");
        }

        PlayerPrefs.SetInt("Creature", selector.current + selector.offset);

        PlayerPrefs.SetString("FullScreen", fs.isOn.ToString());
        Screen.fullScreen = fs.isOn;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans);
        }

        foreach (var item in roomList)
        {
            if (item.RemovedFromList) continue;
            Instantiate(roomListItem, roomListContent).GetComponent<RoomListItem>().SetUp(item);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Instantiate(playerListItem, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
