using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [Header("Direction Events")]
    public UnityEvent<Vector2Int> OnDirectionPressed; //For individual inputs
    public UnityEvent<Vector2Int> OnDirectionHeld; //For continuous inputs

    [Header("Action Events")]
    public UnityEvent OnConfirmPressed; //Used in Menus/Combat for confirming choices. Used in overworld for entering hiding spots and shooting.
    public UnityEvent OnCancelPressed; //Used in Menus/Combat for 
    public UnityEvent OnCancelHeld; //Used in overworld for increasing movement speed and expanding the detection hitbox.
    public UnityEvent OnEscapePressed; //Used in Menus/Combat for the same as Cancel
    public UnityEvent OnReadyPressed; //Used in overworld for readying weapon so the player can attack with confirm.
    public UnityEvent OnSneakHeld; //Used in overworld for decreasing movement speed and shrinking the detection hitbox.

    void Update()
    {
        // Modifiers first - these set flags before movement happens
        if (Input.GetKey(KeyCode.LeftShift))
            OnSneakHeld?.Invoke();

        if (Input.GetKeyDown(KeyCode.X))
            OnCancelPressed?.Invoke();
        if (Input.GetKey(KeyCode.X) && !Input.GetKeyDown(KeyCode.X))
            OnCancelHeld?.Invoke();

        if (Input.GetKeyDown(KeyCode.R))
            OnReadyPressed?.Invoke();

        // Directions - after modifiers so speed flags are set
        if (Input.GetKeyDown(KeyCode.UpArrow))
            OnDirectionPressed?.Invoke(Vector2Int.up);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            OnDirectionPressed?.Invoke(Vector2Int.down);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            OnDirectionPressed?.Invoke(Vector2Int.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            OnDirectionPressed?.Invoke(Vector2Int.right);

        if (Input.GetKey(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.UpArrow))
            OnDirectionHeld?.Invoke(Vector2Int.up);
        else if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.DownArrow))
            OnDirectionHeld?.Invoke(Vector2Int.down);
        else if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.LeftArrow))
            OnDirectionHeld?.Invoke(Vector2Int.left);
        else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
            OnDirectionHeld?.Invoke(Vector2Int.right);

        // Confirm and Escape - after directions
        if (Input.GetKeyDown(KeyCode.Z))
            OnConfirmPressed?.Invoke();

        if (Input.GetKeyDown(KeyCode.Escape))
            OnEscapePressed?.Invoke();
    }
}

//The "?" is important, since it confirms whether or not something is null, and if it is, it won't call the Invoke if nothing is listening.
//Only remove the "?" if you want to test whether or not input functions when you have nothing to receive those inputs. And remember to put it back if you do.