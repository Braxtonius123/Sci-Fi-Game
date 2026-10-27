using UnityEngine;
using System.Collections;
using TMPro;
using Unity.VisualScripting;


public class PlasmaGunSystem : MonoBehaviour
{
    public GameObject Player;

    public enum GunType
    {
        HitScan,
        Projectile
    }

    public GunType type;

    [Header("Plasma Parameters")]
    // Plasma gun stats
    public float heatRate;
    public float maxHeat, heatDissipationRate, overheatCooldownTime, ventingTime;
    [SerializeField] private float currentHeat;
    public bool isOverheated;

    [Header("Shoot Speed Parameters")]
    public float timeBetweenMultiplier;
    public float timeBetweenShots, maxTimeBetweenShooting, minTimeBetweenShooting;
    public float calculatedTimeBetween;

    [Header("Other Parameters")]
    //Gun stats
    public GameObject Projectile;
    public float damage, bulletSpeed, bulletDuration, bonusDamage, spread, range, reloadTime;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    public int bulletsLeft, bulletsShot;

    //bools 
    public bool shooting, readyToShoot, reloading;

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
        text.SetText(bulletsLeft.ToString());

        // Overheat dissipation logic
        if (!isOverheated)
        {
            currentHeat -= heatDissipationRate * Time.deltaTime;
            if (currentHeat < 0)
                currentHeat = 0;
        }
        else
        {
            shooting = false;
            //currentHeat -= heatDissipationRate * Time.deltaTime;
            if (currentHeat <= maxHeat * 0.5f)
                isOverheated = false;
        }

    }
    private void MyInput()
    {
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
        }
        else if (!allowButtonHold && !Input.GetKeyDown(KeyCode.Mouse0))
        {
            shooting = false;
        }

        if (Input.GetKeyDown(KeyCode.R) && !reloading && currentHeat > 0 && !isOverheated) VentHeat();

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0 && !isOverheated)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
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
                if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                    hit.collider.GetComponent<EnemyHealth>().TakeDamage(damage);
                if (hit.collider.GetComponentInParent<EnemyHealth>() != null && hit.collider.CompareTag("WeakPoint"))
                    hit.collider.GetComponentInParent<EnemyHealth>().TakeDamage(bonusDamage);
            }

            //Debug.DrawRay(fpsCam.transform.position, direction, Color.red, 10f);

            GameObject flash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            Destroy(flash, 0.1f);

            bulletsLeft--;
            bulletsShot--;

            Invoke("ResetShot", calculatedTimeBetween);
            if (bulletsShot > 0 && bulletsLeft > 0)
                Invoke("Shoot", timeBetweenShots);
        }

        else if (GunType.Projectile == type && !isOverheated)
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

            currentHeat += heatRate;
            if (currentHeat >= maxHeat)
            {
                isOverheated = true;
                // Wait for overheatCooldownTime seconds and then use a coroutine to do currentHeat -= heatDissipationRate * Time.deltaTime
                Invoke("OverHeatCooldown", overheatCooldownTime);
            }

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
    private void VentHeat()
    {
        reloading = true;
        Invoke("VentingFinished", ventingTime);
    }
    private void VentingFinished()
    {
        currentHeat = 0;
        isOverheated = false;
        reloading = false;
    }
    private void OverHeatCooldown()
    {
        StartCoroutine(OverheatCooldownCoroutine());
    }

    // OverheatCooldownCoroutine 
    private IEnumerator OverheatCooldownCoroutine()
    {
        while (currentHeat > 0)
        {
            currentHeat -= heatDissipationRate * Time.deltaTime;
            yield return null;
        }
        isOverheated = false;
    }
}