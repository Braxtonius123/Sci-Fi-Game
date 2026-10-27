using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float bulletDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, bulletDuration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Assuming the enemy has a method called TakeDamage that takes the damage as a parameter
            other.GetComponent<EnemyHealth>().TakeDamage(damage);
            Debug.Log("Bullet hit an enemy");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Assuming the enemy has a method called TakeDamage that takes the damage as a parameter
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
            Debug.Log("Bullet hit an enemy");
        }
    }
}
