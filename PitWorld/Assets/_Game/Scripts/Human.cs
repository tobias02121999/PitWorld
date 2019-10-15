using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Entity
{
    // Initialize the public enums
    public enum States { DEAD, GETTING_FOOD, GETTING_MEDICINE, SOCIALIZING, IDLING, BEGGING, DONATE }

    // Initialize the public variables
    [HideInInspector]
    public Brain brain;

    [HideInInspector]
    public Inventory inventory;

    [HideInInspector]
    public Awareness awareness;

    public States state;
    public Vector2 idleDuration;
    public float wanderDistance;

    // Initialize the private variables
    public SpriteRenderer speechSprite, happySprite, sadSprite, hungrySprite;

    // Start is called before the first frame update
    void Start()
    {
        Initialize(); // Initialize the entity object
        awareness = GetComponentInChildren<Awareness>();
        brain = gameObject.AddComponent<Brain>();
        brain.awareness = awareness;
        inventory = gameObject.AddComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        Mind(); // Control the humans mind (personality variables)
        Control(); // Control which state should be ran

        speechSprite.enabled = (GetAnim(2));
        happySprite.enabled = (brain.happiness > .5f);
        sadSprite.enabled = !happySprite.enabled;
        hungrySprite.enabled = (brain.food <= .5f);
    }

    // Control which functions should be ran
    void Control()
    {
        // Check for suicide/death
        if (brain.happiness <= 0f || brain.food <= 0f || HP <= 0f)
        {
            Die(); // Die
            state = States.DEAD;
        }
        else
        {
            // Check if low on cash
            if (CheckLowCash(inventory.currency))
            {
                Beg();
                state = States.BEGGING;
            }
            else
            {
                // Check if hungry
                if (brain.food <= .5f)
                {
                    GetFood(); // Get food
                    state = States.GETTING_FOOD;
                }
                else
                {
                    // Check if depressed or hurt
                    if (brain.happiness <= .25f || HP <= (maxHP / 2f))
                    {
                        GetMedicine(); // Get medicine
                        state = States.GETTING_MEDICINE;
                    }
                    else
                    {
                        // Check for social needs
                        if (brain.happiness <= .5f)
                        {
                            Socialize(); // Go socialize
                            state = States.SOCIALIZING;
                        }
                        else
                        {
                            if (CheckForBeggars() && !CheckLowCash(inventory.currency - .01f))
                            {
                                Donate(.01f);
                                state = States.DONATE;
                            }
                            else
                            {
                                Idle(); // Do nothing
                                state = States.IDLING;
                            }
                        }
                    }
                }
            }
        }
    }

    // Control the humans mind (personality variables)
    void Mind()
    {
        brain.happiness -= .0001f;
        brain.food -= .00005f;

        if (brain.social >= .001f)
        {
            brain.happiness += .001f;
            brain.social -= .001f;
        }
    }

    void Idle()
    {
        if (IsMoving())
            PlayAnim(1);
        else
            PlayAnim(0);

        Wander(idleDuration, wanderDistance); // Step around a tiny bit here and there
    }

    void GetFood()
    {
        var size = brain.mem_Stores.Count;
        Store target = null;

        if (size > 0)
        {
            for (var i = 0; i < size; i++)
            {
                float distOld;

                if (target != null)
                {
                    distOld = Vector3.Distance(transform.position, target.transform.position);
                    var dist = Vector3.Distance(transform.position, brain.mem_Stores[i].transform.position);

                    if (brain.mem_Stores[i].hasFood && dist < distOld)
                        target = brain.mem_Stores[i];
                }
                else
                {
                    if (brain.mem_Stores[i].hasFood)
                        target = brain.mem_Stores[i];
                }
            }

            if (target != null)
            {
                var trans = target.entrance.position;
                FindPath(trans);
            }
        }
        else
            Wander(new Vector2(0f, 0f), wanderDistance); // Explore the surrounding area

        if (IsMoving())
            PlayAnim(1);
        else
            PlayAnim(0);
    }

    void GetMedicine()
    {
        var size = brain.mem_Stores.Count;
        Store target = null;

        if (size > 0)
        {
            for (var i = 0; i < size; i++)
            {
                float distOld;

                if (target != null)
                {
                    distOld = Vector3.Distance(transform.position, target.transform.position);
                    var dist = Vector3.Distance(transform.position, brain.mem_Stores[i].transform.position);

                    if (brain.mem_Stores[i].hasMedicine && dist < distOld)
                        target = brain.mem_Stores[i];
                }
                else
                {
                    if (brain.mem_Stores[i].hasMedicine)
                        target = brain.mem_Stores[i];
                }
            }

            if (target != null)
            {
                var trans = target.entrance.position;
                FindPath(trans);
            }
        }
        else
            Wander(new Vector2(0f, 0f), wanderDistance); // Explore the surrounding area

        if (IsMoving())
            PlayAnim(1);
        else
            PlayAnim(0);
    }

    void Socialize()
    {
        var size = awareness.humans.Count;
        if (size > 0)
        {
            for (var i = 0; i < size; i++)
            {
                if (awareness.humans[i].state == States.SOCIALIZING)
                {
                    var human = awareness.humans[i];
                    FindPath(human.transform.position);

                    var dist = Vector3.Distance(transform.position, human.transform.position);
                    if (dist <= 1f)
                    {
                        ResetPath();

                        brain.social = .25f;
                        PlayAnim(2); // Play the socializing animation
                    }
                    else
                    {
                        if (IsMoving())
                            PlayAnim(1);
                        else
                            PlayAnim(0);
                    }
                }
            }
        }
        else
        {
            if (IsMoving())
                PlayAnim(1);
            else
                PlayAnim(0);

            Wander(new Vector2(0f, 0f), wanderDistance); // Explore the surrounding area
        }
    }

    bool CheckLowCash(float amount)
    {
        var size = brain.mem_Stores.Count;
        if (size > 0)
        {
            Store cheapest = brain.mem_Stores[0];

            for (var i = 0; i < size; i++)
            {
                if (brain.mem_Stores[i].price < cheapest.price)
                    cheapest = brain.mem_Stores[i];
            }

            return (amount < cheapest.price);
        }
        else
            return false;
    }

    void Beg()
    {
        ResetPath(); // Reset the destination to stop the agent from moving
        PlayAnim(4);
    }

    bool CheckForBeggars()
    {
        var size = awareness.humans.Count;
        if (size > 0)
        {
            for (var i = 0; i < size; i++)
            {
                if (awareness.humans[i].state == States.BEGGING && brain.traits.Contains(Brain.Traits.GENEROUS))
                    return true;
            }
        }

        return false;
    }

    void Donate(float amount)
    {
        var size = awareness.humans.Count;
        for (var i = 0; i < size; i++)
        {
            if (awareness.humans[i].state == States.BEGGING && brain.traits.Contains(Brain.Traits.GENEROUS))
            {
                var human = awareness.humans[i];
                FindPath(human.transform.position);

                var dist = Vector3.Distance(transform.position, human.transform.position);
                if (dist <= 1f)
                {
                    ResetPath();

                    human.inventory.currency += amount;
                    inventory.currency -= amount;

                    PlayAnim(5); // Play the donating animation
                }
                else
                {
                    if (IsMoving())
                        PlayAnim(1);
                    else
                        PlayAnim(0);
                }
            }
        }
    }

    void Die()
    {
        ResetPath(); // Reset the destination to stop the agent from moving
        Suicide(); // Commit suicide
        PlayAnim(3); // Play the death animation
    }
}
