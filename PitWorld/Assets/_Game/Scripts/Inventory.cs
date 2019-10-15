using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Initialize the public variables
    public float currency;

    // Randomize the variables
    void Awake()
    {
        currency = Random.Range(5f, 50f);
    }
}
