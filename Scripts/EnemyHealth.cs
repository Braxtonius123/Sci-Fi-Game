using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health, maxHealth;//, shield, maxShield, timeTillRecharge, rechargeRate;
    //public bool hasShield = true;
    //private bool currentlyDamaged, shieldBroken;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    /*void Update()
    {
        if (currentlyDamaged && hasShield)
        {
            timeTillRecharge -= Time.deltaTime;
            if (timeTillRecharge <= 0)
            {
                currentlyDamaged = false;
                timeTillRecharge = 2f; // Reset the timer for the next damage
            }
        }
        else if (hasShield)
        {
            if (shield < maxShield)
            {
                shield += rechargeRate * Time.deltaTime;
                if (shield > maxShield)
                {
                    shield = maxShield;
                }
            }
        }
    }*/

    public void TakeDamage(float damage)
    {
        /*health -= damage;
        if (health <= 0)
            DestroyEnemy();
        Debug.Log("Enemy took damage. Remaining health: " + health);*/
        /*if (hasShield)
        {
            shield -= damage;
            damage = Mathf.Max(0, damage - shield); // Calculate remaining damage after shield is depleted
            if (shield <= 0)
            {
                shield = 0;
                shieldBroken = true;
            }
        }
        if (shieldBroken || !hasShield)
        {
            health -= (int)damage;
            Debug.Log("Enemy took damage. Remaining health: " + health);
            if (health <= 0)
            {
                health = 0;
                DestroyEnemy();
            }
        }
        currentlyDamaged = true;*/
        health -= damage;
        if (health <= 0)
            DestroyEnemy();
    }

    private void DestroyEnemy()
    {
        gameObject.SetActive(false);
    }
}
