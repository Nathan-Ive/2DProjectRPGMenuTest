using UnityEngine;
using UnityEngine.Events;

public class OverworldReload : MonoBehaviour
{
    [Header("References")]
    public InputManager inputManager;
    public PlayerMovement playerMovement;
    public AmmoManager ammoManager;

    [Header("Reload Settings")]
    public float baseReloadTime = 2f;
    public float stationaryMultiplier = 0.5f;

    [Header("Events")]
    public UnityEvent OnReloadStarted;
    public UnityEvent OnReloadComplete;

    private bool isReloading = false;
    private float reloadTimer = 0f;
    private float currentReloadDuration = 0f;

    void OnEnable()
    {
        inputManager.OnReadyPressed.AddListener(HandleReloadInput);
    }

    void OnDisable()
    {
        inputManager.OnReadyPressed.RemoveListener(HandleReloadInput);
    }

    void HandleReloadInput()
    {
        if (isReloading || !ammoManager.CanReload())
            return;

        StartReload();
    }

    public void StartReloadCooldown()
    {
        // Called after shooting in overworld
        // Applies the movement penalty briefly
        playerMovement.SetReloading(true);
        Invoke("EndReloadCooldown", 0.5f);
    }

    void EndReloadCooldown()
    {
        playerMovement.SetReloading(false);
    }

    void StartReload()
    {
        isReloading = true;
        reloadTimer = 0f;
        currentReloadDuration = baseReloadTime;

        playerMovement.SetReloading(true);
        OnReloadStarted?.Invoke();
    }

    void Update()
    {
        if (!isReloading)
            return;

        float speedMultiplier = 1f;
        string state = playerMovement.GetCurrentState();

        if (state == "hiding" || state == "reloading")
        {
            // Check if player is standing still
            speedMultiplier = stationaryMultiplier;
        }

        reloadTimer += Time.deltaTime * (1f / speedMultiplier);

        if (reloadTimer >= baseReloadTime)
        {
            CompleteReload();
        }
    }

    void CompleteReload()
    {
        ammoManager.ReloadOne();
        isReloading = false;

        playerMovement.SetReloading(false);
        OnReloadComplete?.Invoke();
    }

    public bool IsReloading()
    {
        return isReloading;
    }
}

