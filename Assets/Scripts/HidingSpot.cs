using UnityEngine;
using UnityEngine.Events;

public class HidingSpot : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent OnPlayerHid;
    public UnityEvent OnPlayerLeft;

    private bool isOccupied = false;
    private PlayerMovement playerMovement;
    private bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerMovement = other.GetComponent<PlayerMovement>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerMovement = null;
        }
    }

    public bool TryEnter(PlayerMovement player)
    {
        if (isOccupied)
            return false;

        isOccupied = true;
        playerMovement = player;

        player.transform.position = transform.position;
        player.SetHiding(true);

        OnPlayerHid?.Invoke();
        return true;
    }

    public void ForceExit()
    {
        if (!isOccupied || playerMovement == null)
            return;

        isOccupied = false;
        playerMovement.SetHiding(false);
        playerMovement = null;

        OnPlayerLeft?.Invoke();
    }

    public void Leave()
    {
        if (!isOccupied || playerMovement == null)
            return;

        isOccupied = false;
        playerMovement.SetHiding(false);
        playerMovement = null;

        OnPlayerLeft?.Invoke();
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public bool IsPlayerInRange()
    {
        return playerInRange;
    }
}

