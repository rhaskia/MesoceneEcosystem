using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    public int save;

    public TMP_InputField text;


    // Start is called before the first frame update
    void Start()
    {
        Reload();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Reload()
    {
        if (save < SaveManager.Instance.saves.Count) text.text = SaveManager.Instance.saves[save].name;
    }

    public void SaveName(string n)
    {
        SaveManager.Instance.saves[save].name = n;
        SaveManager.Instance.Save();
        Reload();
    }
}
