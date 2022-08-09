using Photon.Pun;
using StylizedWater;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Creature
{
    public class MoveInput
    {
        public Vector2 movement;
        public bool crouch;
        public bool trot, run;
        public bool jump, glide, fly;
        public bool flyDown, flyUp;

        public MoveInput(Vector2 m, bool _crouch, bool _trot, bool _run,
            bool _jump, bool _glide, bool _fly, bool _fDown, bool _fUp)
        {
            movement = m; crouch = _crouch; trot = _trot; run = _run; jump = _jump; glide = _glide; fly = _fly; flyDown = _fDown; flyUp = _fUp;
        }
    }

    public class Movement : MonoBehaviour
    {
        public Cinemachine.CinemachineFreeLook cam;

        [Header("Relations")]
        Player.PlayerManager playerM;
        public Rigidbody rb;
        public MoveInput moveInput;
        public Creature creature;

        [Header("Variables")]
        public float stamina;
        public float maxStamina;
        public float jumpForce;

        [Header("GroundCheck")]
        public float groundcheckRadius = 0.1f;
        public LayerMask groundLayer;
        public Transform groundcheck;
        public bool onGround;

        bool crouching, gliding, flying;

        [Header("Gliding")]
        public float maxGlideSpeed;
        public float glideEfficieny;
        public Vector3 glideDir;

        [Header("Swimming")]
        public LayerMask waterLayer;
        public float strength;
        public float underwaterDrag = 3;
        public float underwaterAngularDrag = 1;
        public float depth = 1;
        public float swimStaminaUse;
        StylizedWaterURP water;
        float airDrag = 0;
        float airAngularDrag = 0.05f;
        bool inWater;
        bool underWater;

        float steepness;
        float waveLength;
        float waterSpeed;
        float[] directions;

        PhotonView pv;

        private void Awake()
        {
            pv = GetComponentInParent<PhotonView>();
            if (!pv.IsMine) return;

            playerM = FindObjectOfType<Player.PlayerManager>();
            creature = playerM.creature;

            // Get wave properties from water
            water = FindObjectOfType<StylizedWaterURP>();

            steepness = water.GetWaveSteepness();
            waveLength = water.GetWaveLength();
            waterSpeed = water.GetWaveSpeed();
            directions = water.GetWaveDirections();

            rb.mass = creature.mass;

            pv.RPC("UpdateSizesRPC", RpcTarget.All, creature.mass);
        }

        [PunRPC]
        void UpdateSizesRPC(float mass)
        {
            rb.mass = mass;
        }

        private void FixedUpdate()
        {
            //heaigfght
            BuoyancyManager();

            if (moveInput == null || !pv.IsMine || GameStateManager.Instance.paused)
                return;

            Vector3 speed = new Vector3(moveInput.movement.normalized.x, 0, moveInput.movement.normalized.y);
            float speedMult = GetSpeed();

            if (Mathf.Abs(speed.x) + Mathf.Abs(speed.z) > 0.75f) glideDir = speed;

            //Applying Input If On Ground
            Vector3 side = transform.right * moveInput.movement.normalized.x * speedMult * ((playerM.growth.currentPercent / 2f) + 50f) / 100f;
            Vector3 forward = transform.forward * moveInput.movement.normalized.y * speedMult * ((playerM.growth.currentPercent / 2f) + 50f) / 100f;

            if (onGround || flying || inWater) rb.velocity = new Vector3(side.x + forward.x, rb.velocity.y, side.z + forward.z);

            //Stamina
            stamina += 2;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);

            //Flying
            if (flying)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.one * -0.01f, 0.99f);

                if (moveInput.flyUp) rb.AddForce(new Vector3(0, 0.1f, 0), ForceMode.Impulse);
                if (moveInput.flyDown) rb.AddForce(new Vector3(0, -0.1f, 0), ForceMode.Impulse);
            }
            rb.useGravity = !flying;

            //Gliding
            if (gliding)
            {
                float mult = 1 + (glideEfficieny * Time.deltaTime);
                glideDir = new Vector3(Mathf.Clamp(glideDir.x * mult, -maxGlideSpeed, maxGlideSpeed), -0.1f, Mathf.Clamp(glideDir.z * mult, -maxGlideSpeed, maxGlideSpeed));
                rb.velocity = glideDir;
            }



        }

        void Update()
        {
            if (moveInput == null || !pv.IsMine)
                return;

            //Rotation
            RoomManager.Instance.rotation = cam.m_XAxis.Value;
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, RoomManager.Instance.rotation, transform.rotation.z));

            //Groundcheck
            onGround = Physics.Raycast(groundcheck.position, transform.TransformDirection(Vector3.down), 0.5f, groundLayer);

            //Managing stuff
            if (onGround)
            {
                gliding = false; flying = false;

                if (moveInput.jump && creature.jumpForce.can) rb.AddForce(new Vector3(0, creature.jumpForce.speed, 0), ForceMode.Impulse);
                if (moveInput.crouch) crouching = !crouching;
            }
            else
            {
                crouching = false;

                if (moveInput.glide)
                {
                    gliding = !gliding;
                    flying = false;
                }

                if (moveInput.fly)
                {
                    flying = !flying;
                    gliding = false;
                }
            }

            //Crouching
            if (onGround && moveInput.crouch) crouching = !crouching;
        }

        void BuoyancyManager()
        {
            //Swimming
            var pos = transform.position;
            var wave = transform.position;
            wave.y = water.transform.position.y + GerstnerWaveDisplacement.GetWaveDisplacement(pos, steepness, waveLength, waterSpeed, directions).y;

            float waveHeight = wave.y;
            float effectorHeight = pos.y;

            if (effectorHeight < waveHeight) //In Water
            {
                float submersion = Mathf.Clamp01(waveHeight - effectorHeight) / depth;
                float buoyancy = Mathf.Abs(Physics.gravity.y) * submersion * strength;

                //Buoyancy
                if (stamina > 10)
                {
                    rb.AddForceAtPosition(Vector3.up * buoyancy, pos, ForceMode.Acceleration);
                }
                else
                {
                    rb.AddForceAtPosition(Vector3.up * buoyancy / 3, pos, ForceMode.Acceleration);

                }

                stamina -= 10 * Time.deltaTime;

                gliding = false;
                flying = false;

                if (!inWater) SwitchState(true);
            }
            else if (inWater) SwitchState(false);

            underWater = transform.position.y + (transform.lossyScale.y / 2) < waveHeight;
        }


        //Gets player movement speed
        public float GetSpeed()
        {
            var mult = 1f;
            if (inWater) mult = 0.7f;

            //Trotting
            if (moveInput.trot)
            {
                stamina -= creature.runSpeed.staminaUse;
                if (stamina > creature.runSpeed.minStamina) return creature.runSpeed.speed * mult;
                crouching = false;
            }

            //Running
            if (moveInput.run)
            {
                stamina -= creature.runSpeed.staminaUse;
                if (stamina > creature.runSpeed.minStamina) return creature.runSpeed.speed * mult;
                crouching = false;
            }

            //Crouching
            if (crouching) return creature.sneakSpeed.speed;

            stamina -= creature.walkSpeed.staminaUse;
            return creature.walkSpeed.speed * mult;
        }

        //Switches between underwater and not
        void SwitchState(bool u)
        {
            inWater = u;
            if (inWater)
            {
                rb.drag = underwaterDrag;
                rb.angularDrag = underwaterAngularDrag;
            }
            else
            {
                rb.drag = airDrag;
                rb.angularDrag = airAngularDrag;
            }
        }

        //Updates mass
        private void OnPlayerConnected()
        {
            pv.RPC("UpdateSizesRPC", RpcTarget.All, creature.mass);
        }
    }
}
