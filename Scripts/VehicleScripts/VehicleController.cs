using UnityEngine;

public class VehicleController : MonoBehaviour, IInteractable
{
    private GameObject Player;
    private Rigidbody rb;
    public Transform[] seats;
    [SerializeField] private Transform driverSeat, gunnerTurret;
    [SerializeField] private VehicleInputHandler vih;
    [SerializeField] private float acceleration, maxSpeed, turnSpeed, currentVelocity;
    public int maxSeats; public int currentSeats;
    [HideInInspector] public bool playerInCar = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInCar)
        {
            HandleMovement();
            Player.transform.position = driverSeat.position;
        }
        // Update current velocity for collision damage calculations
        currentVelocity = rb.linearVelocity.magnitude;
    }

    void IInteractable.Interact()
    {
        playerInCar = !playerInCar;
        if (playerInCar)
        {
            EnterCar();
            Debug.Log("Player entered the car.");
        }
        else
        {
            ExitCar();
            Debug.Log("Player exited the car.");
        }
    }

    private void HandleMovement()
    {
        transform.forward = Player.transform.forward;

        float moveInput = vih.MovementInput.y;
        float turnInput = vih.RotationInput.x;

        // Move the car forward/backward
        rb.AddForce(transform.forward * moveInput * acceleration, ForceMode.Acceleration);
        // Turn the car left/right
        transform.Rotate(Vector3.right, turnInput * turnSpeed * Time.deltaTime);
    }

    private void EnterCar()
    {
        Player.GetComponent<FirstPersonController>().canMove = false;
        currentSeats++;
    }

    private void ExitCar()
    {
        Player.GetComponent<FirstPersonController>().canMove = true;
        currentSeats--;
        //Player.transform.position = driverSeat.position - transform.right * 2;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Splatter the enemies!!!
        if (collision.gameObject.CompareTag("Enemy") && currentVelocity > maxSpeed * 0.5f)
        {
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(5 * (int) currentVelocity);
        }
    }
}
