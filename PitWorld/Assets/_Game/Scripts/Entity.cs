using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour
{
    // Initialize the public variables
    public float maxHP = 1f;

    [HideInInspector]
    public float HP;

    public float walkSpeed = 100f, sprintSpeed = 200f;
    public Controls controls = new Controls();
    public string[] animations;

    // Initialize the private variables
    float speed;
    Vector3 lookRot;
    Animation animator;
    NavMeshAgent agent;
    int wanderTimer;
    Vector3 target;

    // Initialize the entity object
    protected void Initialize()
    {
        HP = maxHP;
        lookRot = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
        animator = GetComponentInChildren<Animation>();
        agent = GetComponent<NavMeshAgent>();
        target = transform.position;
    }

    // Move by input
    protected void Move(Rigidbody rb, string axisHor, string axisVer)
    {
        var forward = ((transform.forward * speed) * Input.GetAxis(axisVer));
        var strafe = (transform.right * speed) * Input.GetAxis(axisHor);
        var vel = (forward + strafe) * Time.deltaTime;
        vel.y = rb.velocity.y;

        rb.velocity = vel;
    }

    // Look around by input
    protected void Look(Transform transHor, Transform transVer, Vector2 sensitivity, string axisHor, string axisVer)
    {
        lookRot.x += (Input.GetAxis(axisVer) * sensitivity.y) * Time.deltaTime;
        lookRot.y += (Input.GetAxis(axisHor) * sensitivity.x) * Time.deltaTime;

        lookRot.x = Mathf.Clamp(lookRot.x, -90f, 90f);

        transVer.rotation = Quaternion.Euler(lookRot.x, transVer.eulerAngles.y, transVer.eulerAngles.z);
        transHor.rotation = Quaternion.Euler(transHor.eulerAngles.x, lookRot.y, transHor.eulerAngles.z);
    }

    // Increase the entities speed
    protected void Sprint(bool isSprinting)
    {
        if (isSprinting)
            speed = sprintSpeed;
        else
            speed = walkSpeed;
    }

    // Commit suicide
    protected void Suicide()
    {
        HP = 0f;
    }

    // Play the chosen animation
    protected void PlayAnim(int animID)
    {
        if (!GetAnim(animID))
            animator.Play(animations[animID]);
    }

    // Check if the animation is currently playing
    protected bool GetAnim(int animID)
    {
        return (animator.IsPlaying(animations[animID]));
    }

    // Move towards the cursor using pathfinding
    protected void FindPath(Vector3 target)
    {
        agent.SetDestination(target);
    }

    // Reset the destination to stop the agent from moving
    protected void ResetPath()
    {
        agent.SetDestination(transform.position);
    }

    // Check if the agent is currently moving or not
    protected bool IsMoving()
    {
        return !(agent.remainingDistance <= agent.stoppingDistance);
    }

    // Wander around
    protected void Wander(Vector2 idleDuration, float wanderDistance)
    {
        if (wanderTimer == 1)
        {
            Vector3 randomDir = Random.insideUnitSphere * wanderDistance;
            randomDir += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDir, out hit, wanderDistance, 1);
            target = hit.position;
        }

        if (wanderTimer <= 0)
        {
            FindPath(target);

            if (!IsMoving())
                wanderTimer = Mathf.RoundToInt(Random.Range(idleDuration.x, idleDuration.y));
        }

        wanderTimer--;
    }
}
