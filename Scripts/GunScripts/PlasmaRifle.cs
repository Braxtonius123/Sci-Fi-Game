using UnityEngine;
using TMPro;


public class PlasmaRifle : MonoBehaviour
{
    public PlayerInputHandler pih;

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
            // Cool down gun
            currentHeat -= heatDissipationRate * Time.deltaTime;
            // When cool, allow shooting
            if (currentHeat <= maxHeat * 0.5f)
                isOverheated = false;
        }

    }
    private void MyInput()
    {
        if (pih.PrimaryFireTriggered)
        {
            shooting = true;
            if (calculatedTimeBetween > minTimeBetweenShooting)
                calculatedTimeBetween -= Time.deltaTime * timeBetweenMultiplier;
            if (calculatedTimeBetween < minTimeBetweenShooting)
                calculatedTimeBetween = minTimeBetweenShooting;
        }
        else
        {
            shooting = false;
            calculatedTimeBetween = maxTimeBetweenShooting;
        }

        if (pih.ReloadTriggered && !reloading && currentHeat > 0 && !isOverheated) VentHeat();

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

        if (!isOverheated)
        {
            GameObject bullet = Instantiate(Projectile, attackPoint.position, Quaternion.identity);
            //Spread
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
}