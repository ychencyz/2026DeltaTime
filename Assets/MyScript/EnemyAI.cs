using System;
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
    }
    public AIState nowState = AIState.Idle;
    private AIState prevState;
    public Transform[] waypoints;
    private int currentWaypointsIndex = -1; //選到第幾個waypoint
    private NavMeshAgent agent;
    //public Transform playerTransform;
    public float moveSpeed = 2f;
    public float runSpeed = 5f;
    //public float waitTimeMove = 2f;
    public float attackRange;
    public float detectionRadius;
    [Header("離player的distance")]
    [SerializeField] private float distance;
    //public float stopPursueDistance = 15f; //超過出生點到不追逐的距離
    private Enemy enemy;
    private GameObject playerObject;
    private Transform playerTransform;
    private Vector3 spawnLoction; //出生點
    private CharacterIdentifier characterIdentifier;
    //private float viewAngle = 360f;

    [Header("群體站位")]
    [SerializeField] private float surroundRadiusMultiplier = 0.9f;
    private float _surroundAngleOffset;

    [Header("skillController")]
    private EnemySkillController skillController;
    private EnemySkillSet skillSet;

    //冷卻時間
    private float nextFireTime;  //下次能發動的時間

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //playerObject = GameObject.FindWithTag("Player"); //找到玩家
        playerObject = GameManager.Instance.Go_Player; //找到玩家
        playerTransform = playerObject.transform;
        spawnLoction = transform.position;
        _surroundAngleOffset = Mathf.Abs(GetInstanceID()) % 360f; //防止所有敵人都擠在同一個點上
        enemy = GetComponent<Enemy>();
        skillController = GetComponent<EnemySkillController>();
        skillSet = GetComponent<EnemySkillSet>();
        characterIdentifier = GetComponent<CharacterIdentifier>();
        attackRange = characterIdentifier.data.attackRange;
        detectionRadius = characterIdentifier.data.detectionRadius;
    }

    private void FixedUpdate()
    {
        distance = Vector3.Distance(transform.position, playerTransform.position); //目前距離

        Vector3 targetVector = playerTransform.position - transform.position;
        transform.forward = Vector3.RotateTowards(transform.forward, targetVector, 0.05f, 0.05f);

        switch (nowState)
        {
            case AIState.Idle:
                //if (prevState == AIState.Idle) return;
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
                else
                {
                    prevState = AIState.Idle;
                }
                break;

            case AIState.Patrol:
                //if (prevState == AIState.Patrol) return;
                Debug.Log("巡邏中");
                if (distance < detectionRadius)
                {
                    nowState = AIState.Chase;
                }
                else
                {
                    prevState = AIState.Patrol;
                }
                break;

            case AIState.Chase:
                //if (prevState == AIState.Chase) return;
                if (prevState != AIState.Chase)
                {
                    agent.ResetPath();
                }
                Debug.Log("看到玩家，切換成追逐模式");
                //if (ExceedPursueDistance())
                //{
                //    nowState = AIState.Idle;
                //} else
                if (distance < attackRange)
                {
                    nowState = AIState.Combat;
                }
                else
                {
                    prevState = AIState.Chase;
                }
                break;

            case AIState.Combat:
                //if (prevState == AIState.Combat) return;
                //Debug.Log("玩家在攻擊圈內，停下開始攻擊");
                Debug.Log("Combat State");
                if (distance <= attackRange)
                {
                    agent.isStopped = true;
                    prevState = AIState.Combat;
                }
                else
                {
                    agent.isStopped = false;
                    nowState = AIState.Chase;

                    //if (ExceedPursueDistance())
                    //{
                    //    nowState = AIState.Idle;
                    //}
                    //else
                    //{
                    //    nowState = AIState.Chase;
                    //}
                }
                break;

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
            Debug.Log(nowState + ": " + Time.time + ":move");
            MoveTowardPlayer();
        }
        else if (nowState == AIState.Combat)
        {
            Combat();
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

    //bool ExceedPursueDistance() //追逐限制距離
    //{
    //    float playerDistanceSpawnLoction = Vector3.Distance(spawnLoction, playerTransform.position);
    //    return playerDistanceSpawnLoction > stopPursueDistance;
    //}
    void MoveToPosition(Vector3 targetPosition, float speed)
    {
        //Vector3 directionToTarget = targetPosition - transform.position;
        //Vector3 normalizedDirection = directionToTarget.normalized;
        //Vector3 newDirection = Vector3.RotateTowards(transform.forward, normalizedDirection, Time.deltaTime * 2f, 0.0f);
        //transform.position += normalizedDirection * speed * Time.deltaTime;
        //transform.rotation = Quaternion.LookRotation(newDirection);
        bool canUseAgent = CanUseNavMeshAgent() || TrySnapAgentToNavMesh();

        if (canUseAgent)
        {
            agent.speed = speed;
            agent.isStopped = false;
            if (agent.SetDestination(targetPosition))
                return;
            //WarnMoveIssue($"NavMeshAgent.SetDestination 失敗：{name}");
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

        //Vector3 targetPosition = GetSurroundSlotPosition();

        //MoveToPosition(targetPosition, runSpeed);
        MoveToPosition(playerTransform.position, runSpeed);
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

    private void Combat()
    {
        switch (characterIdentifier.data.id)
        {
            case 1:
                EnemyAttacks.EnemyOne(skillController, skillSet, distance, nowState, nextFireTime);
                break;

            case 2:
                EnemyAttacks.EnemyTwo(skillController, skillSet, distance, nowState, nextFireTime);
                break;

            default:
                throw new Exception("[characterIdentifier.data.id] not found");
        }

    }


    // ===================== Gizmos =====================

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        //Gizmos.color = Color.black;
        //Gizmos.DrawWireSphere(spawnLoction, stopPursueDistance);
        //Gizmos.color = Color.green;
        //Vector3 leftDirection = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward * detectionRadius;
        //Vector3 rightDirection = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward * detectionRadius;
        //Gizmos.DrawLine(transform.position, transform.position + leftDirection);
        //Gizmos.DrawLine(transform.position, transform.position + rightDirection);
    }

}
