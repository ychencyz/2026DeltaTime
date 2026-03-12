//using UnityEngine;
//using System.Collections;

//public class waypoint : MonoBehaviour
//{
//    public Transform[] waypoints; //巡邏點陣列
//    public float detectionRadius = 10f; //偵測玩家的圓形半徑：怪物能看到玩家的最遠距離
//    public LayerMask playerLayer; //指定玩家

//    [Header("ESO 特效設定")]
//    public GameObject aoePrefab; // 這裡之後要拖入你做的 Prefab
//    public float visualDuration = 1.0f; // 圈圈顯示多久 (比攻擊間隔短一點)

//    [Header("戰鬥數值")]
//    public int damagePower = 10;

//    [Header("戰鬥設定")]
//    public bool isRangedUnit = false; //切換近戰和遠戰模式
//    public float attackRange = 2f; //攻擊距離:手要多長
//    public float stopRange = 1.5f; //停止移動距離(靠玩家多近時需要停下來)
//    public int unitIndex = 0;  //計算陣行位置，決定佔位，避免重疊

//    [Header("蓄力設定")]
//    public float chargeTime = 1.0f; // 攻擊前的蓄力時間(紅圈讀條)
//    private bool isCharging = false; // 是否正在蓄力

//    [Header("小隊設定")]
//    public Transform leaderTransform; //如果有數值，代表是隊員，須跟著隊長走

//    [Header("狀態監控")]
//    public Transform targetPlayer; //目前鎖定的目標玩家
//    private bool isRetreating = false; //是否在撤退(還沒做完)
//    private bool isCurrentlyAttacking = false; // 判斷是否正在攻擊，(畫 Gizmos 顏色判斷)

//    private AStarMover aStarMover; //引用另一個A* 尋路腳本
//    private float attackTimer; //紀錄下次可以攻擊的時間點（冷卻計時）
//    private Rigidbody rb; //處理物理碰撞

//    void Awake()
//    {
//        aStarMover = GetComponent<AStarMover>();
//        rb = GetComponent<Rigidbody>();
//        var nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
//        if (nav != null) nav.enabled = false; //強制關閉NavMeshAgent(內建導航)，避免衝突
//    }

//    void Start()
//    {
//        if (leaderTransform == null)
//            SyncWaypointsToMover(); //如果沒有隊長，把巡邏點交給尋路系統
//        else
//            if (aStarMover != null) aStarMover.enabled = false; //如果有隊長，就關閉astarmover，由隊長管理跟隨
//    }

//    void Update()
//    {
//        DetectPlayer(); //掃描範圍裡有無玩家
//        if (aStarMover == null) return;  // 如果沒抓到尋路組件，就停止執行，防止出錯。

//        // --- 蓄力時禁止一切動作 ---
//        if (isCharging) //如果正在蓄力
//        {
//            StopMovement(); //停止移動
//            return; // 直接跳過後續所有邏輯，讓怪物站著不動。
//        }
//        if (targetPlayer != null) //如果有目標玩家，進入戰鬥狀態
//        {
//            isRetreating = false;
//            float dist = Vector3.Distance(transform.position, targetPlayer.position); //計算跟玩家的距離
//            FaceTarget(targetPlayer.position); // 永遠面向玩家

//            // 判斷是否進入攻擊範圍
//            if (dist <= attackRange)
//            {
//                isCurrentlyAttacking = true;
//                ExecuteAttack(); //進入攻擊流程
//            }
//            else
//            {
//                isCurrentlyAttacking = false;
//            }

//            // 判斷移動
//            if (dist <= stopRange)
//            {
//                StopMovement();//如果夠近就停下來
//            }
//            else
//            {
//                aStarMover.enabled = false; //戰鬥中不使用A*自動巡邏
//                MoveToPosition(CalculateSurroundPos(), 10f); //往包圍玩家的位置移動
//            }
//            //判斷玩家逃跑?
//            if (dist > detectionRadius * 1.5f) ResetAllMembers(); //離太遠就散
//        }
//        else  //如果沒有目標玩家(非戰鬥模式)
//        {
//            isCurrentlyAttacking = false;
//            if (leaderTransform != null) //如果不是隊長
//            {
//                isRetreating = false;
//                aStarMover.enabled = false;
//                MoveToPosition(CalculateFollowPos(), 5f); //就跟在隊長後面
//            }
//            else
//            {
//                isRetreating = false;
//                aStarMover.enabled = true; //跟A*巡邏點移動
//            }
//        }
//    }

