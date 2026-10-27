using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float shield;
    [SerializeField] private float maxShield;
    [SerializeField] private int health = 10;
    [SerializeField] private int maxHealth = 10;
    private bool shieldBroken = false;
    [HideInInspector] public bool invincible = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DamagePlayer(10);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*public void DamagePlayer(float damage)
    {
        shield -= damage;
        damage = Mathf.Max(0, damage - shield); // Calculate remaining damage after shield is depleted
        if (shield <= 0)
        {
            shield = 0;
            shieldBroken = true;
        }
        if (shieldBroken)
        {
            health -= (int) damage;
            if (health <= 0)
            {
                health = 0;
                KillPlayer();
            }
        }
    }*/

    public void DamagePlayer(float damage)
    {
        if (invincible) return;
        // If there is shield, consume it first and compute remaining damage correctly
        if (shield > 0f)
        {
            float shieldBefore = shield;

            if (shieldBefore >= damage)
            {
                // Shield absorbs all damage
                shield = shieldBefore - damage;
                if (shield <= 0f)
                {
                    shield = 0f;
                    shieldBroken = true;
                }
            }
            else
            {
                // Shield is depleted; apply remaining damage to health
                float remaining = damage - shieldBefore;
                shield = 0f;
                shieldBroken = true;
                health -= Mathf.RoundToInt(remaining);
            }
        }
        else
        {
            // No shield, apply full damage to health
            health -= Mathf.RoundToInt(damage);
        }

        if (health <= 0)
        {
            health = 0;
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        //Destroy(GameObject);
    }
}
