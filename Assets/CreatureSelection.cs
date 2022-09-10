using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreatureSelection : MonoBehaviour
{
    public Creature.Diet chosenDiet;
    public float speed;
    public float minHeight, maxHeight;

    public GameObject dietObject, selectObject, infoObject;
    int current;

    RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChoseDiet(int d)
    {
        chosenDiet = (Creature.Diet)d;

        OpenSelect();
    }

    public void ChooseCreature(string name)
    {
        for (int i = 0; i < RoomManager.Instance.creatures.Length; i++)
        {
            if (name == RoomManager.Instance.creatures[i].name) current = i;
        }

        RoomManager.Instance.customProperties["Creature"] = current;
        PhotonNetwork.SetPlayerCustomProperties(RoomManager.Instance.customProperties);

        OpenInfo();
    }

    public void Slide(bool up)
    {
        if (up) StartCoroutine(SlideUp());
        else StartCoroutine(SlideDown());
    }

    //Slides Menu Down
    IEnumerator SlideDown()
    {
        while (transform.localPosition.y > minHeight)
        {
            rect.position = new Vector3(rect.position.x, rect.position.y - speed * Time.deltaTime, rect.position.z);
            yield return null;
        }
    }

    //Slides Menu Up
    IEnumerator SlideUp()
    {
        OpenDiet();

        while (transform.localPosition.y < maxHeight)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z);
            yield return null;
        }
    }

    public void OpenDiet() { OpenMenu(true, false, false); }
    public void OpenSelect() { OpenMenu(false, true, false); }
    public void OpenInfo() { OpenMenu(false, false, true); }

    public void OpenMenu(bool a, bool b, bool c)
    { dietObject.SetActive(a); selectObject.SetActive(b); infoObject.SetActive(c); }
}

