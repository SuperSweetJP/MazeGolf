using System.IO.Pipes;
using UnityEngine;
using UnityEngine.InputSystem;
public class TouchManagerScript : MonoBehaviour
{
    private PlayerInput playerInput;

    private InputAction touchPressAction;
    private InputAction touchPositionAction;

    private Vector2 touchStartPosition;
    private Vector2 touchEndPosition;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];
    }

    private void OnEnable()
    {
        touchPressAction.performed += OnTouchStart;
        touchPressAction.canceled += OnTouchEnd;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= OnTouchStart;
        touchPressAction.canceled -= OnTouchEnd;
    }

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        // Read and store the touch start position
        touchStartPosition = touchPositionAction.ReadValue<Vector2>();
        Debug.Log($"Touch Start Position: {touchStartPosition}");
    }

    private void OnTouchEnd(InputAction.CallbackContext context)
    {
        // Read and store the touch end position
        touchEndPosition = touchPositionAction.ReadValue<Vector2>();
        Debug.Log($"Touch End Position: {touchEndPosition}");
    }
}
