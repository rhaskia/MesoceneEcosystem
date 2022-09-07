using UnityEngine;

namespace Creature
{
    //Animation holder
    [System.Serializable]
    public class AnimationBundle
    {
        public Sprite[] side;
        public Sprite[] front;
        public Sprite[] back;
        public float speed = 0.15f;
        public bool oneTime;
    }

    //Move type; holds speeds, stamina use, etc
    [System.Serializable]
    public class MoveType
    {
        public float speed = 1;
        public float staminaUse;
        public float minStamina = 10;
        public Disturbance disturbance; //for sneaking etc
        public bool can = true;
    }

    //Creature info and animation holder
    [CreateAssetMenu(fileName = "Critter")]
    public class Creature : ScriptableObject
    {
        public float mass;
        public float length;

        public float staminaRegen, healthRegen;

        [Space]

        public MoveType sneakSpeed;
        public MoveType walkSpeed, trotSpeed, runSpeed, jumpForce, glideSpeed, flySpeed;

        [Space]

        public AnimationBundle idle;
        public AnimationBundle walk, run, jump, glide, fly, rest, sleep, eat, drink, lmb, rmb, limp, death;

        public Sprite dead;
    }
}
