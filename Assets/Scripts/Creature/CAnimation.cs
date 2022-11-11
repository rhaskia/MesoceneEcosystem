using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    public enum Animations { idle, walk, run, crouch, crouchwalk, jump, glide, fly, rest, sleep, eat, drink, lmb, rmb, limp, death }
    public enum Directions { Side, Front, Back }

    //Creature Animation Manager
    public class CAnimation : MonoBehaviour
    {
        [Header("Info")]
        public Animations currentAnim;
        public Directions currentDir;
        public int currentFrame;

        public Creature current;

        public MeshRenderer material;
        public MeshRenderer back;
        public Transform sprite;
        public Transform AttackHolder;

        public bool flip;

        public CapsuleCollider maincollider;
        public CapsuleCollider slipcollider;
        public float ppu;
        PhotonView pv;

        Vector3 size;
        Vector3 centre = new Vector3();

        public AnimationBundle[] allAnims => new AnimationBundle[] { current.idle, current.walk, current.run, current.crouch, current.crouchwalk, current.jump, current.glide, current.fly,
            current.rest, current.sleep, current.eat, current.drink, current.lmb, current.rmb, current.limp, current.death };

        public void StartAnim()
        {
            pv = GetComponent<PhotonView>();
            ppu = current.idle.side[0].rect.width / current.length;
            ManageAnimation();
        }

        void Update()
        {
            size = new Vector3(material.material.mainTexture.width / ppu, material.material.mainTexture.height / ppu, 0.5f);
            centre = new Vector3(0, material.material.mainTexture.height / (ppu * 2), 0);

            //Sizes
            maincollider.height = size.x;
            maincollider.radius = size.y / 4f;
            maincollider.center = centre + Vector3.up * (material.material.mainTexture.height / ppu) * 0.25f;

            slipcollider.height = material.material.mainTexture.height / ppu;
            slipcollider.radius = size.y / 2f;
            slipcollider.center = centre;

            sprite.localScale = size;
            sprite.localPosition = centre;

            //Flipping
            if (flip) material.material.mainTextureScale = new Vector2(-1, 1);
            else material.material.mainTextureScale = new Vector2(1, 1);

            back.material = material.material;
            back.material.mainTextureScale = new Vector2(-back.material.mainTextureScale.x, 1);

            AttackHolder.localRotation = Quaternion.Euler(new Vector3(transform.rotation.x, flip ? 180 : 0, transform.rotation.z));
        }

        public void SetCurrent(Animations an)
        {
            if (currentAnim == an)
                return;

            currentAnim = an;
            CancelInvoke("ManageAnimation");
            currentFrame = 0;
            print(currentAnim);
            Invoke("ManageAnimation", allAnims[((int)currentAnim)].speed);
        }

        void ManageAnimation()
        {
            AnimationSet(allAnims[((int)currentAnim)]);

            pv.RPC("UpdateAnimations", RpcTarget.All, currentAnim, currentDir, currentFrame, ppu);

            Invoke("ManageAnimation", allAnims[((int)currentAnim)].speed);
        }

        Texture2D textureFromSprite(Sprite sprite)
        {
            if (sprite.rect.width != sprite.texture.width)
            {
                Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                             (int)sprite.textureRect.y,
                                                             (int)sprite.textureRect.width,
                                                             (int)sprite.textureRect.height);
                newText.SetPixels(newColors);
                newText.Apply();
                newText.filterMode = FilterMode.Point;
                return newText;
            }
            return sprite.texture;
        }

        [PunRPC]
        void UpdateAnimations(Animations anim, Directions dir, int frame, float _ppu)
        {
            currentAnim = anim;
            currentDir = dir;
            currentFrame = frame;
            ppu = _ppu;
        }

        void AnimationSet(AnimationBundle anim)
        {
            currentFrame++;

            switch (currentDir)
            {
                case Directions.Side:
                    AnimationCase(anim.side, anim);
                    break;

                case Directions.Front:
                    AnimationCase(anim.front, anim);
                    break;

                case Directions.Back:
                    AnimationCase(anim.back, anim);
                    break;
            }
        }

        void AnimationCase(Sprite[] set, AnimationBundle anim)
        {
            print(set.Length);

            if (currentFrame >= set.Length)
            {
                currentFrame = 0;
                if (anim.oneTime) currentAnim = Animations.idle;
            }

            if (set.Length == 0) Debug.LogWarning("Missing sprite sheet for the " + currentAnim.ToString() + " animation");
            if (set.Length == 0) currentAnim = Animations.idle;
            if (set.Length == 0) return;

            material.material.mainTexture = textureFromSprite(set[currentFrame]);
        }
    }
}