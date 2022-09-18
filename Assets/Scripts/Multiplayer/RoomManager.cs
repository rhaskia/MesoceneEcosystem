using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

//Manages a room
public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    public Creature.Creature[] creatures;
    public float rotation;

    public ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    public bool disconnected;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;

        customProperties["Creature"] = -1;
        PhotonNetwork.SetPlayerCustomProperties(customProperties);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 0 && disconnected)
        {
            MenuManager.Instance.OpenMenu("Error");
            FindObjectOfType<Launcher>().SetErrorText("You have disconnected from the game");
            disconnected = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        }
    }
}
