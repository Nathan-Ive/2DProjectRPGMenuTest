using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed Settings")]
    public float walkSpeed = 5f;
    public float sneakMultiplier = 0.5f;
    public float runMultiplier = 1.5f;
    public float reloadMultiplier = 0.5f;

    [Header("Input")]
    public InputManager inputManager;

    [Header("Events")]
    public UnityEvent<string> OnMovementStateChanged;

    private Vector2Int currentDirection = Vector2Int.zero;
    private bool isSneaking = false;
    private bool isRunning = false;
    private bool isReloading = false;
    private bool isHiding = false;
    private bool isEnabled = true;

    void OnEnable()
    {
        inputManager.OnDirectionPressed.AddListener(HandleDirectionPressed);
        inputManager.OnDirectionHeld.AddListener(HandleDirectionHeld);
        inputManager.OnSneakHeld.AddListener(HandleSneakHeld);
        inputManager.OnCancelHeld.AddListener(HandleRunHeld);
    }

    void OnDisable()
    {
        inputManager.OnDirectionPressed.RemoveListener(HandleDirectionPressed);
        inputManager.OnDirectionHeld.RemoveListener(HandleDirectionHeld);
        inputManager.OnSneakHeld.RemoveListener(HandleSneakHeld);
        inputManager.OnCancelHeld.RemoveListener(HandleRunHeld);
    }

    void HandleDirectionPressed(Vector2Int direction)
    {
        if (!isEnabled || isHiding)
            return;

        currentDirection = direction;
        MoveInDirection(direction);
    }

    void HandleDirectionHeld(Vector2Int direction)
    {
        if (!isEnabled || isHiding)
            return;

        currentDirection = direction;
        MoveInDirection(direction);
    }

    void MoveInDirection(Vector2Int direction)
    {
        float speed = GetCurrentSpeed();
        Vector3 movement = new Vector3(direction.x, direction.y, 0f) * speed * Time.deltaTime;
        transform.position += movement;
    }

    float GetCurrentSpeed()
    {
        float speed = walkSpeed;

        if (isReloading)
        {
            return walkSpeed * reloadMultiplier;
        }

        if (isSneaking)
        {
            speed = walkSpeed * sneakMultiplier;
        }
        else if (isRunning)
        {
            speed = walkSpeed * runMultiplier;
        }

        return speed;
    }

    void HandleSneakHeld(/* no parameters */)
    {
        if (!isEnabled || isHiding)
            return;

        isSneaking = true;
    }

    void HandleRunHeld(/* no parameters */)
    {
        if (!isEnabled || isHiding)
            return;

        isRunning = true;
    }

    void LateUpdate()
    {
        string state = "walk";

        if (isHiding)
            state = "hiding";
        else if (isReloading)
            state = "reloading";
        else if (isSneaking)
            state = "sneaking";
        else if (isRunning)
            state = "running";

        OnMovementStateChanged?.Invoke(state);

        isSneaking = false;
        isRunning = false;
    }

    public void SetHiding(bool hiding)
    {
        isHiding = hiding;
    }

    public void SetReloading(bool reloading)
    {
        isReloading = reloading;
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
    }

    public Vector2Int GetFacingDirection()
    {
        return currentDirection;
    }

    public string GetCurrentState()
    {
        if (isHiding) return "hiding";
        if (isReloading) return "reloading";
        if (isSneaking) return "sneaking";
        if (isRunning) return "running";
        return "walk";
    }
}

