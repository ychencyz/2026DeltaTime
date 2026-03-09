// --- Unity 實戰架構範例 ---

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public enum SkillState { Idle, Anticipation, Execution, Recovery, Cooldown }

public class SkillController : MonoBehaviour
{
    private SkillSet skillSet;
    private SkillData data;

    private void Start()
    {
        skillSet = GetComponent<SkillSet>();
    }
    // 外部進入點
    public void TryCast(SkillData _data, Vector3 position)
    {
        // 狀態裁判 Gate: 檢查角色Status
        if (GetComponent<StatusSystem>().isStunned) return;
        // 狀態裁判 Gate: 檢查前一個skill是否能施法
        if (data != null)
        {
            bool prevSkillOk = (data.state == SkillState.Idle || data.state == SkillState.Cooldown);
            //Debug.Log("data.state:" + data.state);
            //Debug.Log("prevSkillOk:" + prevSkillOk);
            if (!prevSkillOk) return;
        }
        // 狀態裁判 Gate: 檢查按下的skill是否能施法
        //Debug.Log("new _data.state:" + _data.state);
        if (_data.state != SkillState.Idle) return;
        data = _data;
        data.position = position;
        data.skillRoutine = StartCoroutine(SkillLifecycle(data));
    }
    private IEnumerator SkillLifecycle(SkillData data)
    {
        // [Anticipation] 前搖
        data.state = SkillState.Anticipation;
        PlayerDelegates.Instance.OnSkillStart?.Invoke(skillSet, data);
        yield return new WaitForSeconds(data.anticipationTime);

        // [Execution] 執行核心邏輯
        data.state = SkillState.Execution;
        ExecuteSkill();

        // [Recovery] 後搖
        PlayerDelegates.Instance.OnSkillCooldownStarted?.Invoke(skillSet, data);
        data.state = SkillState.Recovery;
        yield return new WaitForSeconds(data.recoveryTime);

        // [Cooldown] 冷卻啟動
        data.state = SkillState.Cooldown;
        StartCoroutine(CooldownRoutine(data));
    }
    private IEnumerator CooldownRoutine(SkillData data)
    {
        yield return new WaitForSeconds(data.cooldownTime);
        data.state = SkillState.Idle;
    }
    public void Interrupt()
    {
        if (data.state == SkillState.Anticipation || data.state == SkillState.Execution)
        {
            PlayerDelegates.Instance.OnSkillInterrupted?.Invoke(skillSet, data);
            StopCoroutine(data.skillRoutine);
            data.state = SkillState.Idle;
            // 清理已生成的特效...
        }
    }
    private void ExecuteSkill()
    {
        SetTargetPosition(out data.position);
        if(data.VFXPrefab != null)
        {
            GameObject obj = Instantiate(data.VFXPrefab, data.position, Quaternion.identity);
            float animationDuration = obj.GetComponent<ParticleSystem>().main.duration;
            Destroy(obj, animationDuration);
        }

        Debug.Log($"Skill [{data.displayName}] Excuted!");
    }
    private void SetTargetPosition(out Vector3 targetPosition)
    {
        // TODO: set target position
        targetPosition = new Vector3(-1.59f, 0f, 2.421f);
    }
}