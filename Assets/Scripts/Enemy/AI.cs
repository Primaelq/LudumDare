using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour
{
    public enum State
    {
        Idle,
        Patrol,
        Chase
    }

    public State state;

    public Transform[] patrolPoints;

    public float viewAngle = 90.0f;

    public float runSpeed = 3.0f, endurance = 5.0f, waitTime = 2.0f;

    public bool playerInSight = false;

    private int currentTarget = 0;

    private NavMeshAgent navMeshAgent;

    private SphereCollider distCollider;

    private float timer = 0.0f;
    
	void Start ()
    {
        state = State.Patrol;

        navMeshAgent = GetComponent<NavMeshAgent>();

        distCollider = GetComponent<SphereCollider>();
	}
	
	void Update ()
    {
	    switch(state)
        {
            case State.Idle:
                Idle();
                break;

            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                Chase();
                break;
        }
	}

    void Idle()
    {
        navMeshAgent.speed = 0.0f;

        if(playerInSight)
        {
            // Say something like "Oh who are you ?!?"
            StartCoroutine(WaitBeforeChase(waitTime));
        }
    }

    void Patrol()
    {
        navMeshAgent.speed = 2.0f;

        navMeshAgent.SetDestination(patrolPoints[currentTarget].position);

        if(Vector3.Distance(transform.position, patrolPoints[currentTarget].position) < 0.5f)
        {
            if(currentTarget == patrolPoints.Length - 1)
            {
                currentTarget = 0;
            }
            else
            {
                currentTarget++;
            }
        }
    }

    void Chase()
    {
        if(timer == 0.0f)
        {
            timer = Time.time;
        }

        if(Time.time - timer < endurance)
        {
            navMeshAgent.speed = runSpeed;
            navMeshAgent.SetDestination(GameObject.FindWithTag("Player").transform.position);

            if(Vector3.Distance(transform.position, GameObject.FindWithTag("Player").transform.position) < 0.25f)
            {
                Debug.Log("Game Over");
                Time.timeScale = 0.0f;
            }
        }
        else
        {
            ReturnToPatrol();
            timer = 0.0f;
        }
    }

    void ReturnToPatrol()
    {
        Transform nearestTransform = patrolPoints[0];

        for (int i = 0; i < patrolPoints.Length - 1; i++)
        {
            nearestTransform = Vector3.Distance(transform.position, nearestTransform.position) < Vector3.Distance(transform.position, patrolPoints[i + 1].position) ? nearestTransform : patrolPoints[i + 1];
        }

        currentTarget = System.Array.IndexOf(patrolPoints, nearestTransform);
        state = State.Patrol;
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && state == State.Patrol)
        {

            Vector3 deltaPosition = other.transform.position - transform.position;
            float deltaAngle = Vector3.Angle(deltaPosition, transform.forward);

            if(deltaAngle < viewAngle / 2)
            {
                RaycastHit hit;

                if(Physics.Raycast(transform.position, deltaPosition.normalized, out hit, distCollider.radius))
                {
                    if(hit.collider.gameObject.tag == "Player")
                    {
                        playerInSight = true;
                        state = State.Idle;
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && playerInSight)
        {
            state = State.Chase;
        }
    }

    IEnumerator WaitBeforeChase(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        state = State.Chase;
    }
}
