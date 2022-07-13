using UnityEngine;

namespace Creature
{

    [System.Serializable]
    public class AnimationBundle
    {
        public Sprite[] side;
        public Sprite[] front;
        public Sprite[] back;
        public float speed = 0.15f;
    }

    [System.Serializable]
    public class MoveType
    {
        public float speed = 1;
        public float staminaUse;
        public float minStamina = 10;
        public float disturbance; //for sneaking etc
        public bool can = true;
    }

    [CreateAssetMenu(fileName = "Critter")]
    public class Creature : ScriptableObject
    {
        public float mass;
        public float length;

        public MoveType sneakSpeed, walkSpeed, trotSpeed, runSpeed, jumpForce, glideSpeed, flySpeed;

        public AnimationBundle idle, walk, run, jump, glide, fly, rest, sleep, eat, drink, lmb, rmb, limp, death;

        public Sprite dead;
    }
}
