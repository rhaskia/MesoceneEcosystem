using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ExitGames.Client.Photon;
using Photon.Pun;

public class ChatManager : MonoBehaviour
{
    public int maxLength;
    public TMP_InputField textInput;
    public TextMeshProUGUI messages;
    string playerType;
    PhotonView pv;

    [PunRPC]
    public void SendMessagePUN(string message)
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

        if (GameStateManager.Instance.paused && Input.GetMouseButtonDown(0)) GameStateManager.Instance.SetState(false);

    }

    void ManageMessage(string message)
    {
        string time = System.DateTime.Now.Hour.ToString("00") + ":" + System.DateTime.Now.Minute.ToString("00");

        messages.text = messages.text + "\n[" + time + "]" + message;
    }
}
