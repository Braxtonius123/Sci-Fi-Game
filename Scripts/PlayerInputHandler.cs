using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;


    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";


    [Header("Action Name References")]
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string rotation = "Rotation";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string dash = "Dash";
    [SerializeField] private string crouch = "Crouch";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string interact = "Interact";
    [SerializeField] private string melee = "Melee";
    // I changed the "Cycle Weapon" to use scale in order to let the player select each individual weapon rather than cycling through them.
    [SerializeField] private string cycleWeapon = "Cycle Weapon";
    [SerializeField] private string scrollWeapon = "Scroll Weapon";
    [SerializeField] private string primaryFire = "Primary Fire";
    [SerializeField] private string secondaryFire = "Secondary Fire";
    [SerializeField] private string reload = "Reload";
    [SerializeField] private string activateJetpack = "Activate Jetpack";
    [SerializeField] private string powerUp = "Power Up";
    [SerializeField] private string roster = "Roster";


    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction crouchAction;
    private InputAction sprintAction;
    private InputAction interactAction;
    private InputAction meleeAction;
    private InputAction cycleWeaponAction;
    private InputAction scrollWeaponAction;
    private InputAction primaryFireAction;
    private InputAction secondaryFireAction;
    private InputAction reloadAction;
    private InputAction activateJetpackAction;
    private InputAction powerUpAction;
    private InputAction rosterAction;


    public Vector2 MovementInput { get; private set; }
    public Vector2 RotationInput { get; private set; }
    public bool JumpTriggered { get; private set; }

    public bool DashTriggered;

    public bool CrouchTriggered;

    public bool SprintTriggered;

    public bool InteractTriggered;

    public bool MeleeTriggered;

    //public bool CycleWeaponTriggered;
    public float CycleWeaponTriggered;

    // ScrollWeaponTriggered is a pass-through value that will be used to scroll through the player's weapons. The value will be a float that will be used to determine the direction of the scroll. A positive value will scroll up, and a negative value will scroll down.
    public float ScrollWeaponTriggered;

    public bool PrimaryFireTriggered;

    public bool SecondaryFireTriggered;

    public bool ReloadTriggered;

    public bool ActivateJetpackTriggered;

    public bool PowerUpTriggered;

    public bool RosterTriggered;


    private void Awake()
    {
        InputActionMap mapReference = playerControls.FindActionMap(actionMapName);


        movementAction = mapReference.FindAction(movement);
        rotationAction = mapReference.FindAction(rotation);
        jumpAction = mapReference.FindAction(jump);
        dashAction = mapReference.FindAction(dash);
        crouchAction = mapReference.FindAction(crouch);
        sprintAction = mapReference.FindAction(sprint);
        interactAction = mapReference.FindAction(interact);
        meleeAction = mapReference.FindAction(melee);
        cycleWeaponAction = mapReference.FindAction(cycleWeapon);
        scrollWeaponAction = mapReference.FindAction(scrollWeapon);
        primaryFireAction = mapReference.FindAction(primaryFire);
        secondaryFireAction = mapReference.FindAction(secondaryFire);
        reloadAction = mapReference.FindAction(reload);
        activateJetpackAction = mapReference.FindAction(activateJetpack);
        powerUpAction = mapReference.FindAction(powerUp);
        rosterAction = mapReference.FindAction(roster);


        SubscribeActionValuesToInputEvents();
    }


    private void SubscribeActionValuesToInputEvents()
    {
        movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        movementAction.canceled += inputInfo => MovementInput = Vector2.zero;


        rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
        rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;


        jumpAction.performed += inputInfo => JumpTriggered = true;
        jumpAction.canceled += inputInfo => JumpTriggered = false;

        dashAction.performed += inputInfo => DashTriggered = true;
        dashAction.canceled += inputInfo => DashTriggered = false;

        crouchAction.performed += inputInfo => CrouchTriggered = true;
        crouchAction.canceled += inputInfo => CrouchTriggered = false;

        sprintAction.performed += inputInfo => SprintTriggered = true;
        sprintAction.canceled += inputInfo => SprintTriggered = false;

        interactAction.performed += inputInfo => InteractTriggered = true;
        interactAction.canceled += inputInfo => InteractTriggered = false;

        meleeAction.performed += inputInfo => MeleeTriggered = true;
        meleeAction.canceled += inputInfo => MeleeTriggered = false;

        cycleWeaponAction.performed += inputInfo => CycleWeaponTriggered = inputInfo.ReadValue<float>();

        scrollWeaponAction.performed += inputInfo => ScrollWeaponTriggered = inputInfo.ReadValue<float>();

        primaryFireAction.performed += inputInfo => PrimaryFireTriggered = true;
        primaryFireAction.canceled += inputInfo => PrimaryFireTriggered = false;

        secondaryFireAction.performed += inputInfo => SecondaryFireTriggered = true;
        secondaryFireAction.canceled += inputInfo => SecondaryFireTriggered = false;

        reloadAction.performed += inputInfo => ReloadTriggered = true;
        reloadAction.canceled += inputInfo => ReloadTriggered = false;

        activateJetpackAction.performed += inputInfo => ActivateJetpackTriggered = true;
        activateJetpackAction.canceled += inputInfo => ActivateJetpackTriggered = false;

        powerUpAction.performed += inputInfo => PowerUpTriggered = true;
        powerUpAction.canceled += inputInfo => PowerUpTriggered = false;

        rosterAction.performed += inputInfo => RosterTriggered = true;
        rosterAction.canceled += inputInfo => RosterTriggered = false;
    }


    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName).Enable();
    }


    private void OnDisable()
    {
        playerControls.FindActionMap(actionMapName).Disable();
    }
}