//    private void OnDrawGizmos()
//    {
//        if (isCurrentlyAttacking)
//        {
//            Gizmos.color = Color.green; // 攻擊中：綠色
//        }
//        else if (targetPlayer != null)
//        {
//            Gizmos.color = Color.red;   // 偵測到玩家：紅色
//        }
//        else
//        {
//            Gizmos.color = Color.blue;  // 巡邏中：藍色
//        }

//        // 畫出偵測半徑
//        Gizmos.DrawWireSphere(transform.position, detectionRadius);

//        // 畫出攻擊範圍
//        Gizmos.DrawWireSphere(transform.position, attackRange);
//    }

//    void ExecuteAttack() //攻擊邏輯
//    {
//        if (Time.time >= attackTimer) //如果冷卻時間到
//        {
//            StartCoroutine(ChargeAndAttack()); //開始跑蓄力攻擊
//            if (aoePrefab != null)
//            {
//                // 在怪物腳下生成特效
//                GameObject v = Instantiate(aoePrefab, transform.position, transform.rotation);
//                // 取得特效腳本並設定：(範圍, 是否為扇形, 角度)
//                // 如果是遠程怪(isRangedUnit)，畫圓圈；否則畫 90 度扇形
//                //v.GetComponent<AOEVisual_Solid>().Setup(attackRange, !isRangedUnit, 90f);
//            }
//            string unitType = isRangedUnit ? "遠程射擊" : "近戰揮砍";
//            // 使用富文本讓 Log 更明顯
//            Debug.Log($"<color={(isRangedUnit ? "cyan" : "orange")}>{gameObject.name} 執行：{unitType}攻擊！</color>");

//            attackTimer = Time.time + 1.5f; // 設定下次攻擊要等1.5秒
//        }

//        IEnumerator ChargeAndAttack() //這是協程，可以處理時間等待
//        {
//            isCharging = true; //設定為蓄力狀態


//            if (aoePrefab != null) // 在自己腳下生成紅圈預警特效
//            {
//                GameObject v = Instantiate(aoePrefab, transform.position, transform.rotation);
//                //var visual = v.GetComponent<AOEVisual_Solid>();
//                //if (visual != null) visual.Setup(attackRange, !isRangedUnit, 90f);
//            }

//            //等待蓄力時間 (讓怪物停在原地)
//            yield return new WaitForSeconds(chargeTime);

//            //蓄力結束，執行真正的攻擊動作
//            string unitType = isRangedUnit ? "遠程射擊" : "近戰揮砍";
//            Debug.Log($"<color={(isRangedUnit ? "cyan" : "orange")}>{gameObject.name} 蓄力完成，發動：{unitType}！</color>");

//            isCharging = false; // 結束蓄力，恢復移動能力
//        }

//    }

//    void MoveToPosition(Vector3 targetPos, float rotationSpeed) //具體移動與旋轉
//    {
//        if (rb != null && !rb.isKinematic) { rb.linearVelocity = Vector3.zero; }
//        // 如果怪物有 Rigidbody (剛體)，且不是 Kinematic (動力學)，就強制把速度歸零

//        float step = aStarMover.moveSpeed * Time.deltaTime;
//        // 計算這一幀應該移動的距離 = 速度 * 時間變化量 (確保在不同幀數下移動距離一致)

//        Vector3 movePos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
//        // 計算最終座標：只取目標的 X 和 Z，高度 (Y) 維持自己目前的高度

//        transform.position = Vector3.MoveTowards(transform.position, movePos, step);
//        // 實際搬移位置：從「現在位置」朝「目標位置」移動一個「step」的距離

//        Vector3 dir = (movePos - transform.position).normalized;
//        // 計算方向向量：目標位置-現在位置

//        if (dir != Vector3.zero) // 如果方向不為零 (代表沒抵達目的地)，就執行旋轉
//            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotationSpeed);
//    }

//    Vector3 CalculateFollowPos()  //計算跟隨隊長的位置
//    {
//        float row = Mathf.Ceil(unitIndex / 2f);
//        // 計算排數：每 2 個人一排

