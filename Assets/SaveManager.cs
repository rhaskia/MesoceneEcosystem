using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Security.Cryptography;
using System.IO;

[Serializable]
public class Save
{
    public string name;
    public int ID;

    public int creature;
    public Creature.Growth.GS growth;

    public int health;

    public int[] position;

    //public Ailment[] ailments;

    public Save(string _name, int _creature)
    {
        name = _name;
        ID = UnityEngine.Random.Range(0, 10000);

        creature = _creature;
        health = RoomManager.Instance.creatures[_creature].maxHealth;

        growth = Creature.Growth.GS.baby;

        position = new int[3];
    }
}

[Serializable]
public class SaveList
{
    public List<Save> saves;

    public SaveList(List<Save> lis)
    {
        saves = lis;
    }
}


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public List<Save> saves;
    public SaveSlotUI[] slots;
    public GameObject newButton;

    public int chosenSave;
    public int chosenC;

    string path;
    byte[] savedKey;
    FileStream dataStream;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null) Destroy(gameObject);

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        path = Application.persistentDataPath + "/mesocenesavedata.json";

        Load();
        ReloadSaves();

        InvokeRepeating("Save", 60f, 60f);

    }

    void ReloadSaves()
    {
        foreach (var item in slots)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < saves.Count) slots[i].gameObject.SetActive(true);
            slots[i].Reload();
        }

        newButton.SetActive(true);
        if (saves.Count != 8) newButton.transform.position = slots[saves.Count].transform.position;
        else newButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateNewSave()
    {
        saves.Add(new Save("New Save", chosenC));
        ReloadSaves();
        Save();
    }

    public void Save()
    {
        string jsonString = JsonUtility.ToJson(new SaveList(saves));

        File.WriteAllText(path, jsonString);

        UpdateInfo();
    }

    public void Load()
    {
        if (File.Exists(path) && PlayerPrefs.HasKey("key"))
        {
            string data = File.ReadAllText(path);

            saves = JsonUtility.FromJson<SaveList>(data).saves;
            UpdateInfo();
        }
        else
        {
            saves = new List<Save>();
        }
    }

    public void RemoveSave(int a)
    {
        saves.RemoveAt(a);
        ReloadSaves();
        Save();
    }

    void UpdateInfo()
    {
        if (saves.Count == 0) return;

        if (saves.Count != 0) RoomManager.Instance.customProperties["Creature"] = saves[chosenSave].creature;
        PhotonNetwork.SetPlayerCustomProperties(RoomManager.Instance.customProperties);
        PhotonNetwork.NickName = saves[chosenSave].name;
    }


}
