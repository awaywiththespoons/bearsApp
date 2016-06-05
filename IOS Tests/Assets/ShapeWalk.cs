using UnityEngine;
using System.Collections;
using Assets;

public class ShapeWalk : Resetable {

    float walkValue;

    public float walkTime = 20;

    public AnimationCurve XPosition;
    public AnimationCurve YPosition;

	// Update is called once per frame
	void Update ()
    {
        Vector3 pos = transform.position;

        pos.x = startingPostion.x + XPosition.Evaluate(walkValue);
        pos.y = startingPostion.y + YPosition.Evaluate(walkValue);

        transform.position = pos; 

        walkValue = Mathf.Clamp01(walkValue + (Time.deltaTime / walkTime)); 
    }

    public override void Reset()
    {
        base.Reset();

        walkValue = 0; 
    }
}
