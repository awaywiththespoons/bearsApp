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
        if (animations != null)
        {
            foreach (Animation animation in animations)
            {
                animation.Rewind();
                animation.Play();
            }
        }

        if (resetables != null)
        {
            foreach (Resetable resetable in resetables)
            {
                resetable.Reset();
            }
        }        
    }
}
