using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] Menu[] menus;
    string lastMenu = "Loading";

    [SerializeField] float loadingTime;
    float lastLoad;
    string menuToLoad;

    public void Awake()
    {
        Instance = this;
        lastMenu = "Loading";
    }

    public void OpenMenu(string menu)
    {
        if (lastMenu == "Loading")
        {
            StartCoroutine("LoadMenuDelay");
            menuToLoad = menu;
        }
        else
        {
            foreach (var _menu in menus)
            {
                if (menu == _menu.menuName) _menu.Open();
                else CloseMenu(_menu);
            }
        }


        lastMenu = menu;
        if (lastMenu == "Loading") lastLoad = Time.timeSinceLevelLoad;
    }

    IEnumerator LoadMenuDelay()
    {
        while (Time.timeSinceLevelLoad - lastLoad < loadingTime)
            yield return null;

        foreach (var _menu in menus)
        {
            if (menuToLoad == _menu.menuName) _menu.Open();
            else CloseMenu(_menu);
        }
    }

    public void OpenMenu(Menu menu)
    {
        foreach (var _menu in menus)
        {
            if (_menu.open) CloseMenu(_menu);
        }

        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
