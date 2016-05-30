using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class Puppet2D_FFDStoreData : MonoBehaviour {

    public List<Transform> FFDCtrls = new List<Transform>();
    public List<int> FFDPathNumber = new List<int>();
	[HideInInspector]
	public bool Editable = true;
	void Update ()
	{
		if(Editable)
		{
			for (int i = FFDCtrls.Count - 1; i >= 0; i--) 
			{
				if (FFDCtrls [i] == null)
					FFDCtrls.RemoveAt (i);
			}
		}
	}
}
