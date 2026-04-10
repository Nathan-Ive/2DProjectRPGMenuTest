using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public enum EnemyState
{
    Patrolling,
    Stationary,
    Chasing,
    Searching,
    Returning
}

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float searchSpeed = 1.5f;

    [Header("Patrol Waypoints")]
    public List<Transform> waypoints;
    public bool isStationary = false;

    [Header("Search Settings")]
    public float searchDuration = 5f;
    public float deAggroTime = 5f;
    public float hidingSpotCheckRadius = 5f;

    [Header("References")]
    public EnemyDetection detection;

    [Header("Events")]
    public UnityEvent<Transform> OnStartedChasing;
    public UnityEvent OnLostPlayer;
    public UnityEvent<HidingSpot> OnCheckingHidingSpot;
    public UnityEvent OnReturnedToPatrol;

    private EnemyState currentState = EnemyState.Patrolling;
    private int currentWaypointIndex = 0;
    private Transform chaseTarget;
    private Vector3 lastSeenPosition;
    private float searchTimer = 0f;
    private float deAggroTimer = 0f;
    private bool hasDeAggrod = false;

    private List<HidingSpot> checkedSpots = new List<HidingSpot>();
    private int searchLookIndex = 0;
    private float lookTimer = 0f;
    private Vector2[] searchDirections = new Vector2[]
    {
        Vector2.up, Vector2.right, Vector2.down, Vector2.left
    };

    void Start()
    {
        if (isStationary)
            currentState = EnemyState.Stationary;

        detection.OnPlayerDetected.AddListener(HandlePlayerDetected);
        detection.OnPlayerLost.AddListener(HandlePlayerLost);
    }

    void OnDestroy()
    {
        detection.OnPlayerDetected.RemoveListener(HandlePlayerDetected);
        detection.OnPlayerLost.RemoveListener(HandlePlayerLost);
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                UpdatePatrol();
                break;
            case EnemyState.Stationary:
                break;
            case EnemyState.Chasing:
                UpdateChase();
                break;
            case EnemyState.Searching:
                UpdateSearch();
                break;
            case EnemyState.Returning:
                UpdateReturn();
                break;
        }
    }

    void HandlePlayerDetected(Transform player)
    {
        chaseTarget = player;
        lastSeenPosition = player.position;
        currentState = EnemyState.Chasing;
        hasDeAggrod = false;
        deAggroTimer = 0f;

        OnStartedChasing?.Invoke(player);
    }

    void HandlePlayerLost()
    {
        hasDeAggrod = false;
        deAggroTimer = 0f;
    }

    void UpdatePatrol()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = (target.position - transform.position).normalized;

        transform.position += direction * patrolSpeed * Time.deltaTime;
        detection.SetFacingDirection(direction);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }
    }

    void UpdateChase()
    {
        if (chaseTarget == null)
        {
            StartSearching();
            return;
        }

        if (!detection.IsPlayerDetected())
        {
            deAggroTimer += Time.deltaTime;

            if (deAggroTimer >= deAggroTime)
            {
                hasDeAggrod = true;
                StartSearching();
                return;
            }
        }
        else
        {
            deAggroTimer = 0f;
            lastSeenPosition = chaseTarget.position;
        }

        Vector3 direction = (chaseTarget.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;
        detection.SetFacingDirection(direction);
    }

    void StartSearching()
    {
        currentState = EnemyState.Searching;
        searchTimer = 0f;
        searchLookIndex = 0;
        lookTimer = 0f;
        checkedSpots.Clear();

        OnLostPlayer?.Invoke();
    }

    void UpdateSearch()
    {
        searchTimer += Time.deltaTime;

        // Look around in all directions
        lookTimer += Time.deltaTime;
        if (lookTimer >= searchDuration / 4f)
        {
            lookTimer = 0f;
            searchLookIndex = (searchLookIndex + 1) % searchDirections.Length;
            detection.SetFacingDirection(searchDirections[searchLookIndex]);
        }

        // Move toward last seen position
        float distanceToLastSeen = Vector3.Distance(transform.position, lastSeenPosition);
        if (distanceToLastSeen > 0.5f)
        {
            Vector3 direction = (lastSeenPosition - transform.position).normalized;
            transform.position += direction * searchSpeed * Time.deltaTime;
        }

        // Check nearby hiding spots
        CheckNearbyHidingSpots();

        // If player is re-detected during search, chase again
        if (detection.IsPlayerDetected())
        {
            currentState = EnemyState.Chasing;
            deAggroTimer = 0f;
            return;
        }

        if (searchTimer >= searchDuration)
        {
            StartReturning();
        }
    }

    void CheckNearbyHidingSpots()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, hidingSpotCheckRadius);

        foreach (Collider2D col in colliders)
        {
            HidingSpot spot = col.GetComponent<HidingSpot>();
            if (spot != null && spot.IsOccupied() && !checkedSpots.Contains(spot))
            {
                checkedSpots.Add(spot);
                OnCheckingHidingSpot?.Invoke(spot);
                spot.ForceExit();
            }
        }
    }

    void StartReturning()
    {
        currentState = EnemyState.Returning;
    }

    void UpdateReturn()
    {
        if (isStationary)
        {
            // Return to original position (first waypoint or spawn)
            if (waypoints != null && waypoints.Count > 0)
            {
                Vector3 home = waypoints[0].position;
                Vector3 direction = (home - transform.position).normalized;
                transform.position += direction * patrolSpeed * Time.deltaTime;
                detection.SetFacingDirection(direction);

                if (Vector3.Distance(transform.position, home) < 0.1f)
                {
                    currentState = EnemyState.Stationary;
                    OnReturnedToPatrol?.Invoke();
                }
            }
            else
            {
                currentState = EnemyState.Stationary;
                OnReturnedToPatrol?.Invoke();
            }
        }
        else
        {
            // Return to nearest waypoint
            Transform nearest = GetNearestWaypoint();
            Vector3 direction = (nearest.position - transform.position).normalized;
            transform.position += direction * patrolSpeed * Time.deltaTime;
            detection.SetFacingDirection(direction);

            // Occasionally look around while returning
            lookTimer += Time.deltaTime;
            if (lookTimer >= 2f)
            {
                lookTimer = 0f;
                searchLookIndex = (searchLookIndex + 1) % searchDirections.Length;
                detection.SetFacingDirection(searchDirections[searchLookIndex]);
            }

            if (Vector3.Distance(transform.position, nearest.position) < 0.1f)
            {
                currentWaypointIndex = waypoints.IndexOf(nearest);
                currentState = EnemyState.Patrolling;
                OnReturnedToPatrol?.Invoke();
            }
        }
    }

    Transform GetNearestWaypoint()
    {
        Transform nearest = waypoints[0];
        float nearestDist = Vector3.Distance(transform.position, nearest.position);

        foreach (Transform wp in waypoints)
        {
            float dist = Vector3.Distance(transform.position, wp.position);
            if (dist < nearestDist)
            {
                nearest = wp;
                nearestDist = dist;
            }
        }

        return nearest;
    }

    public EnemyState GetCurrentState()
    {
        return currentState;
    }
}

