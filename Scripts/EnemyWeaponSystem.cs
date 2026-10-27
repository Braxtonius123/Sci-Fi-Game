using UnityEngine;

public class EnemyWeaponSystem : MonoBehaviour
{
    public enum GunType
    {
        HitScan,
        Projectile
    }

    public GunType type;

    //Gun stats
    public GameObject Projectile;
    public float damage, bulletSpeed, bulletDuration, bonusDamage, timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools 
    bool shooting, reloading;
    [HideInInspector] public bool readyToShoot;

    //Reference
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsPlayer;

    //Graphics
    //public GameObject muzzleFlash, bulletHoleGraphic;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        ControlShooting();
    }

    private void ControlShooting()
    {
        if (bulletsLeft < magazineSize && !reloading) Reload();

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
        }
    }

    public void Shoot(GameObject target)
    {
        readyToShoot = false;

        // Stop from moving
        //Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
        //transform.forward = directionToTarget;

        if (GunType.HitScan == type)
        {
            //Calculate Direction with Spread
            Vector3 direction = transform.forward + new Vector3(Random.Range(-spread, spread) / 10, Random.Range(-spread, spread) / 10, 0);
            /*RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, range, whatIsPlayer);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log($"Hit collider: {hit.collider.name} id:{hit.collider.GetInstanceID()} distance:{hit.distance}");
                    Debug.Log("Enemy hit player with hitscan");
                    hit.collider.GetComponent<PlayerHealth>().DamagePlayer(damage);
                }
                if (hit.collider.CompareTag("Ally"))
                {
                    if (hit.collider.CompareTag("WeakPoint"))
                        hit.collider.GetComponentInParent<AllyAI>().TakeDamage(bonusDamage);
                    else
                        hit.collider.GetComponent<AllyAI>().TakeDamage(damage);
                }
            }*/
            if (Physics.Raycast(transform.position, direction, out rayHit, range, whatIsPlayer))
            {
                if (rayHit.collider.CompareTag("Player"))
                {
                    Debug.Log("Enemy hit player with hitscan");
                    rayHit.collider.GetComponent<PlayerHealth>().DamagePlayer(damage);
                }
                if (rayHit.collider.CompareTag("Ally"))
                {
                    if (rayHit.collider.CompareTag("WeakPoint"))
                        rayHit.collider.GetComponentInParent<AllyAI>().TakeDamage(bonusDamage);
                    else
                        rayHit.collider.GetComponent<AllyAI>().TakeDamage(damage);
                }
            }


            //GameObject flash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            //Destroy(flash, 0.1f);

            if (target == null) return;

            bulletsLeft--;
            bulletsShot--;

            Debug.Log("Marine shot with hitscan");

            Invoke("ResetShot", timeBetweenShooting);
            if (bulletsShot > 0 && bulletsLeft > 0)
                Invoke("Shoot", timeBetweenShots);
        }

        else if (GunType.Projectile == type)
        {
            GameObject bullet = Instantiate(Projectile, attackPoint.position, Quaternion.identity);
            bullet.transform.forward = gameObject.transform.forward + new Vector3(Random.Range(-spread, spread) / 10, Random.Range(-spread, spread) / 10, 0).normalized;
            bullet.GetComponent<Rigidbody>().linearVelocity = bullet.transform.forward * bulletSpeed;
            if (bullet.GetComponent<Bullet>() != null)
            {
                bullet.GetComponent<Bullet>().damage = damage;
                bullet.GetComponent<Bullet>().bulletDuration = bulletDuration;
            }

            //GameObject flash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            //Destroy(flash, 0.1f);

            bulletsLeft--;
            bulletsShot--;

            Invoke("ResetShot", timeBetweenShooting);
            if (bulletsShot > 0 && bulletsLeft > 0)
                Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }
    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