//        float side = (unitIndex % 2 == 0) ? 1f : -1f;
//        // 計算左邊還是右邊：編號偶數在右 (1)，奇數在左 (-1)。

//        Vector3 offset = (leaderTransform.forward * -1.5f * row) + (leaderTransform.right * side * 1.5f);
//        // 計算偏移量：(隊長後方 * 1.5公尺 * 第幾排) + (隊長右方 * 左右側 * 1.5公尺)
//        return leaderTransform.position + offset;
//        // 返回最終目標點：隊長現在的位置 + 計算出的偏移位子
//    }

//    void ResetAllMembers() //全體脫戰，當玩家跑太遠時，讓所有怪物一起恢復和平
//    {
//        waypoint[] allEnemies = Object.FindObjectsByType<waypoint>(FindObjectsSortMode.None);
//        // 在場景中找出所有掛著 waypoint 腳本的物件
//        foreach (waypoint enemy in allEnemies) enemy.targetPlayer = null;
//        // 把每個怪物的「目標玩家」都清空，讓大家回去巡邏
//    }

//    public void ReportPlayerFound(Transform player) //回報發現玩家
//    {
//        if (targetPlayer == null) { targetPlayer = player; }
//        // 如果我目前還沒有目標，就設定這個玩家為我的目標
//    }

//    Vector3 CalculateSurroundPos() //計算包圍玩家的位置
//    {
//        float offsetDist = isRangedUnit ? (attackRange * 0.8f) : (stopRange * 0.9f);
//        // 距離設定：遠程怪站在 80% 射程處，近戰怪站在 90% 停止距離處。
//        float angle = unitIndex * 45f;
//        // 計算角度：每個編號各轉 45 度 (0度, 45度, 90度...)。
//        Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * offsetDist;
//        // 算出偏移向量：先朝正前方偏，再旋轉對應的角度。
//        return targetPlayer.position + offset;
//        // 返回最終目標點：玩家位置 + 旋轉後的偏移點。
//    }

//    void StopMovement() //停止移動
//    {
//        aStarMover.enabled = false; //關閉自動尋路
//        if (rb != null && !rb.isKinematic) rb.linearVelocity = Vector3.zero;
//        // 如果有物理剛體，把速度強制歸零，防止滑行
//    }

//    void SyncWaypointsToMover() //同步巡邏點
//    {
//        if (aStarMover != null && waypoints.Length > 0) // 把我們在 Inspector 面板拉的 waypoints 陣列交給尋路腳本
//        {
//            aStarMover.waypoints = waypoints;
//            aStarMover.enabled = true; //啟動尋路
//        }
//    }

//    void FaceTarget(Vector3 targetPos) //一直面向玩家
//    {
//        Vector3 dir = (targetPos - transform.position).normalized; //計算指向目標的方向
//        dir.y = 0; // 強制把 Y 軸設為 0，防止怪物因為玩家跳起來而整隻仰頭
//        if (dir != Vector3.zero) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
//        // 平滑轉向目標
//    }

//    void DetectPlayer() //掃描玩家
//    {
//        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
//        // 在自己周圍畫一個圓球球，看看有沒有碰撞體屬於玩家層級 (playerLayer)
//        if (hits.Length > 0) // 如果球球內有東西 (玩家)
//        {
//            ReportPlayerFound(hits[0].transform); //自己鎖定玩家
//            foreach (var enemy in Object.FindObjectsByType<waypoint>(FindObjectsSortMode.None))
//                enemy.ReportPlayerFound(hits[0].transform);
//            //找出地圖上所有同樣掛著 waypoint 腳本的怪，叫大家一起過來打
//        }
//    }
//    IEnumerator ChargeAndAttack()
//    {
//        isCharging = true;
//        // ... 生成預警紅圈特效 ...
//        yield return new WaitForSeconds(chargeTime);

//        // --- 這裡新增傷害判定 ---
//        // 在攻擊瞬間檢查玩家是否還在範圍內
//        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, attackRange, playerLayer);
//        foreach (var p in hitPlayers)
//        {
//            var h = p.GetComponent<HealthSystem>();
//            if (h != null) h.TakeDamage(damagePower);
//        }

//        isCharging = false;
//    }
//}
