using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDelegates : MonoBehaviour
{
    public static PlayerDelegates Instance { get; private set; }
    public Action OnSkillChange;

    public Action OnAttackStart;
    public Action OnAttackCancel;
    public Action OnLightAttack;
    public Action OnHeavyAttack;
    public Action OnBlockStart;
    public Action OnBlockCancel;
    public Action OnSkillStart;
    public Action OnSkillCancel;
    public Action OnBreakFree;

    public Action<SkillSet> OnPlayerSkillsLoaded;
    public Action<SkillSet, int> OnPlayerSkillChange;
    public Action<SkillSet, SkillData> OnSkillCooldownStarted;
    public Action<SkillSet, SkillData> OnSkillInterrupted;
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
    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
