using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public float damage = 10f;
    public GameObject Player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.GetComponent<PlayerHealth>().DamagePlayer(damage);
        }
        if (collision.gameObject.CompareTag("Ally"))
        {
            collision.gameObject.GetComponent<AllyAI>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}