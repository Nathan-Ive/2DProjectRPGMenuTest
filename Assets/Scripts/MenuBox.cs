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

    private int currentIndex = 0;
    private Vector2Int currentHeldDirection = Vector2Int.zero;
    private float repeatTimer = 0f;
    private bool isRepeating = false;

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
        MoveCursor(direction);
        currentHeldDirection = direction;
        repeatTimer = 0f;
        isRepeating = false;
    }

    void HandleDirectionHeld(Vector2Int direction)
    {
        if (direction != currentHeldDirection)
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

    void MoveCursor(Vector2Int direction)
    {
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
        options[currentIndex].OnConfirm?.Invoke();
    }

    void HandleCancel()
    {
        OnCancel?.Invoke();
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }
}