using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    // Initialize the public enums
    public enum Traits { NONE, GENEROUS }

    // Initialize the public variables
    public float happiness, anger, social, food;
    public List<Human> mem_Humans = new List<Human>();
    public List<Store> mem_Stores = new List<Store>();
    public Awareness awareness;
    public List<Traits> traits = new List<Traits>();

    // Randomize the variables
    void Awake()
    {
        happiness = Random.Range(.5f, 1f);
        anger = Random.Range(0f, .5f);
        food = Random.Range(.5f, 1f);

        var trait = (Traits)Mathf.RoundToInt(Random.Range(0f, 1f));
        traits.Add(trait);
    }

    // Update is called once per frame
    void Update()
    {
        Memory(); // Control the brains memory
    }

    // Control the brains memory
    void Memory()
    {
        int size;

        size = awareness.humans.Count;
        for (var i = 0; i < size; i++)
        {
            var obj = awareness.humans[i];
            if (!mem_Humans.Contains(obj))
                mem_Humans.Add(obj);
        }
        
        size = awareness.stores.Count;
        for (var i = 0; i < size; i++)
        {
            var obj = awareness.stores[i];
            if (!mem_Stores.Contains(obj))
                mem_Stores.Add(obj);
        }
    }
}
