// --- Unity ��Ԭ[�c�d�� ---

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public enum SkillState { Idle, Anticipation, Execution, Recovery, Cooldown }

public class SkillController : MonoBehaviour
{
    private Combat combat;
    private SkillSet skillSet;
    public SkillData data { get; private set; }
    private Animator animator;
    public bool prevSkillOk
    {
        get
        {
            if (data == null) return true;
            return (data.state == SkillState.Idle || data.state == SkillState.Cooldown);
        }
        private set { }
    }
    private void Start()
    {
        skillSet = GetComponent<SkillSet>();
        combat = GetComponent<Combat>();
        animator = GetComponent<Animator>();
    }
    // �~���i�J�I
    public void TryCast(SkillData _data)
    {
        //// 戰鬥姿態 Gate: 未拔劍不能施放技能
        //if (animator != null && !animator.GetBool("inCombat")) return;
        // 狀態判定 Gate: 檢查暈眩Status
        if (GetComponent<StatusSystem>().isStunned) return;
        // Check if Prev Skill OK
        //Debug.Log("data.state:" + data.state);
        //Debug.Log("prevSkillOk:" + prevSkillOk);
        if (!prevSkillOk) return;
        // Check if New Skill OK
        //Debug.Log("new _data.state:" + _data.state);
        if (_data.state != SkillState.Idle) return;
        data = _data;
        //Interrupt();
        data.skillRoutine = StartCoroutine(SkillLifecycle(data));
    }
    private IEnumerator SkillLifecycle(SkillData data)
    {
        // [Anticipation] �e�n
        data.state = SkillState.Anticipation;
        PlayerDelegates.Instance.OnSkillStart?.Invoke(skillSet, data);
        yield return new WaitForSeconds(data.anticipationTime);

        // [Execution] ����֤��޿�
        data.state = SkillState.Execution;
        PlayerDelegates.Instance.OnSkillExecute?.Invoke(skillSet, data);
        ExecuteSkill();

        // [Recovery] ��n
        PlayerDelegates.Instance.OnSkillCooldownStarted?.Invoke(skillSet, data);
        data.state = SkillState.Recovery;
        yield return new WaitForSeconds(data.recoveryTime);

        // [Cooldown] �N�o�Ұ�
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
        if (data == null) return;
        if (data.state == SkillState.Anticipation || data.state == SkillState.Execution)
        {
            PlayerDelegates.Instance.OnSkillInterrupted?.Invoke(skillSet, data);
            StopCoroutine(data.skillRoutine);
            data.state = SkillState.Idle;
            // �M�z�w�ͦ����S��...
        }
    }
    public void ForceInterrupt()
    {
        PlayerDelegates.Instance.OnSkillInterrupted?.Invoke(skillSet, data);
        StopCoroutine(data.skillRoutine);
        data.state = SkillState.Idle;
    }
    private void ExecuteSkill()
    {
        combat.Attack(data);
    }
}