using UnityEngine;

public class PauseController : MonoBehaviour
{
    [Header("References")]
    public InputManager inputManager;
    public MenuManager menuManager;
    public MenuBox pauseMenuBox;
    public GameObject pauseMenuPanel;
    public CombatManager combatManager;

    void OnEnable()
    {
        inputManager.OnEscapePressed.AddListener(HandleEscape);
        menuManager.OnAllMenusClosed.AddListener(HandleMenusClosed);
    }

    void OnDisable()
    {
        inputManager.OnEscapePressed.RemoveListener(HandleEscape);
        menuManager.OnAllMenusClosed.RemoveListener(HandleMenusClosed);
    }

    void HandleEscape()
    {
        if (!menuManager.IsMenuOpen() && !combatManager.IsBattleActive())
        {
            pauseMenuPanel.SetActive(true);
            menuManager.OpenMenu(pauseMenuBox);
            Time.timeScale = 0f;
        }
    }

    void HandleMenusClosed()
    {
        Time.timeScale = 1f;
    }
}