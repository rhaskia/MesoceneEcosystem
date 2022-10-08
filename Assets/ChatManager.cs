using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Photon.Pun;
using System;
using System.Linq;

public class ChatManager : MonoBehaviour
{
    public int maxLength;
    public TMP_InputField textInput;
    public TextMeshProUGUI messages;
    public Player.PlayerManager plr;
    string playerType;
    PhotonView pv;

    [PunRPC]
    void SendMessagePUN(string message)
    {
        ManageMessage(message);
    }

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameStateManager.Instance.SetState(true);
            textInput.Select();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (textInput.text.Length == 0 || textInput.text.Length > maxLength) return;

            if (IsCommand(textInput.text)) return;

            string nameColor = "#83FFD4";

            switch (RoomManager.Instance.customProperties["Permissions"])
            {
                case "Developer": nameColor = "#4C83FF"; break;
                case "Admin": nameColor = "#FFFF84"; break;
                case "Paleotuber": nameColor = "#B085FF"; break;
            }

            string message = string.Format("<color={0}>{1}<color=white>: <noparse>{2}</noparse>",
                nameColor, PhotonNetwork.LocalPlayer.NickName, textInput.text);

            pv.RPC("SendMessagePUN", RpcTarget.All, message);

            textInput.text = "";
        }

        if (GameStateManager.Instance.paused && Input.GetMouseButtonDown(0) &&
            !plr.menuOpen) GameStateManager.Instance.SetState(false);

    }

    void ManageMessage(string message)
    {
        string time = System.DateTime.Now.Hour.ToString("00") + ":" + System.DateTime.Now.Minute.ToString("00");

        messages.text = messages.text + "\n[" + time + "] " + message;
    }

    bool IsMouseOverUI() { return EventSystem.current.IsPointerOverGameObject(); }

    public void JoinMessage(Photon.Realtime.Player newPlayer)
    {
        string time = System.DateTime.Now.Hour.ToString("00") + ":" + System.DateTime.Now.Minute.ToString("00");

        string nameColor = "#83FFD4";

        switch (newPlayer.CustomProperties["Permissions"])
        {
            case "Developer": nameColor = "#4C83FF"; break;
            case "Admin": nameColor = "#FFFF84"; break;
            case "Paleotuber": nameColor = "#B085FF"; break;
        }

        string message = "<color=" + nameColor + ">" + newPlayer.NickName + "<color=white> joined the server";
        pv.RPC("SendMessagePUN", RpcTarget.All, message);
    }

    public bool IsCommand(string str)
    {
        if (str.Substring(0, 1) != "/") return false;

        if (str.Substring(1, 3).ToLower() == "ban")
        {
            string name = str.Substring(4).Replace(" ", "");

            if (GetPlayer(name) != null) pv.RPC("KickPlayer", GetPlayer(name));
        }

        if (str.Substring(1, 4).ToLower() == "kick")
        {
            string name = str.Substring(5).Replace(" ", "");

            if (GetPlayer(name) != null) pv.RPC("KickPlayer", GetPlayer(name));
        }

        return false;
    }

    public Photon.Realtime.Player GetPlayer(string name)
    {
        foreach (var player in (from kvp in PhotonNetwork.CurrentRoom.Players select kvp.Value).Distinct())
        {
            if (player.NickName.ToLower() == name.ToLower()) return player;
        }

        return null;
    }

    [PunRPC]
    private void KickPlayer()
    {
        PhotonNetwork.LeaveRoom(); // load lobby scene, returns to master server
    }
}
