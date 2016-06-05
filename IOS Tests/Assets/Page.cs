using UnityEngine;
using System.Collections;
using Assets;

public class Page : MonoBehaviour {

    public int PageNumber;

    private Animation[] animations;
    private Resetable[] resetables; 

    void Awake()
    {
        animations = GetComponentsInChildren<Animation>();
        resetables = GetComponentsInChildren<Resetable>();
    }

    public void ResetAnimations()
    {
        foreach (Animation animation in animations)
        {
            animation.Rewind();
            animation.Play(); 
        }

        foreach (Resetable resetable in resetables)
        {
            resetable.Reset(); 
        }        
    }
}
