using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;

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

        public GameObject cam;
        public CinemachineFreeLook freeLook;
        public CinemachineFreeLookZoom zoom;

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
        public Slider sensX, sensY;
        public TextMeshProUGUI textX, textY;
        //public Slider sfx, music;
        public bool menuOpen;


        void Start()
        {
            pv = GetComponent<PhotonView>();

            creature = RoomManager.Instance.creatures[(int)pv.Owner.CustomProperties["Creature"]];
            info.creature = creature;

            if (!pv.IsMine) Destroy(cam);
            //if (!pv.IsMine) Destroy(canvas);


            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            pauseMenu.SetActive(false);

            //Loading Save
            if (pv.IsMine) health.UpdateHealth(SaveManager.Instance.saves[SaveManager.Instance.chosenSave].health);

            zoom.minMaxZoom *= creature.length;

            sensX.value = PlayerPrefs.GetInt("SensX", 10);
            sensY.value = PlayerPrefs.GetInt("SensY", 10);
        }

        //public void UpdateCreature()
        //{
        //    if (pv.IsMine)
        //    {
        //        creature = RoomManager.Instance.creatures[PlayerPrefs.GetInt("Creature")];
        //        pv.RPC("UpdateCreatureRPC", RpcTarget.All, PlayerPrefs.GetInt("Creature"));
        //    }

        //    info.creature = creature;
        //}

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

            //Sliders
            freeLook.m_XAxis.m_AccelTime = 1 / Mathf.Abs(sensX.value);
            freeLook.m_XAxis.m_InvertInput = sensX.value < 0;

            freeLook.m_YAxis.m_AccelTime = 1 / Mathf.Abs(sensY.value);
            freeLook.m_YAxis.m_InvertInput = sensY.value < 0;

            textX.text = (sensX.value / 10).ToString("0.0");
            textY.text = (sensY.value / 10).ToString("0.0");

            PlayerPrefs.SetInt("SensX", (int)sensX.value);
            PlayerPrefs.SetInt("SensY", (int)sensY.value);

            //Pause Menu
            if (Input.GetKeyDown(pauseButton)) SwitchPause();

            if (GameStateManager.Instance.paused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                freeLook.enabled = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                freeLook.enabled = true;
            }

            pauseMenu.SetActive(menuOpen);

            //Input
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            //UI
            healthSlider.value = health.health / (health.maxHealth * 1f);
            staminaSlider.value = info.movement.stamina / (info.movement.maxStamina * 1f);
            thirstSlider.value = health.thirst / (health.maxThirst * 1f);
            hungerSlider.value = health.hunger / (health.maxHunger * 1f);

            //Flipping
            pv.RPC("SetFlip", RpcTarget.All, input);

            //Saving
            SaveManager.Instance.saves[SaveManager.Instance.chosenSave].health = health.health;
            SaveManager.Instance.saves[SaveManager.Instance.chosenSave].position = new int[]
            { (int)info.movement.transform.position.x, (int)info.movement.transform.position.y, (int)info.movement.transform.position.z };
        }

        public void OnPlayerConnected()
        {
            //UpdateCreature();
        }

        public void LeaveGame()
        {
            StartCoroutine("DisconnectPlayer");
        }

        public void SwitchPause()
        {
            GameStateManager.Instance.SwitchState();
            menuOpen = !menuOpen;
        }

        IEnumerator DisconnectPlayer()
        {
            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.InRoom)
                yield return null;

            RoomManager.Instance.disconnected = false;
            SceneManager.LoadScene(0);
        }

        [PunRPC]
        public void SetFlip(Vector2 input)
        {
            if (input.x > 0.1)
            {
                info.canimation.flip = true;
            }
            else if (input.x < -0.1)
            {
                info.canimation.flip = false;
            }
        }

        public void Die()
        {
            //YOU DIED
            Debug.LogError("YOU DIED");
        }

        private void OnDestroy()
        {
            RoomManager.Instance.disconnected = true;
            SceneManager.LoadScene(0);
        }
    }
}
