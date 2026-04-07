using UnityEngine;

public class PauseController : MonoBehaviour
{
    [Header("References")]
    public InputManager inputManager;
    public MenuManager menuManager;
    public MenuBox pauseMenuBox;
    public GameObject pauseMenuPanel;

    void OnEnable()
    {
        inputManager.OnEscapePressed.AddListener(HandleEscape);
    }

    void OnDisable()
    {
        inputManager.OnEscapePressed.RemoveListener(HandleEscape);
    }

    void HandleEscape()
    {
        if (!menuManager.IsMenuOpen())
        {
            pauseMenuPanel.SetActive(true);
            menuManager.OpenMenu(pauseMenuBox);
        }
    }
}