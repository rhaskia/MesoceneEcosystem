using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public int save;

    public TMP_InputField text;
    public Image image;

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
        if (save < SaveManager.Instance.saves.Count)
        {
            text.text = SaveManager.Instance.saves[save].name;

            var sprite = RoomManager.Instance.creatures[SaveManager.Instance.saves[save].creature].idle.side;
            if (sprite.Length != 0)
            {
                image.sprite = sprite[0];
                image.rectTransform.sizeDelta = new Vector2(330, (330 / sprite[0].rect.width) * sprite[0].rect.height);
            }
            else image.rectTransform.sizeDelta = new Vector2(330, 206.8f);
        }
    }

    public void SaveName(string n)
    {
        SaveManager.Instance.saves[save].name = n;
        SaveManager.Instance.Save();
        Reload();
    }
}
