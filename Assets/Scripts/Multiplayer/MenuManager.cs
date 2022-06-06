using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField]
    Menu[] menus;

    public void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menu)
    {
        foreach (var _menu in menus)
        {
            if (menu == _menu.menuName) _menu.Open();
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
