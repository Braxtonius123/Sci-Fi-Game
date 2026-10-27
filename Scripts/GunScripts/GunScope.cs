using UnityEngine;

public class GunScope : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;
    public Camera fpsCam;

    [SerializeField] private float scopeSpeed, targetFOV;
    private int defaultFOV = 60;
    private bool scopedIn = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInputHandler.SecondaryFireTriggered)
        {
            scopedIn = true;
        }
        if (playerInputHandler.SecondaryFireTriggered == false)
        {
            scopedIn = false;
        }

        if (scopedIn)
        {
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, targetFOV, scopeSpeed * Time.deltaTime);
        }
        else
        {
            fpsCam.fieldOfView = Mathf.Lerp(fpsCam.fieldOfView, defaultFOV, scopeSpeed * Time.deltaTime);
        }
    }


}
