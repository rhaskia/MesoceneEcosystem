using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextChange : MonoBehaviour
{
    public string[] text;
    public float delay;
    int current;
    TextMeshProUGUI textmesh;

    void Start()
    {
        textmesh = GetComponent<TextMeshProUGUI>();

        Invoke("ChangeText", delay);
    }

    void ChangeText()
    {
        current++;
        if (current == text.Length) current = 0;

        textmesh.text = text[current];

        Invoke("ChangeText", delay);
    }
}
