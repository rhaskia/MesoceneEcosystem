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
        public Creature.CAnimation animator;
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

            if (pv.IsMine) creature = RoomManager.Instance.creatures[PlayerPrefs.GetInt("Creature")];
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
        }

        [PunRPC]
        public void SetFlip(Vector2 input)
        {
            if (input.x > 0.01)
            {
                animator.flip = true;
            }
            else if (input.x < -0.01)
            {
                animator.flip = false;
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
                animator.currentAnim == Creature.CAnimation.Animations.idle ||
                animator.currentAnim == Creature.CAnimation.Animations.run ||
                animator.currentAnim == Creature.CAnimation.Animations.walk;

            //If not animating movement
            if (!movingAnims)
                return;

            //Animations
            if (rb.velocity.x + rb.velocity.z > creature.walkSpeed.speed + 0.01f || rb.velocity.x + rb.velocity.z < -creature.walkSpeed.speed + 0.01f)
            {
                animator.currentAnim = Creature.CAnimation.Animations.run;
            }
            else if (rb.velocity.x + rb.velocity.z > creature.walkSpeed.speed / 8f || rb.velocity.x + rb.velocity.z < -creature.walkSpeed.speed / 8f)
            {
                animator.currentAnim = Creature.CAnimation.Animations.walk;
            }
            else
            {
                animator.currentAnim = Creature.CAnimation.Animations.idle;
            }
        }
    }
}
