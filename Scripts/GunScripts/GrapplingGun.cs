using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    public GameObject Player;
    private Transform tf;
    private FirstPersonController fpc;
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform attackPoint, fpsCam;
    [SerializeField] private float cooldown, range, grappleSpeed, accumulatedForce;
    private bool grappling, reachedPoint;

    void Awake()
    {
        tf = Player.GetComponent<Transform>();
        fpc = Player.GetComponent<FirstPersonController>();
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        cooldown -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && !reachedPoint && cooldown <= 0)
        {
            StartGrapple();
            accumulatedForce = 0f;
        }
        if (Input.GetMouseButtonUp(0) || reachedPoint)
        {
            StopGrapple();
        }

        if (Vector3.Distance(tf.position, grapplePoint) < 2)
        {
            Debug.Log("Reached grapple point");
            reachedPoint = true;
            cooldown = 0.25f;
        }
        else
            reachedPoint = false;

        if (grappling)
        {
            lr.positionCount = 2;
            Player.transform.position = Vector3.MoveTowards(Player.transform.position, grapplePoint, grappleSpeed * Time.deltaTime);
            // As the player moves towards the grapple point, we can accumulate force to simulate momentum
            accumulatedForce += grappleSpeed * Time.deltaTime / 100;
        }
        else
        {
            lr.positionCount = 0;
            // Decrease the accumulated force over time to simulate friction and air resistance
            accumulatedForce -= 0.25f * (grappleSpeed * Time.deltaTime);
            if (accumulatedForce > 0)
            {
                if (fpc.CalculateWorldDirection() != Vector3.zero)
                    tf.position += fpc.CalculateWorldDirection() * 5 * accumulatedForce * Time.deltaTime;
                else
                    tf.position += tf.forward.normalized * 5 * accumulatedForce * Time.deltaTime;//tf.forward * 10 * accumulatedForce * Time.deltaTime;
            }
        }
        if (fpc.isGrounded)
            accumulatedForce -= accumulatedForce * 2 * Time.deltaTime;
    }

    //Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    // Call when a grapple is started
    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.position, fpsCam.forward, out hit, range, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            grappling = true;
        }
    }

    void StopGrapple()
    {
        grappling = false;
        cooldown = 0.5f;
    }

    void DrawRope()
    {
        if (!grappling) return;
        lr.SetPosition(0, attackPoint.position);
        lr.SetPosition(1, grapplePoint);
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
