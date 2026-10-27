using UnityEngine;
using TMPro;
using Unity.VisualScripting;


public class GunSystem : MonoBehaviour
{
    private LineRenderer lr;
    public GameObject Player;

    public enum GunType
    {
        HitScan,
        SphereCast,
        Projectile,
    }

    public GunType type;

    [Header("Shoot Speed Parameters")]
    public float timeBetweenMultiplier;
    public float timeBetweenShots, maxTimeBetweenShooting, minTimeBetweenShooting;
    public float calculatedTimeBetween;

    [Header("Other Parameters")]
    //Gun stats
    public GameObject Projectile;
    public float damage, bulletSpeed, bulletDuration, bulletSize, bonusDamage, spread, range, reloadTime;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools 
    public bool shooting, readyToShoot, reloading;
    private bool infiniteAmmo = false;


    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;


    //Graphics
    public GameObject muzzleFlash, bulletHoleGraphic;
    /*public CamShake camShake;
    public float camShakeMagnitude, camShakeDuration;*/
    public TextMeshProUGUI text;


    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
        calculatedTimeBetween = maxTimeBetweenShooting;
    }
    private void Update()
    {
        MyInput();
        
        //SetText
        text.SetText(bulletsLeft + " / " + magazineSize);
    }
    private void MyInput()
    {
        //if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        if (allowButtonHold && Input.GetKey(KeyCode.Mouse0))
        {
            shooting = true;
            if (calculatedTimeBetween > minTimeBetweenShooting)
                calculatedTimeBetween -= Time.deltaTime * timeBetweenMultiplier;
            if (calculatedTimeBetween < minTimeBetweenShooting)
                calculatedTimeBetween = minTimeBetweenShooting;
        }
        else if (allowButtonHold && !Input.GetKey(KeyCode.Mouse0))
        {
            shooting = false;
            calculatedTimeBetween = maxTimeBetweenShooting;
        }

        //else shooting = Input.GetKeyDown(KeyCode.Mouse0);
        else if (!allowButtonHold && Input.GetKeyDown(KeyCode.Mouse0))
        {
            shooting = true;
            //calculatedTimeBetween = maxTimeBetweenShooting;
        }
        else if (!allowButtonHold && !Input.GetKeyDown(KeyCode.Mouse0))
        {
            shooting = false;
            //calculatedTimeBetween = maxTimeBetweenShooting;
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        //Shoot
        if (readyToShoot && shooting && !reloading && (bulletsLeft > 0)) //|| (type == GunType.Plasma && !isOverheated)))
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }

        lr = GetComponent<LineRenderer>();
    }
    private void Shoot()
    {
        readyToShoot = false;

        if (GunType.HitScan == type)
        {
            //Calculate Direction with Spread
            Vector3 direction = fpsCam.transform.forward + new Vector3(Random.Range(-spread, spread) / 10, Random.Range(-spread, spread) / 10, 0);
            RaycastHit[] hits = Physics.RaycastAll(fpsCam.transform.position, direction, range, whatIsEnemy);
            foreach (var hit in hits)
            {
                Debug.Log(hit.collider.name);
                if (lr != null)
                {
                    lr.SetPosition(0, attackPoint.position);
                    lr.SetPosition(1, hit.point);
                }
                if (hit.collider.GetComponentInParent<EnemyHealth>() != null && hit.collider.CompareTag("WeakPoint"))
                {
                    hit.collider.GetComponentInParent<EnemyHealth>().TakeDamage(damage + bonusDamage);
                    //Debug.Log("Hp after headshot: " + hit.collider.GetComponentInParent<EnemyHealth>().health);
                }
                if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.GetComponent<EnemyHealth>().TakeDamage(damage);
                    //Debug.Log("Hp after: " + hit.collider.GetComponentInParent<EnemyHealth>().health);
                }
                //Debug.Log(hit.collider.GetComponent<EnemyHealth>().health);
            }

            GameObject flash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            Destroy(flash, 0.1f);

            if (!infiniteAmmo)
                bulletsLeft--;
            bulletsShot--;

            Invoke("ResetShot", calculatedTimeBetween);
            if (bulletsShot > 0 && bulletsLeft > 0)
                Invoke("Shoot", timeBetweenShots);
        }

        else if (GunType.SphereCast == type)
        {
            //Calculate Direction with Spread
            Vector3 direction = fpsCam.transform.forward + new Vector3(Random.Range(-spread, spread) / 10, Random.Range(-spread, spread) / 10, 0);
            RaycastHit[] hits = Physics.SphereCastAll(fpsCam.transform.position, bulletSize, direction, range, whatIsEnemy);
            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                    hit.collider.GetComponent<EnemyHealth>().TakeDamage(damage);
                if (hit.collider.GetComponentInParent<EnemyHealth>() != null && hit.collider.CompareTag("WeakPoint"))
                    hit.collider.GetComponentInParent<EnemyHealth>().TakeDamage(bonusDamage);
            }

            GameObject flash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            Destroy(flash, 0.1f);

            if (!infiniteAmmo)
                bulletsLeft--;
            bulletsShot--;

            Invoke("ResetShot", calculatedTimeBetween);
            if (bulletsShot > 0 && bulletsLeft > 0)
                Invoke("Shoot", timeBetweenShots);
        }


        else if (GunType.Projectile == type)
        {
            GameObject bullet = Instantiate(Projectile, attackPoint.position, Quaternion.identity);
            //Spread
            //bullet.GetComponent<Rigidbody>().linearVelocity = fpsCam.transform.forward + new Vector3(Random.Range(-spread, spread) / 10, Random.Range(-spread, spread) / 10, 0).normalized * bulletSpeed;
            //Try moving bullet forward with transform instead of rigidbody velocity
            bullet.transform.forward = fpsCam.transform.forward + new Vector3(Random.Range(-spread, spread) / 10, Random.Range(-spread, spread) / 10, 0).normalized;
            bullet.GetComponent<Rigidbody>().linearVelocity = bullet.transform.forward * bulletSpeed;
            if (bullet.GetComponent<Bullet>() != null)
            {
                bullet.GetComponent<Bullet>().damage = damage;
                bullet.GetComponent<Bullet>().bulletDuration = bulletDuration;
            }

            GameObject flash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            Destroy(flash, 0.1f);

            if (!infiniteAmmo)
                bulletsLeft--;
            bulletsShot--;

            Invoke("ResetShot", calculatedTimeBetween);
            if (bulletsShot > 0 && bulletsLeft > 0)
                Invoke("Shoot", timeBetweenShots);
        }

        //ShakeCamera
        //camShake.Shake(camShakeDuration, camShakeMagnitude);


        //Graphics
        //Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
        //GameObject flash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        //Destroy(flash, 0.1f);


        //bulletsLeft -= bulletsPerTap;
        //bulletsShot -= bulletsPerTap;


        //Invoke("ResetShot", timeBetweenShooting);


        //if (bulletsShot > 0 && bulletsLeft > 0)
        //Invoke("Shoot", timeBetweenShots);

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

    public void QuickDraw()
    {
        calculatedTimeBetween *= 0.25f;
        infiniteAmmo = true;
    }
}
