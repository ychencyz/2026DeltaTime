using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class EnemyPatrol : MonoBehaviour, ICombatTarget
{
    [Header("巡邏設置")]
    [SerializeField] private WaypointMarker[] waypoints; //路點陣列
    [SerializeField] private bool useWaypoints = true; //是否使用路點
    [SerializeField] private float patrolSpeed = 2f; //巡邏速度
    [SerializeField] private float stopDistance = 0.5f; //停止距離

    [Header("檢測")]
    [SerializeField] private float detectionRange = 20f;  //檢測範圍
    [SerializeField] private float attackRange = 2f; //攻擊範圍
    [SerializeField] private float viewAngle = 120f; //視角範圍

    [Header("攻擊")]
    [SerializeField] private float attackCooldown = 2f; //攻擊冷卻時間
    [SerializeField] private float attackDamage = 15f; //攻擊傷害
    [SerializeField] private float attackDelay = 0.5f;  //攻擊延遲

    [Header("群體站位")]
    [SerializeField] private bool useSurroundSlot = true;
    [SerializeField] private float surroundRadiusMultiplier = 0.9f;

    [Header("攻擊前搖 / Decal")]
    [SerializeField] private GameObject attackTelegraphPrefab;
    [SerializeField] private Vector3 attackTelegraphOffset = new Vector3(0f, 0.05f, 0f);

    [Header("防禦")]
    [SerializeField] private float defense = 5f;

    private Enemy enemyComponent;
    private CharacterIdentifier characterIdentifier;
    private Transform playerTransform;
    private ICombatTarget playerTarget; // 改用 ICombatTarget 介面
    //private UIFloatingBarElement floatingBar;
    private NavMeshAgent navMeshAgent;

    private int currentWaypointIndex = 0;
    private float waitAtWaypointTime = 0f;
    private bool isWaiting = false;
    private bool playerDetected = false;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    private IEnumerator attackCoroutine;

    //private int _hp;
    //private int _maxHp;
    private bool _isDead = false;
    private Vector3 _spawnPosition;
    private float _nextMoveWarningTime = 0f;
    private float _surroundAngleOffset;

    private float distanceToPlayer;
    private EnemySkillController skillController;
    private EnemySkillSet skillSet;
    private void Start()
    {
        _spawnPosition = transform.position;
        enemyComponent = GetComponent<Enemy>();
        characterIdentifier = GetComponent<CharacterIdentifier>();
        //floatingBar = GetComponent<UIFloatingBarElement>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        _surroundAngleOffset = Mathf.Abs(GetInstanceID()) % 360f;
        skillController = GetComponent<EnemySkillController>();
        skillSet = GetComponent<EnemySkillSet>();
        if (navMeshAgent != null)
        {
            navMeshAgent.avoidancePriority = Random.Range(20, 80);
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            navMeshAgent.stoppingDistance = Mathf.Max(stopDistance, attackRange * 0.75f);
        }

        if (useWaypoints)
        {
            //if (waypoints == null || waypoints.Length == 0)
            //    waypoints = GetComponentsInChildren<WaypointMarker>();

            if (waypoints == null || waypoints.Length == 0)
                Debug.LogWarning($"[{nameof(EnemyPatrol)}] 找不到任何 WaypointMarker：{name}", gameObject);
        }

        //if (characterIdentifier != null && characterIdentifier.data != null)
        //{
        //    _hp = characterIdentifier.data.initialHp;
        //    _maxHp = characterIdentifier.data.initialMaxHp;
        //}
        //else if (enemyComponent != null)
        //{
            //_hp = enemyComponent.hp;
            //_maxHp = enemyComponent.maxHp;
        //}

        //if (floatingBar != null && !floatingBar.initialized)
        //{
        //    floatingBar.SetInitialHp(_hp, _maxHp);
        //    if (characterIdentifier != null && characterIdentifier.data != null)
        //        floatingBar.SetInitialName(characterIdentifier.data.displayName);
        //    if (UIData.Instance != null)
        //        floatingBar.Init(floatingBar.go_bar);
        //}

        //if (_maxHp <= 0) _maxHp = 100;
        //if (_hp <= 0) _hp = _maxHp;

        FindPlayer();
    }

    private void FixedUpdate()
    {
        if (_isDead) return;
        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        playerDetected = CanSeePlayer();
    }
    private void Update()
    {
        if (_isDead) return;
        //if (playerTransform == null)
        //    FindPlayer();

        //if (playerTransform == null) return;

        //playerDetected = CanSeePlayer();

        if (playerDetected)
        {
            //float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackRange)
                TryAttack();
            else
                MoveTowardPlayer();
        }
        else
        {
            if (useWaypoints)
                PatrolWaypoints();
            else
                Idle();
        }
    }

    // ===================== 尋找玩家 =====================

    private void FindPlayer()
    {
        // 方法1: 透過 PlayerManager
        PlayerManager pm = PlayerManager.Instance;
        //if (pm != null && pm.go_Player != null)
        //{
        playerTransform = pm.go_Player.transform;
        //}
        //else
        //{
        //    // 方法2: 透過 Tag
        //    GameObject player = GameObject.FindWithTag("Player");
        //    if (player != null)
        //        playerTransform = player.transform;
        //}

        //if (playerTransform == null) return;

        //// 在玩家的整個階層中搜尋 ICombatTarget
        //// 先查自身，再查子物件，再查 root 往下
        ///
        //playerTarget = playerTransform.GetComponent<ICombatTarget>();
        playerTarget = playerTransform.GetComponent<ICombatTarget>();
        //if (playerTarget == null)
        //    playerTarget = playerTransform.GetComponentInChildren<ICombatTarget>();
        //if (playerTarget == null)
        //    playerTarget = playerTransform.root.GetComponentInChildren<ICombatTarget>();

        //if (playerTarget == null)
        //    Debug.LogWarning($"[{name}] 找不到玩家的 ICombatTarget，敵人無法造成傷害！", gameObject);
    }

    // ===================== 偵測 =====================

    private bool CanSeePlayer()
    {
        if (playerTransform == null) return false;

        //float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > detectionRange) return false;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > viewAngle / 2f) return false;

        return true;
    }

    // ===================== 巡邏 =====================

    private void PatrolWaypoints()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (currentWaypointIndex < 0 || currentWaypointIndex >= waypoints.Length)
            currentWaypointIndex = 0;

        WaypointMarker currentWaypoint = waypoints[currentWaypointIndex];
        if (currentWaypoint == null)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            return;
        }

        Vector3 waypointPosition = currentWaypoint.GetPosition();
        float distanceToWaypoint = Vector3.Distance(transform.position, waypointPosition);

        if (distanceToWaypoint <= stopDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitAtWaypointTime = Time.time + currentWaypoint.waitTime;
                if (navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.isActiveAndEnabled)
                    navMeshAgent.isStopped = true;
            } 

            if (Time.time >= waitAtWaypointTime)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                isWaiting = false;
                if (navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.isActiveAndEnabled)
                    navMeshAgent.isStopped = false;
            }
        }
        else
        {
            isWaiting = false;
            MoveToPosition(waypointPosition, patrolSpeed);
        }
    }

    private void Idle()
    {
        if(navMeshAgent.hasPath)
        {
            navMeshAgent.ResetPath();
        }
        //if (CanUseNavMeshAgent())
        //{
        //    navMeshAgent.isStopped = true;
        //    navMeshAgent.SetDestination(transform.position);
        //}
    }

    // ===================== 移動 =====================

    private void MoveToPosition(Vector3 targetPosition, float speed)
    {
        //Vector3 directionToTarget = targetPosition - transform.position;
        //if (directionToTarget.sqrMagnitude > 0.0001f)
        //{
        //    Vector3 normalizedDirection = directionToTarget.normalized;
        //    Vector3 newDirection = Vector3.RotateTowards(transform.forward, normalizedDirection, Time.deltaTime * 2f, 0.0f);
        //    transform.rotation = Quaternion.LookRotation(newDirection);
        //    transform.position += normalizedDirection * speed * Time.deltaTime;
        //}

        bool canUseAgent = CanUseNavMeshAgent() || TrySnapAgentToNavMesh();
        if (canUseAgent)
        {
            navMeshAgent.speed = speed;
            navMeshAgent.isStopped = false;
            if (navMeshAgent.SetDestination(targetPosition))
                return;

            WarnMoveIssue($"NavMeshAgent.SetDestination 失敗，改用手動位移：{name}");
        } else
        {
            Vector3 directionToTarget = targetPosition - transform.position;
            Vector3 normalizedDirection = directionToTarget.normalized;
                transform.position += normalizedDirection * speed * Time.deltaTime;
        }

        //if (directionToTarget.sqrMagnitude > 0.0001f)
        //{
        //    Vector3 normalizedDirection = directionToTarget.normalized;
        //    transform.position += normalizedDirection * speed * Time.deltaTime;
        //}
    }

    private bool CanUseNavMeshAgent()
    {
        return navMeshAgent != null
            && navMeshAgent.enabled
            && navMeshAgent.isActiveAndEnabled
            && navMeshAgent.isOnNavMesh;
    }

    private bool TrySnapAgentToNavMesh()
    {
        if (navMeshAgent == null || !navMeshAgent.enabled || !navMeshAgent.isActiveAndEnabled)
            return false;

        if (navMeshAgent.isOnNavMesh)
            return true;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
        {
            navMeshAgent.Warp(hit.position);
            if (navMeshAgent.isOnNavMesh)
                return true;
        }

        WarnMoveIssue($"NavMeshAgent 不在 NavMesh 上：{name}");
        return false;
    }

    private void WarnMoveIssue(string message)
    {
        if (Time.time < _nextMoveWarningTime) return;
        _nextMoveWarningTime = Time.time + 2f;
        Debug.LogWarning($"[{nameof(EnemyPatrol)}] {message}", gameObject);
    }

    private void MoveTowardPlayer()
    {
        if (playerTransform == null || _isDead) return;

        Vector3 targetPosition = playerTransform.position;
        if (useSurroundSlot)
            targetPosition = GetSurroundSlotPosition();

        MoveToPosition(targetPosition, patrolSpeed * 1.5f);
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

    // ===================== 攻擊 =====================

    private void TryAttack()
    {
        if (isAttacking || Time.time - lastAttackTime < attackCooldown)
            return;
        AttackPlayer();
    }

    private void AttackPlayer()
    {
        //// 沒有 playerTarget 也要嘗試再找一次
        //if (playerTarget == null)
        //{
        //    FindPlayer();
        //    if (playerTarget == null)
        //    {
        //        Debug.LogWarning($"[{name}] 找不到玩家 ICombatTarget，無法攻擊", gameObject);
        //        return;
        //    }
        //}

        lastAttackTime = Time.time;
        skillController.TryCast(skillSet.light_attack);
        //if (attackCoroutine != null)
        //    StopCoroutine(attackCoroutine);

        //attackCoroutine = PerformAttack();
        //StartCoroutine(attackCoroutine);
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        transform.LookAt(playerTransform);

        // 1. 先生成 Decal Projector 預警
        SpawnAttackTelegraph();
        Debug.Log($"{characterIdentifier?.data?.displayName} 攻擊玩家!", gameObject);

        // 2. 等待攻擊延遲（前搖）
        yield return new WaitForSeconds(attackDelay);

        // 3. 前搖結束，判定是否命中並扣血
        if (playerTarget != null && playerTransform != null && !playerTarget.IsDead())
        {
            //float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackRange + 0.25f)
            {
                playerTarget.TakeDamage(attackDamage, transform);
                Debug.Log($"{characterIdentifier?.data?.displayName} 命中玩家! 傷害: {attackDamage}", gameObject);
            }
            else
            {
                Debug.Log($"{characterIdentifier?.data?.displayName} 的攻擊被玩家閃開了。", gameObject);
            }
        }

        isAttacking = false;
    }

    // ===================== Decal 預警 =====================

    private void SpawnAttackTelegraph()
    {
        if (attackTelegraphPrefab == null || playerTransform == null)
            return;

        Vector3 spawnPosition = playerTransform.position;
        spawnPosition.y = transform.position.y;

        RaycastHit hit;
        if (Physics.Raycast(playerTransform.position + Vector3.up * 2f, Vector3.down, out hit, 10f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            spawnPosition = hit.point;

        spawnPosition += attackTelegraphOffset;

        Quaternion spawnRotation = attackTelegraphPrefab.transform.rotation;
        GameObject telegraphInstance = Instantiate(attackTelegraphPrefab, spawnPosition, spawnRotation);
        ESOTelegraphController telegraphController = telegraphInstance.GetComponent<ESOTelegraphController>();
        if (telegraphController != null)
        {
            telegraphController.duration = Mathf.Max(0.05f, attackDelay);
            telegraphController.visualOnly = true;
        }
    }

    // ===================== 受傷 / 死亡 =====================

    public void TakeDamage(float damage, Transform attacker = null)
    {
        if (_isDead) return;

        // 只接受玩家來源傷害
        if (attacker != null)
        {
            if (!attacker.root.CompareTag("Player"))
            {
                return;
            }
        }

        float actualDamage = DamageSystem.Instance != null
            ? DamageSystem.Instance.CalculateDamage(damage, defense)
            : damage;

        //_hp -= (int)actualDamage;
        Enemy test = GetComponent<Enemy>();
        test.hp = test.hp - (int)actualDamage;
        //Debug.Log($"{characterIdentifier?.data?.displayName} 受到傷害: {actualDamage}hp，剩餘血量: {_hp}/{_maxHp}", gameObject);
        Debug.Log($"{characterIdentifier?.data?.displayName} 受到傷害: {actualDamage}hp，剩餘血量: {enemyComponent.hp}/{enemyComponent.maxHp}", gameObject);

        //if (floatingBar != null && floatingBar.initialized)
        //    floatingBar.UpdateHealthBar(_hp, _maxHp);

        // 被打到時立即偵測到玩家（仇恨）
        if (!playerDetected && attacker != null)
        {
            playerDetected = true;
        }

        if (enemyComponent.hp <= 0)
            Die();
    }

    private void Die()
    {
        _isDead = true;
        Debug.Log($"{characterIdentifier?.data?.displayName} 已死亡!", gameObject);
        this.enabled = false;

        if (navMeshAgent != null)
            navMeshAgent.enabled = false;

        //if (floatingBar != null)
        //    floatingBar.DisableBar();

        //Destroy(gameObject, 2f);
    }

    // ===================== ICombatTarget =====================

    public int GetCurrentHP() => enemyComponent.hp;
    public int GetMaxHP() => enemyComponent.maxHp;
    public bool IsDead() => _isDead;
    public Vector3 GetPosition() => transform.position;
    public Transform GetTransform() => transform;

    // ===================== Gizmos =====================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Vector3 leftDirection = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward * detectionRange;
        Vector3 rightDirection = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward * detectionRange;
        Gizmos.DrawLine(transform.position, transform.position + leftDirection);
        Gizmos.DrawLine(transform.position, transform.position + rightDirection);
    }
}
