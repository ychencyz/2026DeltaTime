using System;
using UnityEngine;

public class PlayerDelegates : MonoBehaviour
{
    public static PlayerDelegates Instance { get; private set; }
    //public Action OnAttackStart;
    //public Action OnAttackCancel;
    //public Action OnLightAttack;
    //public Action OnHeavyAttack;
    //public Action OnBlockStart;
    //public Action OnBlockCancel;
    public Action<SkillSet, SkillData> OnSkillStart;
    public Action<SkillSet, SkillData> OnSkillExecute;
    public Action<SkillSet, SkillData> OnSkillInterrupted;
    public Action OnBash;
    public Action<String> OnRollDodgeStart;
    public Action OnRollDodgeEnd;
    //public Action OnBreakFree;
    //UI related
    public Action<SkillSet> OnPlayerSkillsLoaded;
    public Action<SkillSet, int> OnPlayerSkillChange;
    public Action<SkillSet, SkillData> OnSkillCooldownStarted;
    public Action<Player> OnPlayerResourceChange;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
