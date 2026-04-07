using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class MenuBox : MonoBehaviour
{
    [Header("Menu Options")]
    public List<MenuOption> options;
    public int columns = 1;

    [Header("Input")]
    public InputManager inputManager;

    [Header("Repeat Settings")]
    public float repeatDelay = 0.4f;
    public float repeatRate = 0.1f;

    [Header("Events")]
    public UnityEvent OnCancel;
    public UnityEvent OnNavigateToParent;
    public UnityEvent OnNavigateToChild;

    private int currentIndex = 0;
    private Vector2Int currentHeldDirection = Vector2Int.zero;
    private float repeatTimer = 0f;
    private bool isRepeating = false;
    private bool isFocused = false;

    void OnEnable()
    {
        inputManager.OnDirectionPressed.AddListener(HandleDirectionPressed);
        inputManager.OnDirectionHeld.AddListener(HandleDirectionHeld);
        inputManager.OnConfirmPressed.AddListener(HandleConfirm);
        inputManager.OnCancelPressed.AddListener(HandleCancel);
        inputManager.OnEscapePressed.AddListener(HandleCancel);
    }

    void OnDisable()
    {
        inputManager.OnDirectionPressed.RemoveListener(HandleDirectionPressed);
        inputManager.OnDirectionHeld.RemoveListener(HandleDirectionHeld);
        inputManager.OnConfirmPressed.RemoveListener(HandleConfirm);
        inputManager.OnCancelPressed.RemoveListener(HandleCancel);
        inputManager.OnEscapePressed.RemoveListener(HandleCancel);
    }

    void HandleDirectionPressed(Vector2Int direction)
    {
        if (!isFocused)
            return;

        if (direction == Vector2Int.right && IsAtRightEdge())
        {
            OnNavigateToParent?.Invoke();
            return;
        }

        if (direction == Vector2Int.left && IsAtLeftEdge())
        {
            OnNavigateToChild?.Invoke();
            return;
        }

        MoveCursor(direction);
        currentHeldDirection = direction;
        repeatTimer = 0f;
        isRepeating = false;
    }

    void HandleDirectionHeld(Vector2Int direction)
    {
        if (!isFocused)
            return;

        if (direction != currentHeldDirection)
            return;

        if (direction == Vector2Int.right && IsAtRightEdge())
            return;

        if (direction == Vector2Int.left && IsAtLeftEdge())
            return;

        repeatTimer += Time.deltaTime;

        if (!isRepeating)
        {
            if (repeatTimer >= repeatDelay)
            {
                MoveCursor(direction);
                isRepeating = true;
                repeatTimer = 0f;
            }
        }
        else
        {
            if (repeatTimer >= repeatRate)
            {
                MoveCursor(direction);
                repeatTimer = 0f;
            }
        }
    }

    bool IsAtLeftEdge()
    {
        return currentIndex % columns == 0;
    }

    bool IsAtRightEdge()
    {
        return currentIndex % columns == columns - 1;
    }

    void MoveCursor(Vector2Int direction)
    {
        if (options.Count == 0)
            return;

        int currentColumn = currentIndex % columns;
        int currentRow = currentIndex / columns;
        int totalRows = Mathf.CeilToInt((float)options.Count / columns);

        if (direction == Vector2Int.up)
        {
            currentRow = (currentRow - 1 + totalRows) % totalRows;
        }
        else if (direction == Vector2Int.down)
        {
            currentRow = (currentRow + 1) % totalRows;
        }
        else if (direction == Vector2Int.left)
        {
            currentColumn = (currentColumn - 1 + columns) % columns;
        }
        else if (direction == Vector2Int.right)
        {
            currentColumn = (currentColumn + 1) % columns;
        }

        int newIndex = currentRow * columns + currentColumn;

        if (newIndex >= 0 && newIndex < options.Count)
            currentIndex = newIndex;
    }

    void HandleConfirm()
    {
        if (!isFocused)
            return;

        if (options.Count == 0)
            return;

        options[currentIndex].OnConfirm?.Invoke();
    }

    void HandleCancel()
    {
        if (!isFocused)
            return;

        OnCancel?.Invoke();
    }

    public void SetFocused(bool focused)
    {
        isFocused = focused;
    }

    public bool IsFocused()
    {
        return isFocused;
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }
}