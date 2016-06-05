using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class Resetable : MonoBehaviour
    {
        Vector3 startingPostion;

        void Awake()
        {
            startingPostion = transform.position;
        }

        public void Reset()
        {
            transform.position = startingPostion;
        }
    }
}
