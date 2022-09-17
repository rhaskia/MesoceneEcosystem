using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

//List item in room menu of player
public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text, cText;
    Photon.Realtime.Player player;

    public void SetUp(Photon.Realtime.Player _player)
    {
        text.text = _player.NickName;
        player = _player;

        StartCoroutine(UpdateInfo());
    }

    IEnumerator UpdateInfo()
    {
        while (PhotonNetwork.IsConnected)
        {
            int creature = (int)player.CustomProperties["Creature"];

            if (creature != -1) cText.text = RoomManager.Instance.creatures[creature].name;
            else cText.text = "Not Chosen";

            yield return new WaitForSeconds(0.5f);
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (player == otherPlayer) Destroy(gameObject);
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
