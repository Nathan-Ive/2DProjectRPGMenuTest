using UnityEngine;

public class PlayerDetectionHitbox : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public CircleCollider2D detectionCollider;

    [Header("Hitbox Sizes")]
    public float standardRadius = 0.4f;
    public float sneakMultiplier = 0.8f;
    public float runMultiplier = 1.1f;

    void Update()
    {
        string state = playerMovement.GetCurrentState();

        switch (state)
        {
            case "hiding":
                detectionCollider.enabled = false;
                break;
            case "sneaking":
                detectionCollider.enabled = true;
                detectionCollider.radius = standardRadius * sneakMultiplier;
                break;
            case "running":
                detectionCollider.enabled = true;
                detectionCollider.radius = standardRadius * runMultiplier;
                break;
            default:
                detectionCollider.enabled = true;
                detectionCollider.radius = standardRadius;
                break;
        }
    }
}

