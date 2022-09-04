using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodeEntry : MonoBehaviour
{
    public TextMeshProUGUI placeholder;
    public TMP_InputField input;
    public CreatureSelector selector;

    public string[] codes;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void OnTextChange(string text)
    {
        foreach (var code in codes)
        {
            if (text.ToLower() == code)
            {
                ManageCodes(text.ToLower());

                //Setting text to accepted
                input.text = "";
                input.interactable = false;
                placeholder.text = "Accepted";
                Invoke("ResetText", 1.5f);
            }
        }
    }

    void ManageCodes(string code)
    {
        if (code == codes[0]) selector.current = 5;
    }

    void ResetText() { placeholder.text = "Enter Code..."; input.interactable = true; }
}
