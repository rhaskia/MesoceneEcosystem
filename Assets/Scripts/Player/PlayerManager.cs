using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Player
{

    public class PlayerManager : MonoBehaviour
    {
        [Header("Relations")]
        public Creature.Creature creature;
        public Creature.Movement movement;
        public Creature.Animation animator;
        public Creature.Health health;
        public Creature.Growth growth;
        public CameraFollow follow;

        public Rigidbody rb;
        public SpriteRenderer render;

        [Header("Variables")]
        public Canvas canvas;
        public Slider healthSlider;
        public Slider staminaSlider;
        public Slider thirstSlider;
        public Slider hungerSlider;

        public PhotonView pv;

        void Start()
        {
            animator.current = creature;
            pv = GetComponent<PhotonView>();
        }

        void Update()
        {
            if (!pv.IsMine)
                canvas.gameObject.SetActive(false);

            if (!pv.IsMine)
                return;

            //Input
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            //UI
            healthSlider.value = health.health / (health.maxHealth * 1f);
            staminaSlider.value = movement.stamina / (movement.maxStamina * 1f);
            thirstSlider.value = health.thirst / (health.maxThirst * 1f);
            hungerSlider.value = health.hunger / (health.maxHunger * 1f);

            //Flipping
            pv.RPC("SetFlip", RpcTarget.All, input);

            //Animations
            ManageAnimations(input);

            float size = (((growth.currentPercent / 2f) + 50f) / 100f) * creature.size;
            transform.localScale = size * Vector3.one;
        }

        [PunRPC]
        public void SetFlip(Vector2 input)
        {
            if (input.x > 0.01)
            {
                render.flipX = true;
                animator.shadow.flipX = true;
            }
            else if (input.x < -0.01)
            {
                render.flipX = false;
                animator.shadow.flipX = false;
            }
        }

        public void Die()
        {
            //YOU DIED
            Debug.LogError("YOU DIED");
        }

        void ManageAnimations(Vector2 _input)
        {
            //idk
            bool movingAnims =
                animator.currentAnim == Creature.Animation.Animations.idle ||
                animator.currentAnim == Creature.Animation.Animations.run ||
                animator.currentAnim == Creature.Animation.Animations.walk;

            //If not animating movement
            if (!movingAnims)
                return;

            //Animations
            if (rb.velocity.x + rb.velocity.z > creature.walkSpeed.speed + 0.01f || rb.velocity.x + rb.velocity.z < -creature.walkSpeed.speed + 0.01f)
            {
                animator.currentAnim = Creature.Animation.Animations.run;
            }
            else if (rb.velocity.x + rb.velocity.z > creature.walkSpeed.speed / 8f || rb.velocity.x + rb.velocity.z < -creature.walkSpeed.speed / 8f)
            {
                animator.currentAnim = Creature.Animation.Animations.walk;
            }
            else
            {
                animator.currentAnim = Creature.Animation.Animations.idle;
            }
        }
    }
}