using System.Collections;
using UnityEngine;

public class EnemySkillController : MonoBehaviour
{
    private EnemyDelegates enemyDelegates;
    private Combat combat;
    private SkillSet skillSet;
    private SkillData data;
    private Animator animator;
    [SerializeField]
    private SkillState skillState;
    private Coroutine skillRoutine;
    private void Awake()
    {
        enemyDelegates = GetComponent<EnemyDelegates>();
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
        // 戰鬥姿態 Gate: 未拔劍不能施放技能
        //if (animator != null && !animator.GetBool("inCombat")) return;
        // 狀態判定 Gate: 檢查暈眩Status
        //if (GetComponent<StatusSystem>().isStunned) return;

        // ���A���P Gate: �ˬd���U��skill�O�_��I�k
        if (_data.state != SkillState.Idle) return;
        data = _data;
        skillRoutine = StartCoroutine(SkillLifecycle(data));
    }
    private IEnumerator SkillLifecycle(SkillData data)
    {
        // [Anticipation] �e�n
        skillState = SkillState.Anticipation;
        enemyDelegates.OnSkillStart?.Invoke(data);
        yield return new WaitForSeconds(data.anticipationTime);

        // [Execution] ����֤��޿�
        skillState = SkillState.Execution;
        ExecuteSkill();

        // [Recovery] ��n
        skillState = SkillState.Recovery;
        yield return new WaitForSeconds(data.recoveryTime);

        // [Cooldown] �N�o�Ұ�
        skillState = SkillState.Cooldown;
        StartCoroutine(CooldownRoutine(data));
    }
    private IEnumerator CooldownRoutine(SkillData data)
    {
        yield return new WaitForSeconds(data.cooldownTime);
        skillState = SkillState.Idle;
    }
    public void Interrupt()
    {
        if (skillState == SkillState.Anticipation || skillState == SkillState.Execution)
        {
            enemyDelegates.OnSkillInterrupted?.Invoke(data);
            StopCoroutine(skillRoutine);
            skillState = SkillState.Idle;
            // �M�z�w�ͦ����S��...
        }
    }
    private void ExecuteSkill()
    {
        combat.Attack(data);
    }
}
