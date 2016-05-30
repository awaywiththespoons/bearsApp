using UnityEngine;
using System.Collections;

public class accrossScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.Translate(Vector3.right * Time.deltaTime, Camera.main.transform);
	
		if (transform.position.x >= 14) {
			transform.Translate(15, 0, 0);
		}
	}


}