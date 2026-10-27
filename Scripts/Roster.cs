using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Roster : MonoBehaviour
{
    public static Roster Instance { get; private set; }
    public List<string> titles = new List<string>();
    public GameObject Panel;
    public GameObject titlePrefab;

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

    }

    public void AddTitle(string title)
    {
        if (!titles.Contains(title))
        {
            GameObject myTitle = Instantiate(titlePrefab);

            // Set myTitle as a child of the roster
            myTitle.transform.SetParent(Panel.transform);
            myTitle.GetComponent<TextMeshProUGUI>().text = title;

            titles.Add(title);
        }
    }
}
