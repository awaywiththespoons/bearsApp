using UnityEngine;
using System.Collections;



public class pageSorter : MonoBehaviour {


	List <int> pageHistory = new List<int>();

	// Use this for initialization
	void Start () {
		
	
	}
	
	// Update is called once per frame
	void Update () {
		
		
// pages
	if (Input.GetKeyDown(KeyCode.Alpha1) == true){
			print("page0 FOREST");
			pageHistory.add(1);
	}

	if (Input.GetKeyDown(KeyCode.Alpha2) == true){
			print("page1 WATER"); 
			pageHistory.add(2); 
	}

	if (Input.GetKeyDown(KeyCode.Alpha3) == true){
			print("page2 CITY");
			pageHistory.add(3); 
	}

	if (Input.GetKeyDown(KeyCode.Alpha4) == true){
			print("page3 DUNES");
			pageHistory.add(4); 
	}

	if (Input.GetKeyDown(KeyCode.Alpha5) == true){
			print("page4 GIANTS");
			pageHistory.add(5);
	}


// end if there are 5 pages
if (pageHistory.size()>= 5) {
	print("end");
}

// end for three pages in a row
for (int pageIndex = 0; pageIndex <= 5; pageIndex++)
{
	// count when a page is put on the screen 
	int count = 0; 
	for (int j = 0; j < pageHistory.length(); j++) {
		if (pageHistory.get(j) == pageIndex) {
			count++;
		}
	} 
	// when the count of that page is 3 then end
	if (count>=3) {
		print("end more than 3");
	}
		}
	}

}