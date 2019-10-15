using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Awareness : MonoBehaviour
{
    // Initialize the public variables
    public List<Human> humans = new List<Human>();
    public List<Store> stores = new List<Store>();

    // Add the colliding object to the entities list if applied
    void OnTriggerEnter(Collider other)
    {
        var human = other.GetComponent<Human>();
        if (human != null)
            humans.Add(human);

        var store = other.GetComponent<Store>();
        if (store != null)
            stores.Add(store);
    }

    // Remove the colliding object from the entities list if applied
    void OnTriggerExit(Collider other)
    {
        var human = other.GetComponent<Human>();
        if (human != null)
            humans.Remove(human);

        var store = other.GetComponent<Store>();
        if (store != null)
            stores.Remove(store);
    }
}
