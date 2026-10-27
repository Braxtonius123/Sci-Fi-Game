using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public GameObject Player;

    public EnemyWeaponSystem ews;

    public LayerMask whatIsGround, whatIsPlayer;

    //public float health;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // States
    public float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        UpdateTarget();
        agent = GetComponent<NavMeshAgent>();
        ews = GetComponent<EnemyWeaponSystem>();
    }

    private void Update()
    {
        UpdateTarget();
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
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

    private void ChasePlayer()
    {
        agent.SetDestination(Player.transform.position);
    }

    private void AttackPlayer()
    {
        //agent.SetDestination(transform.position);
        transform.LookAt(Player.transform);
        if (ews.readyToShoot)
            ews.Shoot(Player);
    }

    public void UpdateTarget()
    {
        // Find the closest player
        List<GameObject> players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        players.AddRange(GameObject.FindGameObjectsWithTag("Ally"));
        float closestDistance = Mathf.Infinity;
        GameObject closestPlayer = null;

        foreach (GameObject i in players)
        {
            float distance = Vector3.Distance(transform.position, i.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                if (i != null)
                    closestPlayer = i;
            }
        }
        if (closestPlayer != null)
            Player = closestPlayer;
        else
            Patroling();
        //Debug.Log("Enemy updated target: " + Player);
    }

    /*public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            DestroyEnemy();
        Debug.Log("Enemy took damage. Remaining health: " + health);
    }

    private void DestroyEnemy()
    {
        gameObject.SetActive(false);
    }*/

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
}
