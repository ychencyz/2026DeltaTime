using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Combat,
        Die,
        //Reset
    }
    public AIState nowState = AIState.Idle;
    public Transform[] waypoints;
    private int currentWaypointsIndex = -1; //選到第幾個waypoint
    private NavMeshAgent agent;
    //public Transform playerTransform;
    public float moveSpeed = 2f;
    public float runSpeed = 5f;
    public float waitTimeMove = 2f;
    public float attackRange = 2f;
    public Vector3 spawnLoction; //出生點
    public float detectionRadius = 10f;
    public float distance;
    public float stopPursueDistance = 15f; //超過出生點到不追逐的距離
    private GameObject playerObject;
    private Enemy enemy;
    private Transform playerTransform;
    //private float viewAngle = 360f;

    [Header("群體站位")]
    [SerializeField] private float surroundRadiusMultiplier = 0.9f;
    private float _surroundAngleOffset;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //playerObject = GameObject.FindWithTag("Player"); //找到玩家
        playerObject = GameManager.Instance.Go_Player; //找到玩家
        playerTransform = playerObject.transform;
        spawnLoction = transform.position;
        _surroundAngleOffset = Mathf.Abs(GetInstanceID()) % 360f;
        enemy = GetComponent<Enemy>();
    }

    private void FixedUpdate()
    {
        distance = Vector3.Distance(transform.position, playerTransform.position); //目前距離
        switch (nowState)
        {
            case AIState.Idle:
                Debug.Log("玩家離太遠了，切換回待機模式");

                string name = gameObject.name;
                if (distance < detectionRadius)
                {
                    nowState = AIState.Chase;
                }
                else if (waypoints != null && waypoints.Length > 0)
                {
                    nowState = AIState.Patrol;
                }

                break;

            case AIState.Patrol:
                Debug.Log("巡邏中");
                if (distance < detectionRadius)
                {
                    nowState = AIState.Chase;
                }
                break;

            case AIState.Chase:
                Debug.Log("看到玩家，切換成追逐模式");
                if (ExceedPursueDistance())
                {
                    nowState = AIState.Idle;
                }
                else if (distance < attackRange)
                {
                    nowState = AIState.Combat;
                }


                break;

            case AIState.Combat:
                //Debug.Log("玩家在攻擊圈內，停下開始攻擊");
                Debug.Log("Combat State");
                if (distance <= attackRange)
                {
                    agent.isStopped = true;
                }
                else
                {
                    agent.isStopped = false;
                    if (ExceedPursueDistance())
                    {
                        nowState = AIState.Idle;
                    }
                    else
                    {
                        nowState = AIState.Chase;
                    }
                }
                break;

                //case AIState.Reset:
                //    IfExceedPursueDistanceToIdle();
                //    break;
        }

    }

    void Update()
    {
        if (nowState == AIState.Idle)
        {
            Idle();
        }
        else if (nowState == AIState.Patrol)
        {
            Patrol();
        }
        else if (nowState == AIState.Chase)
        {
            MoveTowardPlayer();
        }
        else if (nowState == AIState.Combat)
        {

        }
        else if (nowState == AIState.Die)
        {

        }
    }
    void Patrol()
    {
        Debug.Log("hasPath" + agent.hasPath);
        if (!agent.hasPath)
        {
            string name = gameObject.name;
            currentWaypointsIndex = currentWaypointsIndex + 1;
            if (currentWaypointsIndex > waypoints.Length - 1)
            {
                currentWaypointsIndex = 0;
            }
            MoveToPosition(waypoints[currentWaypointsIndex].position, moveSpeed);
        }

    }

    bool ExceedPursueDistance()
    {
        float playerDistanceSpawnLoction = Vector3.Distance(spawnLoction, playerTransform.position);
        return playerDistanceSpawnLoction > stopPursueDistance;
    }
    void MoveToPosition(Vector3 targetPosition, float speed)
    {
        bool canUseAgent = CanUseNavMeshAgent() || TrySnapAgentToNavMesh();
        if (canUseAgent)
        {
            agent.speed = speed;
            agent.isStopped = false;
            //Vector3 directionToTarget = targetPosition - transform.position;
            //Vector3 normalizedDirection = directionToTarget.normalized;
            //Vector3 newDirection = Vector3.RotateTowards(transform.forward, normalizedDirection, Time.deltaTime * 2f, 0.0f);
            //transform.position += normalizedDirection * speed * Time.deltaTime;
            //transform.rotation = Quaternion.LookRotation(newDirection);
            if (agent.SetDestination(targetPosition))
                return;
            WarnMoveIssue($"NavMeshAgent.SetDestination 失敗，改用手動位移：{name}");
        }
    }
    private bool CanUseNavMeshAgent()
    {
        return agent != null
            && agent.enabled
            && agent.isActiveAndEnabled
            && agent.isOnNavMesh;
    }

    private bool TrySnapAgentToNavMesh()
    {
        if (agent == null || !agent.enabled || !agent.isActiveAndEnabled)
            return false;

        if (agent.isOnNavMesh)
            return true;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            if (agent.isOnNavMesh)
                return true;
        }

        WarnMoveIssue($"NavMeshAgent 不在 NavMesh 上：{name}");
        return false;

    }
    private void WarnMoveIssue(string message)
    {
        //if (Time.time < _nextMoveWarningTime) return;
        //_nextMoveWarningTime = Time.time + 2f;
        Debug.LogWarning($"{message}", gameObject);
    }
    private void MoveTowardPlayer()
    {
        if (playerTransform == null || enemy.IsDead()) return;

        Vector3 targetPosition = GetSurroundSlotPosition();

        MoveToPosition(targetPosition, runSpeed);
    }
    private Vector3 GetSurroundSlotPosition()
    {
        if (playerTransform == null) return transform.position;

        float desiredRadius = Mathf.Max(attackRange * surroundRadiusMultiplier, 1f);
        Vector3 slotDir = Quaternion.Euler(0f, _surroundAngleOffset, 0f) * Vector3.forward;
        Vector3 slotPosition = playerTransform.position + slotDir * desiredRadius;
        slotPosition.y = playerTransform.position.y;
        return slotPosition;
    }

    private void Idle()
    {
        if (agent.hasPath)
        {
            agent.ResetPath();
        }
        //if (CanUseNavMeshAgent())
        //{
        //    navMeshAgent.isStopped = true;
        //    navMeshAgent.SetDestination(transform.position);
        //}
    }

    // ===================== Gizmos =====================

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(spawnLoction, stopPursueDistance);
        //Gizmos.color = Color.green;
        //Vector3 leftDirection = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward * detectionRadius;
        //Vector3 rightDirection = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward * detectionRadius;
        //Gizmos.DrawLine(transform.position, transform.position + leftDirection);
        //Gizmos.DrawLine(transform.position, transform.position + rightDirection);
    }

}
