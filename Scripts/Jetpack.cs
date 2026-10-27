using UnityEngine;

public class Jetpack : MonoBehaviour
{
    public PlayerInputHandler playerInputHandler;
    private FirstPersonController fpc;

    [HideInInspector] public bool hasJetpack = true, usingJetPack, jetPackActive;
    [SerializeField] private float jetPackForce, jetPackFuel, maxJetPackFuel, jetPackCooldown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fpc = GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasJetpack && playerInputHandler.ActivateJetpackTriggered)
        {
            jetPackActive = !jetPackActive;
            playerInputHandler.ActivateJetpackTriggered = false;
        }

        if (hasJetpack && jetPackActive)
        {
            HandleJetpack();
        }

        // Handle jetpack fuel consumption and regeneration
        if (usingJetPack)
        {
            jetPackFuel -= Time.deltaTime;
            if (jetPackFuel <= 0)
            {
                usingJetPack = false;
                jetPackFuel = 0;
            }
        }
        else
        {
            jetPackCooldown -= Time.deltaTime;
            if (jetPackFuel < maxJetPackFuel && jetPackCooldown <= 0)
            {
                jetPackFuel += Time.deltaTime * 0.75f;
                if (jetPackFuel > maxJetPackFuel)
                {
                    jetPackFuel = maxJetPackFuel;
                }
            }
            if (jetPackCooldown <= 0)
            {
                jetPackFuel = maxJetPackFuel;
            }
        }
    }

    private void HandleJetpack()
    {
        if (hasJetpack && jetPackFuel > 0)
        {
            // While the jump button is held, apply upward force
            if (playerInputHandler.JumpTriggered)
            {
                fpc.currentMovement.y = jetPackForce;
                usingJetPack = true;
                jetPackCooldown = 5;
            }
            else
            {
                fpc.currentMovement.y += Physics.gravity.y * fpc.gravityMultiplier * Time.deltaTime;
                usingJetPack = false;
            }
        }
    }
}
