using UnityEngine;
using System.Collections.Generic;

public class GunManager : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;

    //[SerializeField] private List<GameObject> Guns = new List<GameObject>();
    [SerializeField] private List<GameObject> Guns1 = new List<GameObject>();
    [SerializeField] private List<GameObject> Guns2 = new List<GameObject>();
    [SerializeField] private List<GameObject> Guns3 = new List<GameObject>();
    [SerializeField] private List<GameObject> Guns4 = new List<GameObject>();
    [SerializeField] private List<GameObject> Guns5 = new List<GameObject>();
    [SerializeField] private List<GameObject> Guns6 = new List<GameObject>();
    [SerializeField] private List<GameObject> Guns7 = new List<GameObject>();
    [SerializeField] private List<GameObject> Guns8 = new List<GameObject>();
    [SerializeField] private List<GameObject> Guns9 = new List<GameObject>();
    [SerializeField] private List<GameObject> Guns0 = new List<GameObject>();
    [HideInInspector] public List<List<GameObject>> Guns = new List<List<GameObject>>();
    [HideInInspector] public GameObject currentGun;
    public int currentKeyValue = 0, currentGunIndex = 0, gunInKey = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Guns.Add(Guns1);
        Guns.Add(Guns2);
        Guns.Add(Guns3);
        Guns.Add(Guns4);
        Guns.Add(Guns5);
        Guns.Add(Guns6);
        Guns.Add(Guns7);
        Guns.Add(Guns8);
        Guns.Add(Guns9);
        Guns.Add(Guns0);
        /*foreach (List<GameObject> gunList in Guns)
        {
            foreach (GameObject gun in gunList)
                Debug.Log(gun.name);
        }*/

        currentGun = Guns[0][0];
        foreach (List<GameObject> gunList in Guns)
        {
            foreach (GameObject gun in gunList)
            {
                if (gun != currentGun)
                {
                    gun.SetActive(false);
                }
            }
        }
        /*foreach (GameObject gun in Guns)
        {
            if (gun != currentGun)
            {
                gun.SetActive(false);
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        HandleCycle();
    }

    private void HandleCycle()
    {
        // Logic for the number keys
        int selectedGunIndex = Mathf.FloorToInt(playerInputHandler.CycleWeaponTriggered) - 1;
        if (selectedGunIndex >= 0 && selectedGunIndex < Guns.Count)
        {
            if (selectedGunIndex == currentKeyValue)
            {
                if (gunInKey < selectedGunIndex)
                    gunInKey++;
                else
                    gunInKey = 0;
            }
            else
            {
                currentKeyValue = selectedGunIndex;
                gunInKey = 0;
            }
            if (Guns[selectedGunIndex][gunInKey] != null)
            {
                // Equip new gun
                if (currentGun != null)
                {
                    currentGun.SetActive(false);
                }
                currentGun = Guns[selectedGunIndex][gunInKey];
                currentGun.SetActive(true);
                // Call equip method if new gun implements IGun
                if (currentGun.TryGetComponent<IGun>(out IGun gun))
                {
                    gun.Equip();
                }
            }
        }
        playerInputHandler.CycleWeaponTriggered = 0f;

        // Logic for scrolling through the guns
        if (playerInputHandler.ScrollWeaponTriggered != 0f && Guns.Count > 0)
        {
            if (gunInKey < Guns[currentGunIndex].Count - 1)
                gunInKey++;
            else
            {
                currentGunIndex = (currentGunIndex + (int)playerInputHandler.ScrollWeaponTriggered) % Guns.Count;
                gunInKey = 0;
            }
            if (currentGunIndex < 0)
            {
                currentGunIndex += Guns.Count; // Wrap around to the last gun
                gunInKey = 0;
            }

            if (Guns[currentGunIndex][gunInKey] == null)
            {
                while (Guns[currentGunIndex][gunInKey] == null)
                {
                    currentGunIndex++;
                }
            }

            // Equip new gun
            currentKeyValue = currentGunIndex;
            if (currentGun != null)
            {
                currentGun.SetActive(false);
            }
            currentGun = Guns[currentGunIndex][gunInKey];
            currentGun.SetActive(true);
            // Call equip method if new gun implements IGun
            if (currentGun.TryGetComponent<IGun>(out IGun gun))
            {
                gun.Equip();
            }
        }
    }

    /*public void AddGun(GameObject Gun)
    {
        Guns.Add(Gun);
        if (currentGun == null)
        {
            currentGun = Gun;
        }
    }

    public void RemoveGun(GameObject Gun)
    {
        Guns.Remove(Gun);
        if (currentGun == Gun)
        {
            currentGun = null;
        }
    }*/
}
