using TMPro;
using UnityEngine;

public class AssaultRifle : MonoBehaviour, IGun
{
    public PlayerInputHandler pih;

    [Header("Shoot Speed Parameters")]
    public float timeBetweenShots, timeBetweenShooting;

    [Header("Primary Parameters")]
    //Gun stats
    public float damage, bonusDamage, spread, range, reloadTime;
    public int magazineSize, bulletsPerTap;
    int bulletsLeft, bulletsShot;

    // Bools 
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

    [Header("Secondary Parameters")]
    public GameObject Grenade;
    public float grenadeForce, timeBetweenSecondaryFire;

    // Bools
    public bool grenading, readyToGrenade;

    private void Awake()
    {
        pih = GameObject.Find("PlayerInputHandler").GetComponent<PlayerInputHandler>();
        bulletsLeft = magazineSize;
        readyToShoot = true;
        readyToGrenade = true;
    }
    private void Update()
    {
        MyInput();

        //SetText
        text.SetText(bulletsLeft + " / " + magazineSize);
    }
    private void MyInput()
    {
        if (pih.PrimaryFireTriggered)
        {
            shooting = true;
        }
        else if (!pih.PrimaryFireTriggered)
        {
            shooting = false;
        }

        if (pih.SecondaryFireTriggered)
        {
            grenading = true;
        }
        else if (!pih.SecondaryFireTriggered)
        {
            grenading = false;
        }

        if (pih.ReloadTriggered && bulletsLeft < magazineSize && !reloading) Reload();

        //Shoot
        if (readyToShoot && shooting && !reloading && (bulletsLeft > 0))
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
        // Grenade
        if (readyToGrenade && grenading && !reloading)
        {
            UseGrenade();
        }
    }
    private void Shoot()
    {
        readyToShoot = false;

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(Random.Range(-spread, spread) / 10, Random.Range(-spread, spread) / 10, 0);
        RaycastHit[] hits = Physics.RaycastAll(fpsCam.transform.position, direction, range, whatIsEnemy);
        foreach (var hit in hits)
        {
            Debug.Log(hit.collider.name);
            if (hit.collider.GetComponentInParent<EnemyHealth>() != null && hit.collider.CompareTag("WeakPoint"))
            {
                hit.collider.GetComponentInParent<EnemyHealth>().TakeDamage(damage + bonusDamage);
                //Debug.Log("Hp after headshot: " + hit.collider.GetComponentInParent<EnemyHealth>().health);
            }
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
        }

        //ShakeCamera
        //camShake.Shake(camShakeDuration, camShakeMagnitude);


        //Graphics
        //Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
        GameObject flash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        Destroy(flash, 0.1f);


        bulletsLeft -= bulletsPerTap;
        bulletsShot -= bulletsPerTap;


        Invoke("ResetShot", timeBetweenShooting);
        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);

    }
    private void ResetShot()
    {
        readyToShoot = true;
    }
    private void UseGrenade()
    {
        readyToGrenade = false;
        GameObject grenade = Instantiate(Grenade, attackPoint.position, Quaternion.identity);
        grenade.GetComponent<Rigidbody>().AddForce(fpsCam.transform.forward * grenadeForce, ForceMode.Impulse);
        Invoke("ResetGrenade", timeBetweenSecondaryFire);
    }
    private void ResetGrenade()
    {
        readyToGrenade = true;
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
    void IGun.Equip()
    {
        // Logic for animation and changing ammo display
        Debug.Log("Equipped AR");
    }
}
