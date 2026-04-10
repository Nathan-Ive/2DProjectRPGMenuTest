using UnityEngine;
using UnityEngine.Events;

public enum DetectionType
{
    Grounded,
    Flying,
    Predator
}

public class EnemyDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    public DetectionType detectionType = DetectionType.Grounded;

    [Header("Grounded Settings")]
    public float groundedRange = 8f;
    public float groundedAngle = 60f;

    [Header("Flying Settings")]
    public float flyingRange = 6f;
    public float flyingFrontBias = 1.3f;

    [Header("Predator Settings")]
    public float predatorRange = 10f;
    public float predatorAngle = 30f;

    [Header("Events")]
    public UnityEvent<Transform> OnPlayerDetected;
    public UnityEvent OnPlayerLost;

    private Transform player;
    private bool playerDetected = false;
    private Vector2 facingDirection = Vector2.down;

    void Update()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                return;
        }

        bool canSee = CheckDetection();

        if (canSee && !playerDetected)
        {
            playerDetected = true;
            OnPlayerDetected?.Invoke(player);
        }
        else if (!canSee && playerDetected)
        {
            playerDetected = false;
            OnPlayerLost?.Invoke();
        }
    }

    bool CheckDetection()
    {
        PlayerDetectionHitbox hitbox = player.GetComponent<PlayerDetectionHitbox>();
        if (hitbox != null && !hitbox.detectionCollider.enabled)
            return false;

        Vector2 toPlayer = (Vector2)(player.position - transform.position);
        float distance = toPlayer.magnitude;

        switch (detectionType)
        {
            case DetectionType.Grounded:
                return CheckConeDetection(toPlayer, distance, groundedRange, groundedAngle);

            case DetectionType.Flying:
                return CheckFlyingDetection(toPlayer, distance);

            case DetectionType.Predator:
                return CheckConeDetection(toPlayer, distance, predatorRange, predatorAngle);

            default:
                return false;
        }
    }

    bool CheckConeDetection(Vector2 toPlayer, float distance, float range, float angle)
    {
        if (distance > range)
            return false;

        float angleBetween = Vector2.Angle(facingDirection, toPlayer);
        return angleBetween <= angle / 2f;
    }

    bool CheckFlyingDetection(Vector2 toPlayer, float distance)
    {
        float effectiveRange = flyingRange;

        float angleBetween = Vector2.Angle(facingDirection, toPlayer);
        if (angleBetween <= 45f)
        {
            effectiveRange = flyingRange * flyingFrontBias;
        }

        return distance <= effectiveRange;
    }

    public void SetFacingDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
            facingDirection = direction.normalized;
    }

    public bool IsPlayerDetected()
    {
        return playerDetected;
    }

    public Vector2 GetFacingDirection()
    {
        return facingDirection;
    }
}

