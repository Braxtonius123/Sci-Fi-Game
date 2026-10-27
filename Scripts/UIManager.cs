using UnityEngine;
using UnityEngine.Rendering;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public PlayerInputHandler pih;
    public GameObject Roster;
    private bool rosterOpen = false;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pih.RosterTriggered)
        {
            rosterOpen = true;
        }
        if (!pih.RosterTriggered)
        {
            rosterOpen = false;
        }

        if (rosterOpen)
        {
            Roster.SetActive(true);
        }
        else
        {
            Roster.SetActive(false);
        }
    }
}
