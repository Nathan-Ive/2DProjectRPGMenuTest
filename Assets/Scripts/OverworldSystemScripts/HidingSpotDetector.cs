using UnityEngine;

public class HidingSpotDetector : MonoBehaviour
{
    public PlayerOverworldActions playerActions;

    void OnTriggerEnter2D(Collider2D other)
    {
        HidingSpot spot = other.GetComponent<HidingSpot>();
        if (spot != null)
        {
            playerActions.SetNearbyHidingSpot(spot);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        HidingSpot spot = other.GetComponent<HidingSpot>();
        if (spot != null)
        {
            playerActions.SetNearbyHidingSpot(spot);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        HidingSpot spot = other.GetComponent<HidingSpot>();
        if (spot != null)
        {
            playerActions.ClearNearbyHidingSpot(spot);
        }
    }
}