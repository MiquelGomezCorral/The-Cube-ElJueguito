using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminateParticlesScript : MonoBehaviour
{
    public float Time;
    void Start()
    {
        Destroy(gameObject, Time);
    }

}
