using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    public class CAnimation : MonoBehaviour
    {
        public enum Animations { idle, walk, run, jump, glide, fly, rest, sleep, eat, drink, LMB, RMB, limp, death }
        public enum Directions { Side, Front, Back }

        [Header("Info")]
        public Animations currentAnim;
        public Directions currentDir;
        public int currentFrame;

        public Creature current;

        public MeshRenderer material;
        public Transform sprite;

        public bool flip;

        public CapsuleCollider maincollider;
        public CapsuleCollider slipcollider;
        public float ppu;
        PhotonView pv;


        void Start()
        {
            Invoke("ManageAnimation", 0.1f);
            pv = GetComponent<PhotonView>();
            ppu = current.ppu;
        }

        void Update()
        {
            print(pv.IsMine);
            if (!pv.IsMine) return;

            //Flipping
            if (flip) material.material.mainTextureScale = new Vector2(-1, 1);
            else material.material.mainTextureScale = new Vector2(1, 1);

            //Setting collidor size
            var size = new Vector3(material.material.mainTexture.width / ppu, material.material.mainTexture.height / ppu, 0.5f);
            var centre = new Vector3(0, material.material.mainTexture.height / (ppu * 2), 0);

            maincollider.height = size.x;
            maincollider.radius = size.y / 2f;
            maincollider.center = centre;

            slipcollider.height = size.x * 1.01f;
            slipcollider.radius = size.y / 2.01f;
            slipcollider.center = centre;

            print(size);
            sprite.localScale = size;
            sprite.localPosition = centre;
        }

        void ManageAnimation()
        {
            var allAnims = new AnimationBundle[] { current.idle, current.walk, current.run, current.jump, current.glide, current.fly, current.rest, current.sleep, current.eat, current.drink, current.lmb, current.rmb, current.limp, current.death };
            AnimationSet(allAnims[((int)currentAnim)]);

            pv.RPC("UpdateAnimations", RpcTarget.All, currentAnim, currentDir, currentFrame);

            Invoke("ManageAnimation", 0.1f);
        }

        public Texture2D textureFromSprite(Sprite sprite)
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
        void UpdateAnimations(Animations anim, Directions dir, int frame)
        {
            currentAnim = anim;
            currentDir = dir;
            currentFrame = frame;
        }

        void AnimationSet(AnimationBundle anim)
        {
            currentFrame++;


            switch (currentDir)
            {
                case Directions.Side:
                    if (currentFrame >= anim.side.Length)
                    { currentFrame = 0; }

                    material.material.mainTexture = textureFromSprite(anim.side[currentFrame]);
                    break;

                case Directions.Front:
                    if (currentFrame >= anim.front.Length)
                    { currentFrame = 0; }

                    material.material.mainTexture = textureFromSprite(anim.front[currentFrame]);
                    break;

                case Directions.Back:
                    if (currentFrame >= anim.back.Length)
                    { currentFrame = 0; }

                    material.material.mainTexture = textureFromSprite(anim.back[currentFrame]);
                    break;
            }
        }

        void AnimationOneTime(AnimationBundle anim)
        {
            switch (currentDir)
            {
                case Directions.Side:
                    if (currentFrame >= anim.side.Length)
                    { currentAnim = Animations.idle; currentFrame = 0; }
                    material.material.mainTexture = textureFromSprite(anim.front[currentFrame]);
                    break;

                case Directions.Front:
                    if (currentFrame >= anim.front.Length)
                    { currentAnim = Animations.idle; currentFrame = 0; }
                    material.material.mainTexture = textureFromSprite(anim.front[currentFrame]);
                    break;

                case Directions.Back:
                    if (currentFrame >= anim.back.Length)
                    { currentAnim = Animations.idle; currentFrame = 0; }
                    material.material.mainTexture = textureFromSprite(anim.front[currentFrame]);
                    break;
            }
            currentFrame++;

        }
    }
}