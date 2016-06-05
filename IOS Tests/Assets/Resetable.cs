using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class Resetable : MonoBehaviour
    {
        [HideInInspector] 
        public Vector3 startingPostion;

        void Awake()
        {
            startingPostion = transform.position;
        }

        public virtual void Reset()
        {
            transform.position = startingPostion;
        }
    }
}
