using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    public class Combat : MonoBehaviour
    {
        public KeyCode attackKey;
        public CAnimation animation;

        public GameObject attackHitBox;

        public bool isAtacking;
        public bool isBlocking;

        // Update is called once per frame
        void Update()
        {
            if (GameStateManager.Instance.paused)
                return;

            if (Input.GetKeyDown(attackKey))
            {
                Attack1();
                animation.SetCurrent(Animations.LMB);
            }

            if (Input.GetMouseButtonDown(0))
            {
                isBlocking = true;
                animation.SetCurrent(Animations.LMB);
                Physics.OverlapBox(attackHitBox.transform.position, attackHitBox.transform.lossyScale.x / 2, atta)
            }

            if (Input.GetMouseButtonDown(1))
            {
                isBlocking = false;
                animation.SetCurrent(Animations.RMB);
            }

            attackHitBox.SetActive(animation.currentAnim == Animations.LMB);
        }

        [ContextMenu("Attack 01")]
        public void Attack1()
        {
            //controllerANIM.SetTrigger("Biting");
            isBlocking = false;
            //controllerANIM.SetBool("Atacking", isBlocking);
        }

        public void SetBlocking()
        {
            if (isBlocking)
            {
                isAtacking = false;
            }
            else if (!isBlocking)
            {
                isAtacking = true;
            }

            //controllerANIM.SetBool("Atacking", isAtacking);
        }
    }
}