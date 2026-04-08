using UnityEngine;
using UnityEngine.Events;

public class PlayerOverworldActions : MonoBehaviour
{
    [Header("References")]
    public InputManager inputManager;
    public PlayerMovement playerMovement;
    public AmmoManager ammoManager;
    public OverworldReload overworldReload;

    [Header("Events")]
    public UnityEvent OnGunReadied;
    public UnityEvent OnGunUnreadied;
    public UnityEvent<GameObject> OnShotFiredAtEnemy;
    public UnityEvent<HidingSpot> OnEnteredHidingSpot;
    public UnityEvent OnLeftHidingSpot;

    private bool isGunReady = false;
    private HidingSpot currentHidingSpot = null;
    private HidingSpot nearbyHidingSpot = null;
    private bool isHiding = false;

    void OnEnable()
    {
        inputManager.OnConfirmPressed.AddListener(HandleConfirm);
        inputManager.OnCancelPressed.AddListener(HandleCancel);
        inputManager.OnReadyPressed.AddListener(HandleReady);
    }

    void OnDisable()
    {
        inputManager.OnConfirmPressed.RemoveListener(HandleConfirm);
        inputManager.OnCancelPressed.RemoveListener(HandleCancel);
        inputManager.OnReadyPressed.RemoveListener(HandleReady);
    }

    void HandleConfirm()
    {
        if (isHiding)
        {
            if (isGunReady && ammoManager.CanShoot())
            {
                ShootFromHiding();
            }
            return;
        }

        if (isGunReady && ammoManager.CanShoot())
        {
            ShootInOverworld();
            return;
        }

        Debug.Log("Confirm pressed. nearbyHidingSpot: " + nearbyHidingSpot + " isGunReady: " + isGunReady);

        if (nearbyHidingSpot != null && !isGunReady)
        {
            Debug.Log("Trying to enter. isOccupied: " + nearbyHidingSpot.IsOccupied());
            EnterHidingSpot();
        }
    }

    void HandleCancel()
    {
        
        if (isHiding)
        {
            LeaveHidingSpot();
            return;
        }

        if (isGunReady)
        {
            UnreadyGun();
        }
    }

    void HandleReady()
    {
        if (isHiding && !isGunReady)
        {
            ReadyGun();
            return;
        }

        if (isGunReady)
        {
            UnreadyGun();
        }
        else
        {
            ReadyGun();
        }
    }

    void ReadyGun()
    {
        isGunReady = true;
        OnGunReadied?.Invoke();
    }

    void UnreadyGun()
    {
        isGunReady = false;
        OnGunUnreadied?.Invoke();
    }

    void EnterHidingSpot()
    {
        if (nearbyHidingSpot.TryEnter(playerMovement))
        {
            isHiding = true;
            currentHidingSpot = nearbyHidingSpot;
            OnEnteredHidingSpot?.Invoke(currentHidingSpot);
        }
    }

    void LeaveHidingSpot()
    {
        if (currentHidingSpot != null)
        {
            currentHidingSpot.Leave();
            isHiding = false;
            currentHidingSpot = null;
            playerMovement.SetHiding(false);
            UnreadyGun();
            OnLeftHidingSpot?.Invoke();
        }
    }

    void ShootFromHiding()
    {
        ammoManager.Shoot();
        OnShotFiredAtEnemy?.Invoke(null);
    }

    void ShootInOverworld()
    {
        ammoManager.Shoot();
        overworldReload.StartReloadCooldown();
    }

    public void ForceLeaveHidingSpot()
    {
        if (isHiding && currentHidingSpot != null)
        {
            currentHidingSpot.ForceExit();
            isHiding = false;
            currentHidingSpot = null;
            playerMovement.SetHiding(false);
            UnreadyGun();
            OnLeftHidingSpot?.Invoke();
        }
    }

    public void SetNearbyHidingSpot(HidingSpot spot)
    {
        nearbyHidingSpot = spot;
    }

    public void ClearNearbyHidingSpot(HidingSpot spot)
    {
        if (nearbyHidingSpot == spot)
            nearbyHidingSpot = null;
    }

    public bool IsHiding()
    {
        return isHiding;
    }

    public bool IsGunReady()
    {
        return isGunReady;
    }

    public HidingSpot GetCurrentHidingSpot()
    {
        return currentHidingSpot;
    }
}

