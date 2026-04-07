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
            menuStack.Peek().enabled = false;
        else
            OnMenuOpened?.Invoke();

        menu.gameObject.SetActive(true);
        menu.enabled = true;
        menu.OnCancel.AddListener(() => CloseCurrentMenu());
        menuStack.Push(menu);
    }

    public void CloseCurrentMenu()
    {
        if (menuStack.Count == 0)
            return;

        MenuBox current = menuStack.Pop();
        current.OnCancel.RemoveAllListeners();
        current.gameObject.SetActive(false);

        if (menuStack.Count > 0)
        {
            menuStack.Peek().enabled = true;
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
            current.gameObject.SetActive(false);
        }

        OnAllMenusClosed?.Invoke();
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


    public void PauseCurrentMenu()
    {
        if (menuStack.Count > 0)
            menuStack.Peek().enabled = false;
    }

    public void ResumeCurrentMenu()
    {
        if (menuStack.Count > 0)
            menuStack.Peek().enabled = true;
    }

}


