using TMPro;
using UnityEngine;

public class GunSecondary : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;
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
    public bool allowButtonHold, separateAmmo;
    int bulletsLeft, bulletsShot;


    //bools 
    bool shooting, readyToShoot, reloading;


    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;


    //Graphics
    public GameObject muzzleFlash, bulletHoleGraphic;
    /*public CamShake camShake;
    public float camShakeMagnitude, camShakeDuration;*/
    public TextMeshProUGUI primaryText;
    public TextMeshProUGUI secondaryText;


    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {
        MyInput();

        //SetText
        if (separateAmmo)
        {
            secondaryText.SetText(bulletsLeft + " / " + magazineSize);
        }
        else
        {
            // Deactivate secondary text if not using separate ammo
            if (secondaryText.gameObject.activeSelf)
                secondaryText.gameObject.SetActive(false);
            primaryText.SetText(bulletsLeft + " / " + magazineSize);
        }
    }
    private void MyInput()
    {
        //if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        // Modify shooting input to use PlayerInputHandler
        if (allowButtonHold) shooting = playerInputHandler.SecondaryFireTriggered;
        //else shooting = Input.GetKeyDown(KeyCode.Mouse0);
        else shooting = playerInputHandler.SecondaryFireTriggered; // Assuming you want to trigger on button press, not hold


        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();


        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
            if (!allowButtonHold)
                playerInputHandler.SecondaryFireTriggered = false; // Reset the trigger after shooting
        }
    }
    private void Shoot()
    {
        readyToShoot = false;

        if (GunType.HitScan == type)
        {
            //Spread
            //float x = Random.Range(-spread, spread) / 10;
            //float y = Random.Range(-spread, spread) / 10;

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

            GameObject flash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            Destroy(flash, 0.1f);

            bulletsLeft--;
            bulletsShot--;

            Invoke("ResetShot", timeBetweenShooting);
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

            bulletsLeft--;
            bulletsShot--;

            Invoke("ResetShot", timeBetweenShooting);
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
}
