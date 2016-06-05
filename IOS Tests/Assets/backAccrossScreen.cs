using UnityEngine;
using System.Collections;
using Assets;

public class backAccrossScreen : Resetable
{
	// Update is called once per frame
	void Update () {

		transform.Translate(Vector3.left * Time.deltaTime, Camera.main.transform);

		}
	}

