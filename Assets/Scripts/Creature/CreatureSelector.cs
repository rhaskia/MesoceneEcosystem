using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreatureSelector : MonoBehaviour
{
    Creature.Creature[] creatures;
    public Image[] images;
    public int current;
    public int offset;
    public Sprite empty;

    public TextMeshProUGUI nameText, infoText;

    // Start is called before the first frame update
    void Start()
    {
        creatures = RoomManager.Instance.creatures;

        UpdateSelector();
    }

    void Update()
    {
        UpdateSelector();

        nameText.text = creatures[current + offset].name;
        infoText.text = "Weight: " + creatures[current + offset].mass.ToString() + "kg\nLength: " + creatures[current + offset].length.ToString() + "m";
    }


    void UpdateSelector()
    {
        if (current + offset >= RoomManager.Instance.publicCreatures)
        {
            //Turns all sprites to nothing
            for (int i = 0; i < images.Length; i++)
            { images[i].sprite = empty; }

            var sprite = creatures[current + offset].idle.side[0];
            images[offset].sprite = sprite;
            images[offset].transform.localScale = new Vector3(images[offset].rectTransform.rect.width, (images[offset].rectTransform.rect.width / sprite.rect.width) * sprite.rect.height);
        }
        else
        {
            //Goes through every image, and sets creature sprites and heights
            for (int i = 0; i < images.Length; i++)
            {
                if (i + current >= RoomManager.Instance.publicCreatures || i + current < 0)
                {
                    images[i].sprite = empty;
                }
                else
                {
                    var sprite = creatures[i + current].idle.side[0];
                    images[i].sprite = sprite;
                    images[i].transform.localScale = new Vector3(images[i].rectTransform.rect.width, (images[i].rectTransform.rect.width / sprite.rect.width) * sprite.rect.height);
                }
            }
        }
    }

    public void MoveLeft()
    {
        if (current + offset >= RoomManager.Instance.publicCreatures)
        {
            current = RoomManager.Instance.publicCreatures - offset * 2 + 1;
            return;
        }

        current = Mathf.Clamp(current - 1, -offset, RoomManager.Instance.publicCreatures - offset * 2);
    }

    public void MoveRight()
    {
        if (current + offset >= RoomManager.Instance.publicCreatures)
        {
            current = RoomManager.Instance.publicCreatures - offset * 2 + 1;
            return;
        }

        current = Mathf.Clamp(current + 1, -offset, RoomManager.Instance.publicCreatures - offset * 2 + 1);
    }
}
