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
        public SpriteRenderer spriteRenderer;
        public SpriteRenderer shadow;
        public Vector3 shadowDir;
        public LayerMask layerMask;
        public Creature current;
        public MeshRenderer material;

        public bool flip;

        public BoxCollider collider;
        public float ppu;
        PhotonView pv;

        void Start()
        {
            Invoke("ManageAnimation", 0.1f);
            pv = gameObject.GetComponentInParent<PhotonView>();
        }

        void Update()
        {
            if (pv.IsMine) return;

            //Shadow stuff
            //Need to make a shader for it really
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(shadowDir), out hit, Mathf.Infinity, layerMask))
            {
                shadow.transform.position = hit.point;
            }

            //Setting collidor size
            collider.size = new Vector3(spriteRenderer.sprite.rect.width / ppu, spriteRenderer.sprite.rect.height / ppu, 0.5f);
            collider.center = new Vector3(0, spriteRenderer.sprite.rect.height / (ppu * 2), 0);
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

                    var texture = textureFromSprite(anim.side[currentFrame]);
                    if (flip) material.material.mainTextureScale = new Vector2(-1, 1);
                    else material.material.mainTextureScale = new Vector2(1, 1);
                    material.material.mainTexture = texture;

                    break;

                case Directions.Front:
                    if (currentFrame >= anim.front.Length)
                    { currentFrame = 0; }
                    spriteRenderer.sprite = anim.front[currentFrame];
                    break;

                case Directions.Back:
                    if (currentFrame >= anim.back.Length)
                    { currentFrame = 0; }
                    spriteRenderer.sprite = anim.back[currentFrame];
                    break;
            }
        }

        void AnimationOneTime(Sprite[] side, Sprite[] front, Sprite[] back)
        {
            switch (currentDir)
            {
                case Directions.Side:
                    if (currentFrame >= side.Length)
                    { currentAnim = Animations.idle; currentFrame = 0; }
                    spriteRenderer.sprite = side[currentFrame];
                    break;
                case Directions.Front:
                    if (currentFrame >= front.Length)
                    { currentAnim = Animations.idle; currentFrame = 0; }
                    spriteRenderer.sprite = front[currentFrame];
                    break;
                case Directions.Back:
                    if (currentFrame >= back.Length)
                    { currentAnim = Animations.idle; currentFrame = 0; }
                    spriteRenderer.sprite = back[currentFrame];
                    break;
            }
            currentFrame++;

        }
    }
}