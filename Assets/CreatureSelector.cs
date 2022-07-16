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
        for (int i = 0; i < images.Length; i++)
        {
            if (i + current >= creatures.Length || i + current < 0)
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

    public void MoveLeft()
    {
        current = Mathf.Clamp(current - 1, -offset, creatures.Length - offset * 2);
    }

    public void MoveRight()
    {
        current = Mathf.Clamp(current + 1, -offset, creatures.Length - offset * 2);
    }
}
