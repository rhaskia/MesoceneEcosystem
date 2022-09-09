using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureSlot : MonoBehaviour
{
    public Creature.Creature creature;
    public Image image;

    // Update is called once per frame
    void Update()
    {
        if (creature != null) image.sprite = creature.idle.side[0];
    }
}
