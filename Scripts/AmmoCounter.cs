using UnityEngine;

public class AmmoCounter : MonoBehaviour
{
    public GameObject Player;
    private GunManager gm;
    public GameObject SecondaryAmmo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = Player.GetComponent<GunManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.currentGun.GetComponent<GunSecondary>() != null && gm.currentGun.GetComponent<GunSecondary>().separateAmmo)
        {
            SecondaryAmmo.SetActive(true);
        }
        else
        {
            SecondaryAmmo.SetActive(false);
        }
    }
}
