using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    public class Combat : MonoBehaviour
    {
        public KeyCode attackKey;
        public CAnimation animator;

        public GameObject attackHitBox;

        public bool isAttacking;

        PhotonView pv;

        void Start()
        {
            pv = GetComponent<PhotonView>();
        }

        // Update is called once per frame
        void Update()
        {
            if (GameStateManager.Instance.paused || !pv.IsMine)
                return;

            if (Input.GetKeyDown(attackKey))
            {
                Attack1();
                animator.SetCurrent(Animations.LMB);
            }

            if (Input.GetMouseButtonDown(0))
            {
                //isBlocking = true;
                animator.SetCurrent(Animations.LMB);
                var collisions = Physics.OverlapBox(attackHitBox.transform.position, attackHitBox.transform.lossyScale / 2, attackHitBox.transform.rotation);

                foreach (var item in collisions)
                {
                    if (item.tag == "Creature" && item.gameObject != gameObject)
                    {
                        item.GetComponent<Health>().TakeDamage(20);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                //isBlocking = false;
                animator.SetCurrent(Animations.RMB);
            }

            isAttacking = animator.currentAnim == Animations.LMB;
            attackHitBox.SetActive(animator.currentAnim == Animations.LMB);
        }

        [ContextMenu("Attack 01")]
        public void Attack1()
        {
            //controllerANIM.SetTrigger("Biting");
            //isBlocking = false;
            //controllerANIM.SetBool("Atacking", isBlocking);
        }

        //public void setblocking()
        //{
        //    if (isblocking)
        //    {
        //        isatacking = false;
        //    }
        //    else if (!isblocking)
        //    {
        //        isatacking = true;
        //    }

        //    //controlleranim.setbool("atacking", isatacking);
        //}
    }
}