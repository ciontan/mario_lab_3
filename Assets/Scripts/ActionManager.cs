using UnityEngine;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    public PlayerInput playerInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    
    // triggered upon performed interaction (default successful press)
    public void OnJump()
    {
        Debug.Log("OnJump called");
        // TODO
    }

    // triggered upon 1D value change (default successful press and cancelled)
    public void OnMove(InputValue input)
    {
        if (input.Get() == null)
        {
            Debug.Log("Move released");
        }
        else
        {
            Debug.Log($"Move triggered, with value {input.Get()}"); // will return null when released
        }
        // TODO
    }

    // triggered upon performed interaction (custom successful hold)
    public void OnJumphold(InputValue value)
    {
        Debug.Log($"OnJumpHold performed with value {value.Get()}");
        // TODO

    }

}
