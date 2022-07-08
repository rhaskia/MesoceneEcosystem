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
        public StylizedWaterURP water;
        public LayerMask waterLayer;
        public Transform watercheck;
        public float waterLevel;
        public float strength;
        public float underwaterDrag = 3;
        public float underwaterAngularDrag = 1;
        public float depth = 1;
        public float swimStaminaUse;
        float airDrag = 0;
        float airAngularDrag = 0.05f;
        bool underwater;

        float steepness;
        float waveLength;
        float waterSpeed;
        float[] directions;

        PhotonView pv;

        private void Awake()
        {
            pv = GetComponentInParent<PhotonView>();
            if (!pv.IsMine) return;

            GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
            playerM = FindObjectOfType<Player.PlayerManager>();
            creature = playerM.creature;

            // Get wave properties from water
            water = FindObjectOfType<StylizedWaterURP>();

            steepness = water.GetWaveSteepness();
            waveLength = water.GetWaveLength();
            waterSpeed = water.GetWaveSpeed();
            directions = water.GetWaveDirections();
        }

        private void OnDestroy()
        {
            GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }

        private void FixedUpdate()
        {
            //heaigfght

            if (moveInput == null || !playerM.pv.IsMine)
                return;

            Vector3 speed = new Vector3(moveInput.movement.normalized.x, 0, moveInput.movement.normalized.y);
            float speedMult = GetSpeed();

            if (Mathf.Abs(speed.x) + Mathf.Abs(speed.z) > 0.75f) glideDir = speed;

            //Applying Input If On Ground
            Vector3 s = transform.right * moveInput.movement.normalized.x * speedMult * ((playerM.growth.currentPercent / 2f) + 50f) / 100f;
            Vector3 f = transform.forward * moveInput.movement.normalized.y * speedMult * ((playerM.growth.currentPercent / 2f) + 50f) / 100f;

            if (onGround || flying || underwater) rb.velocity = new Vector3(s.x + f.x, rb.velocity.y, s.z + f.z);

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

            //Swimming
            var pos = transform.position;
            var wave = transform.position;
            wave.y = water.transform.position.y + GerstnerWaveDisplacement.GetWaveDisplacement(pos, steepness, waveLength, waterSpeed, directions).y;

            float waveHeight = wave.y;
            float effectorHeight = pos.y;

            if (effectorHeight < waveHeight) // submerged
            {
                float submersion = Mathf.Clamp01(waveHeight - effectorHeight) / depth;
                float buoyancy = Mathf.Abs(Physics.gravity.y) * submersion * strength;

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

                if (!underwater) SwitchState(true);
            }
            else if (underwater) SwitchState(false);

        }
        void Update()
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, RoomManager.Instance.rotation, transform.rotation.z));

            if (moveInput == null || !playerM.pv.IsMine)
                return;

            //Groundcheck
            onGround = Physics.Raycast(groundcheck.position, transform.TransformDirection(Vector3.down), 0.5f, groundLayer);

            //Managing stuff
            if (onGround)
            {
                gliding = false; flying = false;

                if (moveInput.jump) rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
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

        public float GetSpeed()
        {
            var mult = 1f;
            if (underwater) mult = 0.7f;

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

        private void OnGameStateChanged(GameState newGameState)
        {
            enabled = newGameState == GameState.GamePlay;
        }

        void SwitchState(bool u)
        {
            underwater = u;
            if (underwater)
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
    }
}
