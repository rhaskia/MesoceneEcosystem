using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextChange : MonoBehaviour
{
    public string[] text;
    public string[] tips;
    public Sprite[] sprites;
    public Image image;
    public string tip;
    public float delay;
    int current;
    int currentSprite;
    public TextMeshProUGUI textmesh;
    public TextMeshProUGUI tipstextmesh;

    void Start()
    {
        Invoke("ChangeText", delay);
        Invoke("ChangeSprite", delay);

        ReloadFact();
    }

    void Update()
    {
        tipstextmesh.text = tip;
    }

    void ReloadFact()
    {
        tip = tips[Random.Range(0, tips.Length)];
    }

    void ChangeSprite()
    {
        currentSprite++;
        if (currentSprite >= sprites.Length) currentSprite = 0;

        image.sprite = sprites[currentSprite];

        Invoke("ChangeSprite", 0.08f);
    }

    void ChangeText()
    {
        current++;
        if (current >= text.Length) current = 0;

        textmesh.text = text[current];

        Invoke("ChangeText", delay);
    }
}
