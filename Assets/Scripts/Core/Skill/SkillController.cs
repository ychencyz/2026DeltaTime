// --- Unity 實戰架構範例 ---

using UnityEngine;
using System.Collections;



// 2. 生命週期枚舉
public enum SkillState { Idle, Anticipation, Execution, Recovery, Cooldown }

// 3. 核心控制器
public class SkillController : MonoBehaviour
{
    public SkillData data;
    private SkillState currentState = SkillState.Idle;
    private Coroutine skillRoutine;
    // 外部進入點
    public void TryCast()
    {
        // 狀態裁判 Gate: 檢查是否能施法
        if (currentState != SkillState.Idle) return;
        if (GetComponent<StatusSystem>().isStunned) return;

        skillRoutine = StartCoroutine(SkillLifecycle());
    }
    private IEnumerator SkillLifecycle()
    {
        // [Anticipation] 前搖
        currentState = SkillState.Anticipation;
        yield return new WaitForSeconds(data.anticipationTime);

        // [Execution] 執行核心邏輯
        currentState = SkillState.Execution;
        ExecuteProjectile();

        // [Recovery] 後搖
        PlayerDelegates.Instance.OnSkillCooldownStarted?.Invoke(data);
        currentState = SkillState.Recovery;
        yield return new WaitForSeconds(data.recoveryTime);

        // [Cooldown] 冷卻啟動
        StartCoroutine(CooldownRoutine());
    }
    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(data.cooldownTime);
        currentState = SkillState.Idle;
    }
    public void Interrupt()
    {
        if (currentState == SkillState.Anticipation)
        {
            PlayerDelegates.Instance.OnSkillInterrupted?.Invoke(data);
            StopCoroutine(skillRoutine);
            currentState = SkillState.Idle;
            // 清理已生成的特效...
        }
    }
    private void ExecuteProjectile()
    {
        // 實作投射物生成，注意處理 NullReferenceException
        GameObject reference = data.projectilePrefab;
        GameObject obj = Instantiate(reference, Vector3.zero, Quaternion.identity);
        float animationDuration = obj.GetComponent<ParticleSystem>().main.duration;
        Destroy(obj, animationDuration);
        
        Debug.Log("Fire!");
    }
}