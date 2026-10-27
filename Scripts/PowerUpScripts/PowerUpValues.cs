using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpValues : MonoBehaviour
{
    public enum powerUpType
    {
        QuadDamage,
        Berserk,
        Haste,
        Camouflage
    }
    public powerUpType type;

    public int powerUpsLeft;
    private TextMeshProUGUI powerUpText;
    private Button btn;

    public GameObject PlayerInputHandler;
    private PlayerInputHandler pih;

    public GameObject Player;
    private GunManager gm;
    private PlayerHealth ph;
    private FirstPersonController fpc;

    [HideInInspector] public bool powerUpActive = false;
    private bool lastPowerUpPressed = false;
    [SerializeField] private float powerUpDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        powerUpText = GetComponentInChildren<TextMeshProUGUI>();
        powerUpText.text = powerUpsLeft.ToString();

        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnPressed);

        pih = PlayerInputHandler.GetComponent<PlayerInputHandler>();
        gm = Player.GetComponent<GunManager>();
        ph = Player.GetComponent<PlayerHealth>();
        fpc = Player.GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (powerUpActive)
        {
            powerUpDuration -= Time.deltaTime;
            if (powerUpDuration <= 0)
            {
                EndPower();
            }
        }
    }

    public void OnPressed()
    {
        PowerUp();
    }

    void PowerUp()
    {
        if (powerUpsLeft <= 0 || powerUpActive)
            return;

        powerUpsLeft--;
        powerUpText.text = powerUpsLeft.ToString();
        powerUpActive = true;
    }

    public void EndPower()
    {
        powerUpActive = false;
    }
}
