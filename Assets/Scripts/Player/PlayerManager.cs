using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace Player
{
    //Manages most player stuff
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

        public KeyCode pauseButton;
        public GameObject pauseMenu;


        void Awake()
        {
            pv = GetComponent<PhotonView>();

            UpdateCreature();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UpdateCreature()
        {
            if (pv.IsMine)
            {
                creature = RoomManager.Instance.creatures[PlayerPrefs.GetInt("Creature")];
                pv.RPC("UpdateCreatureRPC", RpcTarget.All, PlayerPrefs.GetInt("Creature"));
            }

            animator.current = creature;
        }

        [PunRPC]
        void UpdateCreatureRPC(int c)
        {
            creature = RoomManager.Instance.creatures[c];
        }

        void Update()
        {
            animator.current = creature;

            if (!pv.IsMine)
                canvas.gameObject.SetActive(false);

            if (!pv.IsMine)
                return;

            //Pause Menu
            if (Input.GetKeyDown(pauseButton)) SwitchPause();

            if (GameStateManager.Instance.paused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                pauseMenu.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                pauseMenu.SetActive(false);
            }


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

        private void OnPlayerConnected()
        {
            UpdateCreature();
        }

        public void LeaveGame()
        {
            StartCoroutine("DisconnectPlayer");
        }

        public void SwitchPause()
        {
            GameStateManager.Instance.SwitchState();
        }

        IEnumerator DisconnectPlayer()
        {
            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.InRoom)
                yield return null;

            SceneManager.LoadScene(0);
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

        private void OnDestroy()
        {
            SceneManager.LoadScene(0);
        }
    }
}

