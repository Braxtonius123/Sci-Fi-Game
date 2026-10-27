using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset vehicleControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Vehicle";

    [Header("Action Name References")]
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string rotation = "Rotation";

    private InputAction movementAction;
    private InputAction rotationAction;

    public Vector2 MovementInput { get; private set; }
    public Vector2 RotationInput { get; private set; }

    private void Awake()
    {
        InputActionMap mapReference = vehicleControls.FindActionMap(actionMapName);


        movementAction = mapReference.FindAction(movement);
        rotationAction = mapReference.FindAction(rotation);

        SubscribeActionValuesToInputEvents();
    }

    private void SubscribeActionValuesToInputEvents()
    {
        movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        movementAction.canceled += inputInfo => MovementInput = Vector2.zero;

        rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
        rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;
    }

    private void OnEnable()
    {
        vehicleControls.FindActionMap(actionMapName).Enable();
    }

    private void OnDisable()
    {
        vehicleControls.FindActionMap(actionMapName).Disable();
    }
}
