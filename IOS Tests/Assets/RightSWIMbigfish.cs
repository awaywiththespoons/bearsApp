using UnityEngine;
using System.Collections;
using Assets;

public class RightSWIMbigfish : Resetable
{
	
	// Update is called once per frame
	void Update () {

		transform.Translate(Vector3.right * (Time.deltaTime*4), Camera.main.transform);
	
	}
}
