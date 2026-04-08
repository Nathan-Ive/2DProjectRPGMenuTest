using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent OnMenuOpened;
    public UnityEvent OnAllMenusClosed;

    private Stack<MenuBox> menuStack = new Stack<MenuBox>();

    public void OpenMenu(MenuBox menu)
    {
        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetFocused(false);
        }
        else
        {
            OnMenuOpened?.Invoke();
        }

        menu.gameObject.SetActive(true);
        menu.enabled = true;
        menu.SetFocused(true);

        menu.OnCancel.AddListener(() => CancelFocusedMenu());
        menu.OnNavigateToParent.AddListener(() => FocusParent());
        menuStack.Push(menu);
    }

    public void CloseCurrentMenu()
    {
        if (menuStack.Count == 0)
            return;

        MenuBox current = menuStack.Pop();
        current.OnCancel.RemoveAllListeners();
        current.OnNavigateToParent.RemoveAllListeners();
        current.SetFocused(false);
        current.gameObject.SetActive(false);

        if (menuStack.Count > 0)
        {
            menuStack.Peek().SetFocused(true);
        }
        else
        {
            OnAllMenusClosed?.Invoke();
        }
    }

    public void CloseAllMenus()
    {
        while (menuStack.Count > 0)
        {
            MenuBox current = menuStack.Pop();
            current.OnCancel.RemoveAllListeners();
            current.OnNavigateToParent.RemoveAllListeners();
            current.SetFocused(false);
            current.gameObject.SetActive(false);
        }

        OnAllMenusClosed?.Invoke();
    }

    public void FocusParent()
    {
        if (menuStack.Count <= 1)
            return;

        menuStack.Peek().SetFocused(false);

        MenuBox[] menus = menuStack.ToArray();
        menus[1].SetFocused(true);
    }

    public void FocusChild()
    {
        if (menuStack.Count <= 1)
            return;

        MenuBox[] menus = menuStack.ToArray();
        menus[1].SetFocused(false);

        menuStack.Peek().SetFocused(true);
    }

    public void PauseCurrentMenu()
    {
        if (menuStack.Count > 0)
            menuStack.Peek().SetFocused(false);
    }

    public void ResumeCurrentMenu()
    {
        if (menuStack.Count > 0)
            menuStack.Peek().SetFocused(true);
    }

    public bool IsMenuOpen()
    {
        return menuStack.Count > 0;
    }

    public MenuBox GetCurrentMenu()
    {
        if (menuStack.Count > 0)
            return menuStack.Peek();
        return null;
    }

    public void CancelFocusedMenu()
    {
        MenuBox focused = null;
        MenuBox[] menus = menuStack.ToArray();

        foreach (MenuBox menu in menus)
        {
            if (menu.IsFocused())
            {
                focused = menu;
                break;
            }
        }

        if (focused == null)
            return;

        if (focused == menuStack.Peek())
        {
            CloseCurrentMenu();
        }
        else
        {
            CloseAllMenus();
        }
    }

}