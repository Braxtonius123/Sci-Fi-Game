using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class DodgeEnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public EnemyWeaponSystem ews;

    private EnemyHealth enhl;
    private Rigidbody rb;

    public string[] enemyTags;
    private GameObject Enemy;
    public LayerMask whatIsGround, whatIsEnemy, whatIsAlly;

    public enum State
    {
        Patroling,
        Chasing,
        Attacking,
        TakingCover,
        Dodging
    }
    private State currentState;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // States
    public float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

    // Dodging
    public float maxDodgeCooldown, dodgeRange;
    private float dodgeCooldown;
    private bool dodging;

    private void Awake()
    {
        UpdateTarget();
        agent = GetComponent<NavMeshAgent>();
        ews = GetComponent<EnemyWeaponSystem>();
        enhl = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateTarget();
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsEnemy);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsEnemy);
        if (!playerInSightRange && !playerInAttackRange) currentState = State.Patroling;//Patroling();
        if (playerInSightRange && !playerInAttackRange) currentState = State.Chasing;//ChasePlayer();
        if (playerInAttackRange && playerInSightRange) currentState = State.Attacking;//AttackPlayer();
        // Taking cover or dodging
        if (enhl.health < enhl.maxHealth * 0.5f)
        {
            if (Vector3.Distance(transform.position, FindClosestCover()) < 1)
                currentState = State.TakingCover;//agent.SetDestination(FindClosestCover());
            //else
                //currentState = State.Dodging;//Dodge(transform.position, 5, 3);
        }

        switch (currentState)
        {
            case State.Patroling:
                Patroling();
                break;
            case State.Chasing:
                ChaseEnemy();
                break;
            case State.Attacking:
                AttackEnemy();
                break;
            case State.TakingCover:
                agent.SetDestination(FindClosestCover());
                break;
            //case State.Dodging:
                //if (!dodging)
                    //Dodge(transform.position, 1);
                //break;
        }

        //if (dodging)
        //agent.speed = 7;
        //else
        //agent.speed = 3.5f;

        dodgeCooldown -= Time.deltaTime;
        if (dodgeCooldown <= 0)
            dodging = false;

        if (!dodging && playerInSightRange)
            Dodge(transform.position, dodgeRange);
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChaseEnemy()
    {
        agent.SetDestination(Enemy.transform.position);
    }

    private void AttackEnemy()
    {
        //agent.SetDestination(transform.position);
        transform.LookAt(Enemy.transform);
        if (ews.readyToShoot)
            ews.Shoot(Enemy);
        //if (!dodging)
            //Dodge(transform.position, dodgeRange);
    }

    private void Dodge(Vector3 returnPoint, float distance)
    {
        dodging = true;
        dodgeCooldown = maxDodgeCooldown;
        /*Vector3 distanceBetween = transform.position - Enemy.transform.position;
        Vector3 dodgeDirection = Vector3.Cross(distanceBetween, Vector3.up);
        Vector3 dodgeLocation = dodgeDirection * distance;
        dodgePos = dodgeLocation;
        agent.SetDestination(dodgeLocation);*/
        int direction = Random.Range(0, 2);
        Vector3 dodgeDirection = transform.right * distance;
        if (direction == 0)
        {
            dodgeDirection = transform.right * distance;
            rb.AddForce(dodgeDirection, ForceMode.Impulse);
            Debug.Log("Right");
        }
        else
        {
            dodgeDirection = -transform.right * distance;
            rb.AddForce(dodgeDirection, ForceMode.Impulse);
            Debug.Log("Left");
        }
        rb.AddForce(transform.forward * distance * 0.5f, ForceMode.Impulse);
    }

    public void UpdateTarget()
    {
        // Find the closest player
        List<GameObject> enemies = new List<GameObject>();
        foreach (string tag in enemyTags)
        {
            enemies.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject i in enemies)
        {
            float distance = Vector3.Distance(transform.position, i.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                if (i != null)
                    closestEnemy = i;
            }
        }
        if (closestEnemy != null)
            Enemy = closestEnemy;
        else
            Patroling();
    }

    public void ApplyExternalForce(Vector3 impactPoint, float force, float radius)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = transform.position - impactPoint;
            float distance = direction.magnitude;
            if (distance < radius)
            {
                float forceMagnitude = force * (1 - (distance / radius));
                rb.AddForce(direction.normalized * forceMagnitude, ForceMode.Impulse);
            }
        }
    }

    public Vector3 FindClosestCover()
    {
        // Find closest cover
        Vector3 coverPoint = new Vector3();

        Collider[] covers = Physics.OverlapSphere(transform.position, 1000, LayerMask.GetMask("Cover"));
        float closestDistance = Mathf.Infinity;
        Vector3 closestCover = Vector3.zero;

        foreach (Collider cover in covers)
        {
            float distance = Vector3.Distance(transform.position, cover.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCover = cover.transform.position;
            }
        }

        // Find spot behind cover to hide
        GameObject coverPointObject = new GameObject();
        Vector3 directionToCover = (closestCover - Enemy.transform.position).normalized;
        float distanceToCover = Vector3.Distance(Enemy.transform.position, closestCover);

        for (float i = 0; i < distanceToCover; i += 0.5f)
        {
            coverPointObject.transform.position = Enemy.transform.position + directionToCover * i;
            if (Vector3.Distance(coverPointObject.transform.position, closestCover) < 0.5f)
            {
                while (Physics.CheckSphere(coverPointObject.transform.position, 0.1f, LayerMask.GetMask("Cover")))
                {
                    coverPointObject.transform.position += directionToCover * 0.1f;
                }
                coverPoint = coverPointObject.transform.position;
                break;
            }
        }
        Destroy(coverPointObject);
        return coverPoint;
    }
}
