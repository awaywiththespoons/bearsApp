using UnityEngine;
using System.Collections;
using Assets;

public class moveLEFT : Resetable
{
	// Update is called once per frame
	void Update () {

		transform.Translate(Vector3.left * (Time.deltaTime/2), Camera.main.transform);
	
	}
}
