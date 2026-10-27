using NUnit;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement Speeds")]
    [SerializeField] private float moveSpeed = 5.0f;
    [HideInInspector]public bool isWalking = false, canMove = true;
    [SerializeField] private float walkMultiplier = 0.5f;


    [Header("Jump Parameters")]
    private Vector3 dashDirection;
    [HideInInspector] public bool isClimbing, isDashing;
    [HideInInspector] public bool isGrounded, isCrouching;
    [SerializeField] private float jumpForce = 5.0f, dashForce;
    private float dashTime;
    [HideInInspector] public float gravityMultiplier = 1.0f;


    [Header("Melee Parameters")]
    [SerializeField] private float meleeRange = 2.0f;
    [SerializeField] private float meleeDamage = 25;
    [SerializeField] private float meleeCooldown = 1.0f;


    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;


    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;


    [HideInInspector] public Vector3 currentMovement;
    private float verticalRotation;
    //private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTriggered ? sprintMultiplier : 1);
    private float currentSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        if (canMove)
            HandleMovement();
        HandleRotation();
        HandleInteract();
        if (characterController.isGrounded || isClimbing)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (isDashing)
        {
            characterController.Move(dashDirection * dashForce * Time.deltaTime);
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                isDashing = false;
                dashTime = 0.2f; // Reset dash time for the next dash
            }
        }

        // Handle sprint toggling
        if (playerInputHandler.SprintTriggered)
        {
            isWalking = true;
        }
        if (playerInputHandler.SprintTriggered == false)
        {
            isWalking = false;
        }

        if (isWalking)
        {
            currentSpeed = moveSpeed * walkMultiplier;
        }
        else
        {
            currentSpeed = moveSpeed;
        }
    }


    public Vector3 CalculateWorldDirection()
    {
        Vector3 inputDirection = new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        return worldDirection.normalized;
    }


    private void HandleJumping()
    {
        if (isGrounded)
        {
            currentMovement.y = -0.5f;

            if (playerInputHandler.JumpTriggered)
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            // Bunny hopping
            currentMovement.x += CalculateWorldDirection().x * currentSpeed * Time.deltaTime;
            currentMovement.z += CalculateWorldDirection().z * currentSpeed * Time.deltaTime;
        }
    }

    private void HandleDashing()
    {
        if (playerInputHandler.DashTriggered)
        {
            isDashing = true;
            dashDirection = mainCamera.transform.forward;
            dashTime = 0.2f;
            playerInputHandler.DashTriggered = false;
        }
    }

    private void HandleCrouching()
    {
        if (playerInputHandler.CrouchTriggered)
            isCrouching = true;
        else
            isCrouching = false;

        if (isCrouching)
        {
            characterController.height = 1.0f; // Crouched height
            mainCamera.transform.localPosition = new Vector3(0, 0.5f, 0); // Lower the camera
        }
        else
        {
            characterController.height = 2.0f; // Standing height
            mainCamera.transform.localPosition = new Vector3(0, 1.0f, 0); // Raise the camera
        }
    }

    private void HandleInteract()
    {
        if (playerInputHandler.InteractTriggered)
        {
            // Call interact method on all nearby interactable objects
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f);
            foreach (var hitCollider in hitColliders)
            {
                IInteractable interactable = hitCollider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
                if (interactable == null)
                {
                    interactable = hitCollider.GetComponentInParent<IInteractable>();
                }
                if (interactable == null)
                {
                    interactable = hitCollider.GetComponentInChildren<IInteractable>();
                }
            }
            playerInputHandler.InteractTriggered = false;
        }
    }

    private void HandleMelee()
    {
        if (playerInputHandler.MeleeTriggered)
        {
            /*RaycastHit hit;
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, meleeRange))
            {
                Debug.Log("Melee hit enemy");
                if (hit.collider.GetComponent<Rigidbody>() != null)
                {
                     hit.collider.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * meleeDamage / 4, ForceMode.Impulse);
                }
                if (hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.GetComponent<EnemyHealth>().TakeDamage(meleeDamage);
                    Debug.Log("Melee damaged enemy");
                }
            }*/

            RaycastHit hit;
            if (Physics.SphereCast(mainCamera.transform.position, 1, mainCamera.transform.forward, out hit, meleeRange))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.GetComponent<EnemyHealth>().TakeDamage(meleeDamage);
                    Debug.Log("Melee damaged enemy");
                }
                Debug.Log("Melee hit enemy");
                if (hit.collider.GetComponent<Rigidbody>() != null)
                {
                     hit.collider.GetComponent<Rigidbody>().AddForce(mainCamera.transform.forward * meleeDamage / 4, ForceMode.Impulse);
                }
            }
            meleeCooldown = 1.0f; // Reset cooldown
            playerInputHandler.MeleeTriggered = false;
        }
    }

    public void GoBerserk(bool berserk)
    {
        if (berserk)
        {
            meleeDamage *= 6;
            meleeRange *= 2;
        }
        else
        {
            meleeDamage /= 6;
            meleeRange /= 2;
        }
    }
    public void GoHasty(bool hasty)
    {
        if (hasty)
        {
            moveSpeed *= 1.5f;
            jumpForce *= 1.5f;
        }
        else
        {
            moveSpeed /= 1.5f;
            jumpForce /= 1.5f;
        }
    }

    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * currentSpeed;
        currentMovement.z = worldDirection.z * currentSpeed;

        HandleJumping();
        HandleDashing();
        HandleCrouching();
        meleeCooldown -= Time.deltaTime;
        if (meleeCooldown < 0.0f)
            HandleMelee();
        characterController.Move(currentMovement * Time.deltaTime);
    }


    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }


    private void ApplyVerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }


    private void HandleRotation()
    {
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSensitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSensitivity;


        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }

    // Allow the player to be affected by external forces (like explosions) without breaking the character controller's movement
    public void ApplyExternalForce(Vector3 impactPoint, float force, float radius)
    {
         Vector3 forceDirection = transform.position - impactPoint;
        float distance = forceDirection.magnitude;
        if (distance < radius)
        {
            float forceMagnitude = force * (1 - (distance / radius));
            Vector3 forceVector = forceDirection.normalized * forceMagnitude;
            currentMovement += forceVector;
        }
    }
}
