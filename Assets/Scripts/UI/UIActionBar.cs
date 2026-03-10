using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIActionBar : MonoBehaviour
{
    public GameObject go_QuickSlot;
    public GameObject go_Skills;
    public GameObject go_UltimateSlot;
    public List<GameObject> ActionBarSkillSlots;

    private void OnEnable()
    {
        PlayerDelegates.Instance.OnSkillCooldownStarted += StartCountDown;
        PlayerDelegates.Instance.OnSkillInterrupted += StopCountDown;
    }
    private void OnDisable()
    {
        PlayerDelegates.Instance.OnSkillCooldownStarted -= StartCountDown;
        PlayerDelegates.Instance.OnSkillInterrupted -= StopCountDown;
    }
    void StartCountDown(SkillSet skillSet, SkillData data)
    {
        int index = skillSet.GetSkillIndexBySkillId(data.id);
        if (index > -1)
        {
            SkillSlot skillSlot = ActionBarSkillSlots[index].GetComponent<SkillSlot>();
            skillSlot.StartCountDown(data.recoveryTime + data.cooldownTime);
        }
    }
    void StopCountDown(SkillSet skillSet, SkillData data)
    {
        int index = skillSet.GetSkillIndexBySkillId(data.id);
        SkillSlot skillSlot = ActionBarSkillSlots[index].GetComponent<SkillSlot>();
        skillSlot.StopCountdown();
    }
    void InitQuickSlot(GameObject player)
    {

    }


    void InitUltimateSlot(GameObject player)
    {

    }
}
