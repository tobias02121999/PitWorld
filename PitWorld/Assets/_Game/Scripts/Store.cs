using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : Building
{
    // Initialize the public variables
    public bool hasFood, hasMedicine;
    public float price, quality;

    // Start is called before the first frame update
    void Start()
    {
        Initialize(); // Initialize the building object
    }

    // Update is called once per frame
    void Update()
    {
        Sell(); // Sell items to the customers
    }

    // Sell items to the customers
    void Sell()
    {
        var size = awareness.humans.Count;
        for (var i = 0; i < size; i++)
        {
            var human = awareness.humans[i];
            if (human.brain.food <= .5f && human.inventory.currency >= price)
            {
                human.brain.food = 1f;
                human.inventory.currency -= price;
            }

            if ((human.brain.happiness <= .25f || human.HP <= (human.HP / 2f)) && human.inventory.currency >= price)
            {
                human.brain.happiness = 1f;
                human.HP = human.maxHP;
                human.inventory.currency -= price;
            }
        }
    }
}
