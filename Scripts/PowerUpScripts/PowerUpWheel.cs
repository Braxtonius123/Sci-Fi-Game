using UnityEngine;
using UnityEngine.Rendering;

public class PowerUpWheel : MonoBehaviour
{
    public GameObject PlayerInputHandler;
    private PlayerInputHandler pih;
    public GameObject Buttons;
    private RadialLayout rl;

    private bool buttonsOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pih = PlayerInputHandler.GetComponent<PlayerInputHandler>();
        rl = Buttons.GetComponent<RadialLayout>();
    }

    // Update is called once per frame
    void Update()
    {
        rl.CalculateLayoutInputVertical();
        rl.CalculateLayoutInputHorizontal();
        if (pih.PowerUpTriggered)
        {
            Buttons.SetActive(true);
            //Mathf.Lerp(rl.MinAngle, rl.MaxAngle, 0.5f);
            buttonsOpen = true;
        }
        else
        {
            buttonsOpen = false;
        }

        if (buttonsOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            rl.fDistance = Mathf.Lerp(rl.fDistance, 500, Time.deltaTime * 6);
            if (rl.fDistance > 499)
            {
                rl.fDistance = 500;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            rl.fDistance = Mathf.Lerp(rl.fDistance, 0, Time.deltaTime * 6);
            if (rl.fDistance < 1)
            {
                rl.fDistance = 0;
                Buttons.SetActive(false);
            }
        }
    }
}
