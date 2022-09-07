using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    public class CreatureInfo : MonoBehaviour
    {
        public Creature creature;
        public Movement movement;
        public Growth growth;
        public CAnimation canimation;
        bool set;

        // Start is called before the first frame update
        void Start()
        {
            movement.creature = creature;
            canimation.current = creature;
        }

        // Update is called once per frame
        void Update()
        {
            if (creature != null && !set)
            {
                canimation.StartAnim();
                set = true;
            }
        }
    }
}
