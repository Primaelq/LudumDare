using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public int edgeResolveIteration = 5;

    public float viewAngle = 90.0f, drawResolution = 2.0f, edgeDistanceThreshold;

    public float runSpeed = 3.0f, endurance = 5.0f, waitTime = 2.0f;

    public bool playerInSight = false;

    public LayerMask obstacleMask;

    public MeshFilter viewMeshFilter;

    public AudioClip[] sounds;
    public AudioClip[] sounds2;
    private AudioSource sound;

    private int currentTarget = 0;

    private NavMeshAgent navMeshAgent;

    private SphereCollider distCollider;

    private float timer = 0.0f;

    public float giveUpChaseDistance;

    public float hitDistance;

    private Animator anim;

    Mesh viewMesh;
    
	void Start ()
    {
        state = State.Patrol;

        navMeshAgent = GetComponent<NavMeshAgent>();

        distCollider = GetComponent<SphereCollider>();

        sound = GetComponent<AudioSource>();

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        anim = GetComponentInChildren<Animator>();

	}
	
	void Update ()
    {
        anim.SetFloat("Speed", navMeshAgent.speed);

        if (playerInSight && state == State.Chase && Vector3.Distance(navMeshAgent.destination, transform.position) < hitDistance)
        {
            anim.SetTrigger("Punch");
            TryToKillPlayer();
        }

        if(GameObject.FindGameObjectWithTag("Player") != null && !GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().openingSafe)
        {
            switch (state)
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
	}

    void LateUpdate()
    {
        DrawFieldOfView();
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

        if(Vector3.Distance(transform.position, patrolPoints[currentTarget].position) < 0.25f || navMeshAgent.velocity == Vector3.zero)
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

        if(Time.time - timer < endurance || (Vector3.Distance(GameObject.FindWithTag("Player").transform.position, transform.position) < giveUpChaseDistance))
        {
            navMeshAgent.speed = runSpeed;

            if(GameObject.FindWithTag("Player") != null)
            {
                navMeshAgent.SetDestination(GameObject.FindWithTag("Player").transform.position);
            }
            else
            {
                ReturnToPatrol();
            }

            if(Vector3.Distance(transform.position, GameObject.FindWithTag("Player").transform.position) < 1.25f)
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

       // if(Vector3.Distance())
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

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * drawResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for(int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if(i > 0)
            {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;

                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);

                    if(edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }

                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int VertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[VertexCount];
        int[] triangles = new int[(VertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for(int i = 0; i < VertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if(i < VertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for(int i = 0; i < edgeResolveIteration; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;

            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float gloablAngle)
    {
        Vector3 direction = DirFromAngle(gloablAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, direction, out hit, distCollider.radius * transform.localScale.y, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, gloablAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + direction * distCollider.radius * transform.localScale.y, distCollider.radius * transform.localScale.y, gloablAngle);
        }
    }

    Vector3 DirFromAngle(float angleDegree, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleDegree += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleDegree * Mathf.Deg2Rad), 0.0f, Mathf.Cos(angleDegree * Mathf.Deg2Rad));
    }

    struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }

    struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
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

                if(Physics.Raycast(transform.position, deltaPosition.normalized, out hit, distCollider.radius * transform.localScale.y))
                {
                    if(hit.collider.gameObject.tag == "Player")
                    {
                        sound.clip = sounds[(int)Random.Range(0, sounds.Length - 1)];
                        sound.Play();

                        playerInSight = true;
                        state = State.Idle;
                    }
                  
                    if(hit.collider.gameObject.tag == "FurnitureShape")
                    {
                        sound.clip = sounds2[(int)Random.Range(0, sounds2.Length - 1)];
                        sound.Play();

                        WaitBeforeReturnPatrol(2.0f);
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
  
  IEnumerator WaitBeforeReturnPatrol(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ReturnToPatrol();
    }

    public void TryToKillPlayer()
    {
        if(GetComponentInChildren<CheckKillPlayer>().playerInRange)
        {
            //Debug.Log("Ded");
            GameObject g = GameObject.FindWithTag("Player");
            if(g!=null)
                g.SendMessage("Die");
        }
    }
}
