using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreatureSelection : MonoBehaviour
{
    public Creature.Diet chosenDiet;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChoseDiet(int d)
    {
        chosenDiet = (Creature.Diet)d;
    }
}

