﻿using UnityEngine;
using System.Collections;
using Assets;

public class swimUP : Resetable
{
	// Update is called once per frame
	void Update () {

		transform.Translate(Vector3.up * Time.deltaTime, Camera.main.transform);	
	}
}
