using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    [Header("Relations")]
    public Creature creature;
    public Movement movement;
    public CreatureAnimation animator;
    public Health health;
    public Growth growth;
    public CameraFollow follow;

    public Rigidbody rb;
    public SpriteRenderer render;

    [Header("Variables")]
    public Canvas canvas;
    public Slider healthSlider;
    public Slider staminaSlider;
    public Slider thirstSlider;
    public Slider hungerSlider;

    PhotonView pv;

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

        //Animations
        ManageAnimations(input);

        float size = (((growth.currentPercent / 2f) + 50f) / 100f) * creature.size;
        transform.localScale = size * Vector3.one;
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
            animator.currentAnim == CreatureAnimation.Animations.idle ||
            animator.currentAnim == CreatureAnimation.Animations.run ||
            animator.currentAnim == CreatureAnimation.Animations.walk;

        //If not animating movement
        if (!movingAnims)
            return;

        //Animations
        if (rb.velocity.x + rb.velocity.z > creature.walkSpeed.speed + 0.01f || rb.velocity.x + rb.velocity.z < -creature.walkSpeed.speed + 0.01f)
        {
            animator.currentAnim = CreatureAnimation.Animations.run;
        }
        else if (rb.velocity.x + rb.velocity.z > creature.walkSpeed.speed / 8f || rb.velocity.x + rb.velocity.z < -creature.walkSpeed.speed / 8f)
        {
            animator.currentAnim = CreatureAnimation.Animations.walk;
        }
        else
        {
            animator.currentAnim = CreatureAnimation.Animations.idle;
        }
    }
}