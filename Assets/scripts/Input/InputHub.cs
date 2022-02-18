using UnityEngine;
using UnityEngine.InputSystem;

public class InputHub : MonoBehaviour
{
    PlanetPlayerController planetPlayerController;
    PlanetPlayerController getPlanetPlayerController()
    {
        return planetPlayerController ? planetPlayerController : planetPlayerController = FindObjectOfType<PlanetPlayerController>();
    }

    CameraPivot cameraPivot;
    CameraPivot GetCameraPivot()
    {
        return cameraPivot ? cameraPivot : cameraPivot = FindObjectOfType<CameraPivot>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            getPlanetPlayerController().setTurbo(true);
        else if (context.phase == InputActionPhase.Canceled)
            getPlanetPlayerController().setTurbo(false);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            getPlanetPlayerController().triggerJump();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        getPlanetPlayerController().MoveVec = value;
        // print(value);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        GetCameraPivot().LookVec = value;
        // print(value);
    }
}