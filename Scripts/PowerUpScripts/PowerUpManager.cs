using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static PowerUpValues;

public class PowerUpManager : MonoBehaviour
{
    public PowerUpValues[] powerUps;
    private PowerUpValues currentPowerUp;
    public float powerUpDuration;

    // Coloring
    private Image powerUpTint;
    private Color clear = new Color(1f, 1f, 1f, 0f);
    private Color blue = new Color(0f, 0.5f, 1f, 0.4f);
    private Color purple = new Color(0.5f, 0f, 1f, 0.4f);
    private Color red = new Color(1f, 0f, 0f, 0.4f);
    private Color yellow = new Color(1f, 1f, 0f, 0.4f);
    private Color orange = new Color(1f, 0.5f, 0f, 0.4f);
    private Color green = new Color(0f, 1f, 0f, 0.4f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        powerUpTint = GetComponent<Image>();
        powerUpTint.color = clear;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (PowerUpValues powerUp in powerUps)
        {
            if (powerUp.powerUpActive)
            {
                currentPowerUp = powerUp;

                Debug.Log("Power Used");
                switch (currentPowerUp.type)
                {
                    case powerUpType.QuadDamage:
                        StartCoroutine(FadeTint(blue));
                        break;
                    case powerUpType.Berserk:
                        StartCoroutine(FadeTint(red));
                        break;
                    case powerUpType.Haste:
                        StartCoroutine(FadeTint(yellow));
                        break;
                    case powerUpType.Camouflage:
                        StartCoroutine(FadeTint(green));
                        break;
                }

                StartCoroutine(PowerUpDuration());
            }
        }
    }

    private IEnumerator PowerUpDuration()
    {
        yield return new WaitForSeconds(powerUpDuration);
        currentPowerUp.EndPower();
        StartCoroutine(FadeTint(clear));
    }

    private IEnumerator FadeTint(Color chosenColor)
    {
        yield return null;

        while (powerUpTint.color != chosenColor)
        {
            powerUpTint.color = Color.Lerp(powerUpTint.color, chosenColor, Time.deltaTime * 2);
            yield return null;
        }
    }
}
