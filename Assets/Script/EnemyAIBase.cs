using UnityEngine;
using System.Collections;

public class EnemyAIBase : MonoBehaviour,IDamagable<float,GameObject>
{

    public enum EAIState
    {
        Idle,
        Patrolling,
        Suspicious,
        Chasing
    }

    #region Properties

    [Header("AI Settings")]
    [SerializeField]
    /* Field of View of the AI */
    public float FieldOfViewAngle = 100.0f;

    /* AI Attack Range */
    public float AttackRange = 2.0f;

    /* Navmesh Component Reference */
    protected NavMeshAgent Agent;

    /* State of the AI Agent */
    public EAIState AgentState = EAIState.Idle;

    /* Whether Player is In Sight */
    public bool PlayerInSight = false;

    /* Whether Player is In Range */
    public bool PlayerInRange = false;

    /* Whether Enemy Chasing the Player */
    public bool IsChasingPlayer = false;

    /* Enemy Target */
    Transform TargetTransform;

    /* Area of Sight*/
    SphereCollider AreaOfSight;


    //Light Eye;
    /* Last Known Position of the Player */
    protected Vector3 LastPlayerPos;
    #endregion
    void Awake()
    {
       // Eye = GetComponentInChildren<Light>();
        #region Initialize Agent
        //Get Navmesh Agent
        Agent = GetComponent<NavMeshAgent>();
        if (Agent == null)
        {
            this.enabled = false;
            Debug.Log("Error: AI Does Not Have NavMeshAgent Component");
        }
        #endregion

        AreaOfSight = GetComponent<SphereCollider>();
    }

    // Use this for initialization
    void Start()
    {
        LastPlayerPos = Vector3.zero;
        StartPartrol();
    }

    #region AI Logic 
    // Update is called once per frame
    void FixedUpdate()
    {

        //Check for Sight If we are not already Chasing the Player and Player is in range
        if(AgentState != EAIState.Chasing && PlayerInRange)
        {
            if(IsPlayerInSight())
            {
                StartChase();
            }
        }

        //Handle Chasing. Move to the Target
        if(AgentState == EAIState.Chasing)
        {
            //Check if We have a valid target
            if (TargetTransform)
            {
                //Move to Target
                Agent.SetDestination(TargetTransform.position);
                LastPlayerPos = TargetTransform.position;

                //If we are Close then Attack
                if(Vector3.Distance(transform.position,TargetTransform.position) <= AttackRange)
                {
                    IDamagable<float, GameObject> Interface = (IDamagable<float,GameObject>)TargetTransform.GetComponent(typeof(IDamagable<float, GameObject>));
                    if (Interface != null)
                        Interface.Damage(100.0f, gameObject);
                }
            }
            else
            {
                StopChase();
            }
        }

       //Check if Agent has Stopped , if so are we at the destination
        if(Agent.velocity.sqrMagnitude > 0)
        {
            if(IsDestinationReached())
            {
                AgentState = EAIState.Idle;
                //Eye.color = Color.green;
                StartPartrol();
            }
        }
       
    }

    //Reset AI
    public void ResetAI()
    {
        //Reset Target and Position
        TargetTransform = null;
        LastPlayerPos = transform.position;

        AgentState = EAIState.Idle;
        StartPartrol();

    }

    //Starts the Chase by Setting the Agent State to Chasing
    private void StartChase()
    {
        //Set State to Chasing
        AgentState = EAIState.Chasing;

        //Eye.color = Color.red;
        
    }

    //Stops the Chase by Setting the Agent State to Suspicious and Moves to the Last Known Position
    private void StopChase()
    {
        //Eye.color = Color.yellow;

        //Set Agent State to Suspicious
        AgentState = EAIState.Suspicious;

        //Move to the Last Known Position
        Agent.SetDestination(LastPlayerPos);
    }

    //Try to Find a Point on The Map and Patrols Over There
    private void StartPartrol()
    {
        Vector3 NewDestination;
        if(FindRandomPatrolPoint(out NewDestination))
        {
            Agent.SetDestination(NewDestination);
            AgentState = EAIState.Patrolling;
        }
    }

    #endregion

    #region Helper Functions
    //Find a Random Position on the NavMesh based on Range
    private bool FindRandomPatrolPoint(out Vector3 PatrolDestination)
    {

        //Distance from The AI to Patrol
        float distance = 50;

        //Used to Store Result from our query
        NavMeshHit navHit;

        //Try for Max 10 Times
        for(int i = 0; i < 10; i++)
        {

            //Find Direction from AI Origin
            Vector3 direction = Random.insideUnitSphere * distance + Vector3.zero;


            //Find closes point on Navmesh
            if (NavMesh.SamplePosition(direction,out navHit, distance,NavMesh.AllAreas))
            {
                Debug.DrawRay(navHit.position, Vector3.up, Color.blue, 4.0f);
                PatrolDestination = navHit.position;
                return true;
            }
        }

        PatrolDestination = Vector3.zero;
        return false;
    }

    //Returns true if the Player is in Sight and in Field of View
    private bool IsPlayerInSight()
    {
        if (TargetTransform != null &&
            AgentState != EAIState.Chasing)
        {
            //Get the Direction of the Player to the Enemy 
            Vector3 direction = TargetTransform.position - transform.position;

            //Calculate Angle Between Enemy Vector Forward and the Player
            float angle = Vector3.Angle(direction, transform.forward);

            //If Calculated Angle is Within the Enemy Field of View
            if (angle < FieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction.normalized, out hit, AreaOfSight.radius))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //Checked Destination Reached
    private bool IsDestinationReached()
    {
        if (Agent.remainingDistance - Agent.stoppingDistance < 0)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region OnTriggersHandling
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerInRange = true;
            TargetTransform = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = false;
            TargetTransform = null;

            //Stop the Chase if we are chasing
            if(AgentState == EAIState.Chasing)
                StopChase();
        }
    }
    #endregion

    public void Damage(float DamageValue, GameObject Instigator)
    {
        //TODO Apply Damage
        if (Instigator != null)
        {

        }
    }
}