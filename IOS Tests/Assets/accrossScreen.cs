using UnityEngine;
using System.Collections;
using Assets;

public class accrossScreen : Resetable
{
	// Update is called once per frame
	void Update () {

		transform.Translate(Vector3.right * (Time.deltaTime/8), Camera.main.transform);
	
		if (transform.position.x >= 14) {
			transform.Translate(15, 0, 0);
		}
	}
}