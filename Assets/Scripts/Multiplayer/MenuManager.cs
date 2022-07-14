using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages all the menus
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] Menu[] menus;

    //Singleton
    public void Awake()
    {
        Instance = this;
    }

    //Opens menu with string
    public void OpenMenu(string menu)
    {
        foreach (var _menu in menus)
        {
            if (menu == _menu.menuName) _menu.Open();
            else CloseMenu(_menu);
        }
    }

    //Opens menu with menu
    public void OpenMenu(Menu menu)
    {
        foreach (var _menu in menus)
        {
            if (_menu.open) CloseMenu(_menu);
        }

        menu.Open();
    }

    //Closes a menu
    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
