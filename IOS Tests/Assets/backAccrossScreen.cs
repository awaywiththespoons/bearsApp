using UnityEngine;
using System.Collections;

public class backAccrossScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.Translate(Vector3.left * Time.deltaTime, Camera.main.transform);

		}
	}

