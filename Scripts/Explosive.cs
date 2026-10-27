using UnityEngine;

public class Explosive : MonoBehaviour
{
    public enum ExplosiveType
    {
        Countdown,
        Detonation,
        Touch
    }
    public ExplosiveType explosiveType;

    [Header("Countdown Parameters")]
    public float countdown;

    [Header("Detonation Parameters")]
    private GameObject Detonator;
    private PlayerInputHandler playerInputHandler;
    private GunManager gm;

    [Header("Touch Parameters")]
    // Specify what causes an explosion on contact
    public string[] explosionContacts;
    public float explosionDelay = 0.01f;

    //public float delay = 3f; // Time before explosion
    public float radius = 5f;
    public float force = 700f;
    public int damage;
    public GameObject ExplosionEffect; // Drag a Particle System prefab here

    bool hasExploded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //countdown = delay;
        Detonator = GameObject.Find("Detonator");
        playerInputHandler = GameObject.Find("PlayerInputHandler").GetComponent<PlayerInputHandler>();
        gm = GameObject.Find("Player").GetComponent<GunManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (explosiveType == ExplosiveType.Countdown)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f && !hasExploded)
            {
                Explode();
                hasExploded = true;
            }
        }

        if (explosiveType == ExplosiveType.Detonation && playerInputHandler.SecondaryFireTriggered && gm.currentGun.Equals(Detonator))
        {
            Explode();
        }
    }

    void Explode()
    {
        // Show Visuals
        Instantiate(ExplosionEffect, transform.position, transform.rotation);

        // Find nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in colliders)
        {
            // Add Explosion Force
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }

            // Damage Enemies
            if (nearbyObject.CompareTag("Enemy"))
            {
                nearbyObject.GetComponent<EnemyHealth>().TakeDamage(damage);
                Debug.Log("Explosion got enemy; enemy health: " + nearbyObject.GetComponent<EnemyHealth>().health);
            }
            if (nearbyObject.CompareTag("Ally"))
            {
                nearbyObject.GetComponent<AllyAI>().TakeDamage(damage);
            }
            if (nearbyObject.CompareTag("Player"))
            {
                nearbyObject.GetComponent<FirstPersonController>().ApplyExternalForce(transform.position, force / 500, radius);
            }
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (explosiveType == ExplosiveType.Touch)
        {
            foreach (string contactTag in explosionContacts)
            {
                if (collision.gameObject.CompareTag(contactTag))
                {
                    if (collision.gameObject.CompareTag("Enemy"))
                        Debug.Log("Rocket hit enemy; enemy health: " + collision.gameObject.GetComponent<EnemyHealth>().health);
                    //Explode();
                    Invoke("Explode", explosionDelay);
                    break;
                }
            }
        }
    }
}
