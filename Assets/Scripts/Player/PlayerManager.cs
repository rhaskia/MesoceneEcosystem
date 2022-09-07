using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Cinemachine;

namespace Player
{
    //Manages most player stuff
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        [Header("Relations")]
        public Creature.Creature creature;
        public Creature.CreatureInfo info;
        public Creature.Health health;
        public Creature.Growth growth;
        public CameraFollow follow;
        public GameObject cam;

        public Rigidbody rb;

        [Header("Variables")]
        public Canvas canvas;
        public Slider healthSlider;
        public Slider staminaSlider;
        public Slider thirstSlider;
        public Slider hungerSlider;

        public PhotonView pv;

        public KeyCode pauseButton;
        public GameObject pauseMenu;


        void Start()
        {
            pv = GetComponent<PhotonView>();

            UpdateCreature();

            if (!pv.IsMine) Destroy(cam);


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

            info.creature = creature;
        }

        [PunRPC]
        void UpdateCreatureRPC(int c)
        {
            creature = RoomManager.Instance.creatures[c];
            print(c);
        }

        void Update()
        {

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
            staminaSlider.value = info.movement.stamina / (info.movement.maxStamina * 1f);
            thirstSlider.value = health.thirst / (health.maxThirst * 1f);
            hungerSlider.value = health.hunger / (health.maxHunger * 1f);

            //Flipping
            pv.RPC("SetFlip", RpcTarget.All, input);

            //Animations
            ManageAnimations(input);
        }

        public void OnPlayerConnected()
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
                info.canimation.flip = true;
            }
            else if (input.x < -0.01)
            {
                info.canimation.flip = false;
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
                info.canimation.currentAnim == Creature.Animations.idle ||
                info.canimation.currentAnim == Creature.Animations.run ||
                info.canimation.currentAnim == Creature.Animations.walk;

            //If not animating movement
            if (!movingAnims)
                return;

            //Animations
            if (rb.velocity.magnitude > creature.walkSpeed.speed + 0.01f || rb.velocity.magnitude < -creature.walkSpeed.speed + 0.01f)
            {
                info.canimation.SetCurrent(Creature.Animations.run);
            }
            else if (rb.velocity.magnitude > creature.walkSpeed.speed / 8f || rb.velocity.magnitude < -creature.walkSpeed.speed / 8f)
            {
                info.canimation.SetCurrent(Creature.Animations.walk);
            }
            else
            {
                info.canimation.SetCurrent(Creature.Animations.idle);
            }
        }

        private void OnDestroy()
        {
            SceneManager.LoadScene(0);
        }
    }
}
