using UnityEngine;
using System.Collections;

public class EnemyAIBase : MonoBehaviour
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


    Light Eye;
    /* Last Known Position of the Player */
    protected Vector3 LastPlayerPos;
    #endregion
    void Awake()
    {
        Eye = GetComponentInChildren<Light>();
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

    }

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
            if (TargetTransform)
            {
                Agent.SetDestination(TargetTransform.position);
                LastPlayerPos = TargetTransform.position;
            }
        }

       
        if(Agent.velocity.sqrMagnitude > 0)
        {
            if(IsDestinationReached())
            {
                AgentState = EAIState.Idle;
                Eye.color = Color.green;
            }
        }
       
    }


    //Returns true if the Player is in Sight and in Field of View
    private bool IsPlayerInSight()
    {
        if(TargetTransform != null &&
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

    //Starts the Chase by Setting the Agent State to Chasing
    private void StartChase()
    {
        //Set State to Chasing
        AgentState = EAIState.Chasing;

        Eye.color = Color.red;
        
    }

    //Stops the Chase by Setting the Agent State to Suspicious and Moves to the Last Known Position
    private void StopChase()
    {
        Eye.color = Color.yellow;

        //Set Agent State to Suspicious
        AgentState = EAIState.Suspicious;

        //Move to the Last Known Position
        Agent.SetDestination(LastPlayerPos);
    }

    //Checked Destination Reached
    private bool IsDestinationReached()
    {
        if(Agent.remainingDistance - Agent.stoppingDistance < 0)
        {
            return true;
        }
        return false;
    }

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
}