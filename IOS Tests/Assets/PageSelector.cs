using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;

[RequireComponent(typeof(FlickGesture))]
public class PageSelector : MonoBehaviour {

    private Dictionary<int, Page> pages = new Dictionary<int, Page>();
    private FlickGesture horizontalFlick;
    private int activePage; 

    public void SelectPage(int pageNumber)
    {
        Page page;

        if (pages.TryGetValue(pageNumber, out page) == false)
        {
            print("No page with the page number '" + pageNumber + "' could be found.");
            return; 
        }

        print("Setting current page to '" + pageNumber + "'.");

        activePage = pageNumber; 

        foreach (Page other in pages.Values)
        {
            other.gameObject.SetActive(other.PageNumber == pageNumber); 
        }
    }

    void Awake()
    {
        horizontalFlick = GetComponent<FlickGesture>();

        horizontalFlick.Flicked += HorizontalFlick_Flicked;

        foreach (Page page in GetComponentsInChildren<Page>())
        {
            if (pages.ContainsKey(page.PageNumber) == true)
            {
                print("2 or more pages with the page number '" + page.PageNumber + "' exist.");

                continue; 
            }

            pages.Add(page.PageNumber, page);

            page.gameObject.SetActive(page.PageNumber == 0);
        }
    }

    private void HorizontalFlick_Flicked(object sender, System.EventArgs e)
    {
        if (horizontalFlick.ScreenFlickVector.x < 0)
        {
            SelectPage(activePage - 1);
        }
        else
        {
            SelectPage(activePage + 1);
        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        // flip between pages (debug) 
        if (Input.GetKeyDown(KeyCode.Alpha0) == true)
        {
            SelectPage(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) == true)
        {
            SelectPage(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) == true)
        {
            SelectPage(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) == true)
        {
            SelectPage(3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) == true)
        {
            SelectPage(4);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5) == true)
        {
            SelectPage(5);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6) == true)
        {
            SelectPage(6);
        }

        if (Input.GetKeyDown(KeyCode.Alpha7) == true)
        {
            SelectPage(7);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8) == true)
        {
            SelectPage(8);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9) == true)
        {
            SelectPage(9);
        }

    }
}
