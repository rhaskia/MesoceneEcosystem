using UnityEngine;

public enum Biome
{
    BorealForest, Chaparral, Coastal, CoastalSpires, Desert, GameTrail, Grasslands, HotSprings, MarshallIslands, OasisIslands,
    Rainforest, Redwoods, RollingHills, Savannah, Taiga, TemperateArchipelago, TemperateForest, Tundra, Wetlands, WindwellBay
};

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

    public enum Diet { omnivore, herbivore, carnivore, piscivore, insectivore }

    //Creature info and animation holder
    [System.Serializable]
    [CreateAssetMenu(fileName = "Critter")]
    public class Creature : ScriptableObject
    {
        public Diet diet;

        public float mass;
        public float length;

        public int maxHealth = 100;
        public float staminaRegen, healthRegen;

        public string description;

        [Space]

        public MoveType sneakSpeed;
        public MoveType walkSpeed, trotSpeed, runSpeed, jumpForce, glideSpeed, flySpeed;
        public float buoyancy;
        public MoveType swimSpeed;

        [Space]

        public AnimationBundle idle;
        public AnimationBundle walk, run, jump, crouch, crouchwalk, glide, fly, rest, sleep, eat, drink, lmb, rmb, limp, death;

        public Sprite dead;
    }
}
