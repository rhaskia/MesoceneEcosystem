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
        CreatureInfo info;
        public Rigidbody rb;
        public MoveInput moveInput;
        public Creature creature;
        public CAnimation anim;
        public Combat combat;

        [Header("Variables")]
        public float stamina;
        public float maxStamina;
        public float jumpForce;
        Vector3 totalInput;

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
        LayerMask waterLayer;
        bool inWater;
        public float submergence;
        public float submergenceOffset = 0.5f;
        public float submergenceRange = 1f;

        public float airDrag = 0.75f;
        public float waterDrag = 2f;

        public float buoyancy = 1f;
        public float swimThreshold;

        //StylizedWater.wa water;

        bool swimming => submergence >= swimThreshold;

        PhotonView pv;

        private void Start()
        {
            pv = GetComponentInParent<PhotonView>();
            info = GetComponent<CreatureInfo>();

            if (!pv.IsMine) return;

            rb.mass = creature.mass;
        }

        [PunRPC]
        void UpdateSizesRPC()
        {
            rb.mass = creature.mass;
        }

        private void FixedUpdate()
        {
            //heaigfght
            //BuoyancyManager();

            if (moveInput == null || !pv.IsMine || GameStateManager.Instance.paused)
                return;

            Vector3 speed = new Vector3(moveInput.movement.normalized.x, 0, moveInput.movement.normalized.y);
            float speedMult = GetSpeed();

            if (Mathf.Abs(speed.x) + Mathf.Abs(speed.z) > 0.75f) glideDir = speed;

            //Applying Input If On Ground
            Vector3 side = transform.right * moveInput.movement.normalized.x * speedMult * ((info.growth.currentPercent / 2f) + 50f) / 100f;
            Vector3 forward = transform.forward * moveInput.movement.normalized.y * speedMult * ((info.growth.currentPercent / 2f) + 50f) / 100f;
            totalInput = side + forward;

            if (onGround || flying) rb.velocity = new Vector3(totalInput.x, rb.velocity.y, totalInput.z);

            //Stamina
            stamina += creature.staminaRegen * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);

            //Flying
            if (flying)
            {
                if (moveInput.flyUp) rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
                if (moveInput.flyDown) rb.AddForce(new Vector3(0, -jumpForce, 0), ForceMode.Impulse);
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
            if (moveInput == null || !pv.IsMine || GameStateManager.Instance.paused)
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

                if (moveInput.glide && creature.glideSpeed.can)
                {
                    gliding = !gliding;
                    flying = false;
                }

                if (moveInput.fly && creature.flySpeed.can)
                {
                    flying = !flying;
                    gliding = false;
                }
            }

            //Crouching
            if (onGround && moveInput.crouch) crouching = !crouching;

            //Animations
            ManageAnimations();

            //Swimmming
            if (inWater)
            {

                if (swimming && stamina > creature.swimSpeed.minStamina && creature.swimSpeed.can)
                {
                    buoyancy = creature.buoyancy;

                    if (moveInput.flyUp) buoyancy = creature.buoyancy + creature.swimSpeed.speed;
                    if (moveInput.flyDown) buoyancy = creature.buoyancy - creature.swimSpeed.speed;

                    stamina -= creature.swimSpeed.staminaUse;
                }
                else buoyancy = 0.5f;

                EvaluateSubmergence();
                rb.drag = waterDrag * submergence;
                rb.velocity += -9.81f * ((1f - buoyancy * submergence) * Time.deltaTime) * Vector3.up;
            }
            else rb.drag = airDrag;

            //

        }

        //Gets player movement speed
        public float GetSpeed()
        {
            var mult = 1f;
            if (inWater) mult = 0.75f;

            //Crouching
            if (crouching) return creature.sneakSpeed.speed;

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

            stamina -= creature.walkSpeed.staminaUse * Time.fixedDeltaTime;
            return creature.walkSpeed.speed * mult;
        }

        //Updates mass
        private void OnPlayerConnected()
        {
            pv.RPC("UpdateSizesRPC", RpcTarget.All, creature.mass);
        }

        void ManageAnimations()
        {
            //idk
            bool movingAnims =
                info.canimation.currentAnim == Animations.idle ||
                info.canimation.currentAnim == Animations.run ||
                info.canimation.currentAnim == Animations.walk;

            //If not animating movement
            if (!movingAnims)
                return;

            //Normal Anims
            if (totalInput.magnitude > 0.1f)
            {
                if (moveInput.trot || moveInput.run) info.canimation.SetCurrent(Animations.run);
                else info.canimation.SetCurrent(Animations.walk);
            }
            else info.canimation.SetCurrent(Animations.idle);

            //Crouching animations
            if (crouching)
            {
                if (info.canimation.currentAnim == Animations.idle) info.canimation.SetCurrent(Animations.crouch);
                else info.canimation.SetCurrent(Animations.crouchwalk);
            }


        }

        //Swimming Stuff
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Water"))
            {
                inWater = true;
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Water"))
            {
                inWater = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Water"))
            {
                inWater = false;
            }
        }

        void EvaluateSubmergence()
        {
            if (Physics.Raycast(
            transform.position + Vector3.up * submergenceOffset,
            -Vector3.up, out RaycastHit hit, submergenceRange + 1,
            waterLayer, QueryTriggerInteraction.Collide))
            {
                submergence = 1f - hit.distance / submergenceRange;
            }
            else
            {
                submergence = 1f;
            }
        }
    }
}
