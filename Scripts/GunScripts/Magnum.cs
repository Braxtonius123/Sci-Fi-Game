using Mono.Cecil.Cil;
using TMPro;
using UnityEngine;

public class Magnum : MonoBehaviour, IGun
{
    public PlayerInputHandler pih;

    [Header("Shoot Speed Parameters")]
    public float timeBetweenShots, timeBetweenShooting;

    [Header("Primary Parameters")]
    //Gun stats
    public float damage, bonusDamage, spread, range, reloadTime, aimAssist;
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
    bool scopedIn = false;
    public float targetFOV, scopeSpeed;
    int defaultFOV = 60;

    private void Awake()
    {
        pih = GameObject.Find("PlayerInputHandler").GetComponent<PlayerInputHandler>();
        bulletsLeft = magazineSize;
        readyToShoot = true;
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
            pih.PrimaryFireTriggered = false;
        }
        else
        {
            shooting = false;
        }

        if (pih.SecondaryFireTriggered)
        {
            scopedIn = true;
        }
        else
        {
            scopedIn = false;
        }

        if (pih.ReloadTriggered && bulletsLeft < magazineSize && !reloading) Reload();

        //Shoot
        if (readyToShoot && shooting && !reloading && (bulletsLeft > 0))
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
        // Scope
        if (scopedIn)
        {
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, targetFOV, scopeSpeed * Time.deltaTime);
        }
        else
        {
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, defaultFOV, scopeSpeed * Time.deltaTime);
        }
    }
    private void Shoot()
    {
        readyToShoot = false;

        /*Calculate Direction with Spread
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
        }*/

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(Random.Range(-spread, spread) / 10, Random.Range(-spread, spread) / 10, 0);
        RaycastHit[] hits = Physics.SphereCastAll(fpsCam.transform.position, aimAssist, direction, range, whatIsEnemy);
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                hit.collider.GetComponent<EnemyHealth>().TakeDamage(damage);
            if (hit.collider.GetComponentInParent<EnemyHealth>() != null && hit.collider.CompareTag("WeakPoint"))
                hit.collider.GetComponentInParent<EnemyHealth>().TakeDamage(bonusDamage);
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
        Debug.Log("Equipped Magnum");
    }
}
