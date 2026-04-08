using UnityEngine;
using UnityEngine.Events;

public class AmmoManager : MonoBehaviour
{
    [Header("Gun Settings")]
    public string gunName;
    public int maxAmmo = 3;
    public int currentAmmo = 3;
    public int damagePerShot = 50;
    public float overworldReloadTime = 1.5f;

    [Header("Events")]
    public UnityEvent OnAmmoChanged;
    public UnityEvent OnReloadStarted;
    public UnityEvent OnReloadFinished;
    public UnityEvent OnOutOfAmmo;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool CanShoot()
    {
        return currentAmmo > 0;
    }

    public int Shoot()
    {
        if (currentAmmo <= 0)
        {
            OnOutOfAmmo?.Invoke();
            return 0;
        }

        currentAmmo--;
        OnAmmoChanged?.Invoke();
        return damagePerShot;
    }

    public bool CanReload()
    {
        return currentAmmo < maxAmmo;
    }

    public void ReloadOne()
    {
        if (currentAmmo < maxAmmo)
        {
            currentAmmo++;
            OnAmmoChanged?.Invoke();
        }
    }

    public void SetAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(amount, 0, maxAmmo);
        OnAmmoChanged?.Invoke();
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return maxAmmo;
    }
}