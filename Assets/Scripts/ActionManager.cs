
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    public UnityEvent jump;
    public UnityEvent jumpHold;
    public UnityEvent<int> moveCheck;

    public void OnJumpHoldAction(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("JumpHold was started");
        else if (context.performed)
        {
            Debug.Log("JumpHold was performed");
            Debug.Log(context.duration);
            jumpHold.Invoke();
        }
        else if (context.canceled)
            Debug.Log("JumpHold was cancelled");
    }

    // called twice, when pressed and unpressed
    public void OnJumpAction(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("Jump was started");
        else if (context.performed)
        {
            jump.Invoke();
            Debug.Log("Jump was performed");
        }
        else if (context.canceled)
            Debug.Log("Jump was cancelled");

    }

    // called twice, when pressed and unpressed
    public void OnMoveAction(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            float inputValue = context.ReadValue<float>();

            if (Mathf.Abs(inputValue) > 0.1f) // Dead zone
            {
                int faceRight = inputValue > 0 ? 1 : -1;
                Debug.Log($"Moving: {faceRight}");
                moveCheck.Invoke(faceRight);
            }
            else
            {
                Debug.Log("Input below threshold");
                moveCheck.Invoke(0);
            }
        }
        else if (context.canceled)
        {
            Debug.Log("Move stopped");
            moveCheck.Invoke(0);
        }
    }
}
