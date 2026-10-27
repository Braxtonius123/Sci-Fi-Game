using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.AI;

public class AllyAI : MonoBehaviour
{
    [SerializeField] private string title;
    public enum State
    {
        Patroling,
        Following,
        Waiting,
        Chasing,
        EnteringVehicle,
        Attacking,
        TakingCover
    }
    private State currentState;

    public NavMeshAgent agent;

    public GameObject Enemy, Grenade;

    public AllyWeaponSystem aws;

    public LayerMask whatIsGround, whatIsEnemy, whatIsPlayer;

    private int grenadesLeft = 2, mySeat = 1;

    [SerializeField] private float health, maxHealth, meleeDamage, meleeRange, grenadeThrowForce, grenadeRange;
    private float meleeCooldown, grenadeCooldown;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet; public bool followsPlayer;
    public float walkPointRange;

    // States
    public float sightRange, attackRange;
    private bool enemyInSightRange, enemyInAttackRange, playerInSightRange,canShoot = true, inVehicle, updatedSeats;

    private void Awake()
    {
        UpdateTarget();
        agent = GetComponent<NavMeshAgent>();
        aws = GetComponent<AllyWeaponSystem>();
    }

    private void Update()
    {
        VehicleController vc = FindClosestVehicle().GetComponent<VehicleController>();
        UpdateTarget();

        enemyInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsEnemy);
        enemyInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsEnemy);
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!enemyInSightRange && !enemyInAttackRange && canShoot)
            currentState = State.Patroling;
        if (followsPlayer && playerInSightRange && Vector3.Distance(GameObject.Find("Player").transform.position, transform.position) >= 5 && !enemyInSightRange && followsPlayer && canShoot)
            currentState = State.Following;
        if (followsPlayer && !enemyInSightRange && Vector3.Distance(GameObject.Find("Player").transform.position, transform.position) < 5)
            currentState = State.Waiting;
        else if (enemyInSightRange && !enemyInAttackRange && canShoot)
            currentState = State.Chasing;
        if (Vector3.Distance(transform.position, FindClosestVehicle().transform.position) < 20 && vc.currentSeats < vc.maxSeats && vc.playerInCar && canShoot)
        {
            agent.SetDestination(FindClosestVehicle().transform.position);
            if (Vector3.Distance(transform.position, FindClosestVehicle().transform.position) < 5 && vc.currentSeats < vc.maxSeats && vc.playerInCar && canShoot)
            {
                currentState = State.EnteringVehicle;
            }
        }
        else if (!vc.playerInCar && inVehicle)
            EnterVehicle(false);
        /*if (Vector3.Distance(transform.position, FindClosestVehicle().transform.position) < 5 && vc.currentSeats < vc.maxSeats && vc.playerInCar && canShoot)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
                currentState = State.EnteringVehicle;
        }*/
        else if (enemyInAttackRange && enemyInSightRange && canShoot)
            currentState = State.Attacking;
        if (health <= maxHealth / 4)
        {
            currentState = State.TakingCover;
            canShoot = false;
        }
        switch (currentState)
        {
            case State.Patroling:
                Patroling();
                break;
            case State.Following:
                Following();
                break;
            case State.Waiting:
                Wait();
                break;
            case State.Chasing:
                ChaseEnemy();
                break;
            case State.EnteringVehicle:
                EnterVehicle(true);
                break;
            case State.Attacking:
                AttackEnemy();
                break;
            case State.TakingCover:
                agent.SetDestination(FindClosestCover());
                break;
        }
        // Regenerate health over time
        if (health < maxHealth)
        {
            health += 5 * Time.deltaTime; // Regenerate 5 health per second
            if (health > maxHealth)
                health = maxHealth;
        }
        if (health >= maxHealth * 0.75)
        {
            canShoot = true;
        }

        if (inVehicle)
        {
            agent.isStopped = true;
            if (!updatedSeats)
            {
                vc.currentSeats++;
                mySeat = vc.currentSeats;
                updatedSeats = true;
            }
            Debug.Log("Marine is in seat number: " + vc.currentSeats);
            Vector3 seatPos = vc.seats[mySeat - 1].position;
            transform.position = seatPos;
        }
        else
        {
            agent.isStopped = false;
            if (updatedSeats)
            {
                vc.currentSeats--;
                updatedSeats = false;
            }
        }

        if (meleeCooldown > 0)
            meleeCooldown -= Time.deltaTime;
        if (grenadeCooldown > 0)
            grenadeCooldown -= Time.deltaTime;

        Roster.Instance.AddTitle(title);
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

    private void Following()
    {
        agent.isStopped = false;
        agent.SetDestination(GameObject.Find("Player").transform.position);
    }

    private void Wait()
    {
        agent.isStopped = true;
    }

    private void ChaseEnemy()
    {
        agent.SetDestination(Enemy.transform.position);
    }

    private GameObject FindClosestVehicle()
    {
        GameObject[] vehicles = GameObject.FindGameObjectsWithTag("Vehicle");
        float closestDistance = Mathf.Infinity;
        GameObject closestVehicle = null;
        foreach (GameObject vehicle in vehicles)
        {
            float distance = Vector3.Distance(transform.position, vehicle.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestVehicle = vehicle;
            }
        }
        return closestVehicle;
    }

    private void EnterVehicle(bool enter)
    {
        inVehicle = enter;
    }

    private void AttackEnemy()
    {
        //agent.SetDestination(transform.position);
        transform.LookAt(Enemy.transform);

        int randomAttack = Random.Range(0, 5);
        if (randomAttack == 0 && meleeCooldown < 0 && Vector3.Distance(transform.position, Enemy.transform.position) <= meleeRange)
        {
            int shouldMelee = Random.Range(0, 2);
            if (shouldMelee == 0 && canShoot)
                HandleMelee();
            else
                meleeCooldown = 1;
        }
        else if (randomAttack < 2 && grenadeCooldown <= 0 && grenadesLeft > 0 && Vector3.Distance(transform.position, Enemy.transform.position) >= grenadeRange)
        {
            int shouldGrenade = Random.Range(0, 2);
            if (shouldGrenade == 0 && canShoot)
                HandleGrenade();
            else grenadeCooldown = 4;
        }
        else
        {
            if (canShoot)
                if (aws.readyToShoot)
                    aws.Shoot(Enemy);
        }
    }

    private void HandleMelee()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, meleeRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<EnemyHealth>().TakeDamage(meleeDamage);
                //Debug.Log("Marine performed melee attack on enemy");
            }
        }
        meleeCooldown = 1.0f; // Reset cooldown
    }

    private void HandleGrenade()
    {
        GameObject grenadeInstance = Instantiate(Grenade, transform.position + transform.forward, Quaternion.identity);
        Rigidbody grenadeRigidbody = grenadeInstance.GetComponent<Rigidbody>();
        grenadeRigidbody.AddForce(transform.forward * grenadeThrowForce, ForceMode.VelocityChange);
        grenadeCooldown = 4.0f;
        grenadesLeft--;
    }

    public void UpdateTarget()
    {
        // Find the closest enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
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
        //Debug.Log("Marine updated target: " + Enemy);
    }

    // Finds closest cover then finds the point on the cover that is furthest from the enemy and moves to it
    public Vector3 FindClosestCover()
    {
        // This is the point where the marine will move to position the cover between itself and the enemy
        Vector3 coverPoint = new Vector3();

        // OverlapSphere to find nearby cover objects
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
        Debug.Log("Marine found closest cover at: " + closestCover);

        // Create an empty GameObject, move it along a line from the enemy to the cover, when the GameObject is past the cover stop moving it and set the coverPoint to the position of the GameObject
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
        Debug.Log("Marine found cover point at: " + coverPoint);
        return coverPoint;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        //if (health <= maxHealth / 2)
        //{
            //agent.SetDestination(FindClosestCover());
            //canShoot = false;
        //}
        //else
        //{
            //Patroling();
        //}
        if (health <= 0)
            DestroyMarine();
        //Debug.Log("Marine took damage. Remaining health: " + health);
    }

    private void DestroyMarine()
    {
        gameObject.SetActive(false);
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
}