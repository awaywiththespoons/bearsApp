using UnityEngine;
using System.Collections;

	public class swimUP : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	
	}
		
		
	// Update is called once per frame
	void Update () {

		transform.Translate(Vector3.up * Time.deltaTime, Camera.main.transform);

	
	}
}
