using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    // Initialize the public variables
    //[HideInInspector]
    public Awareness awareness;

    //[HideInInspector]
    public Transform entrance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Initialize the building object
    protected void Initialize()
    {
        awareness = GetComponentInChildren<Awareness>();
        entrance = awareness.transform;
    }
}
