using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureSelector : MonoBehaviour
{
    Creature.Creature[] creatures;
    public Image[] images;
    public int current;
    public int offset;

    // Start is called before the first frame update
    void Start()
    {
        creatures = RoomManager.Instance.creatures;

        UpdateSelector();
    }

    void Update()
    {
        UpdateSelector();
    }

    
    void UpdateSelector()
    {
        for (int i = 0; i < images.Length; i++)
        {
            var sprite = creatures[i - current].idle.side[0];
            images[i].sprite = sprite;
            images[i].transform.localScale = new Vector3(images[i].rectTransform.rect.width, (images[i].rectTransform.rect.width / sprite.rect.width) * sprite.rect.height);
        }
    }
}
