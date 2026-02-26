using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIActionBar : MonoBehaviour
{
    public GameObject go_QuickSlot;
    public GameObject go_Skills;
    public GameObject go_UltimateSlot;
    public List<GameObject> SkillSlots;
    private SkillSet playerSkillSet;
    private PlayerInput PlayerInput;
    private SkillController SkillController;
    private InputActionAsset InputActionAsset;
    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerSkillSet = PlayerData.Instance.SkillSet;
        PlayerInput = PlayerData.Instance.PlayerInput;
        InputActionAsset = PlayerData.Instance.InputActionAsset;
        SkillController = PlayerData.Instance.SkillController;
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
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
    void StartCountDown(SkillData data)
    {
        int index = playerSkillSet.GetSkillIndexBySkillId(data.id);
        SkillSlot skillSlot = SkillSlots[index].GetComponent<SkillSlot>();
        skillSlot.StartCountDown(data.recoveryTime+data.cooldownTime);
    }
    void StopCountDown(SkillData data)
    {
        int index = playerSkillSet.GetSkillIndexBySkillId(data.id);
        SkillSlot skillSlot = SkillSlots[index].GetComponent<SkillSlot>();
        skillSlot.StopCountdown();
    }
    public void InitUIActionBar()
    {
        InitQuickSlot();
        InitSkills();
        InitUltimateSlot();
    }
    void InitQuickSlot()
    {

    }
    void InitSkills()
    {
        if(SkillSlots.Count< 1) throw new System.Exception("SkillSlots©|¥¼½á­È¡I"); ;
        for (int i = 0; i < SkillSlots.Count; i++)
        {
            string actionName = playerSkillSet.GetActionNameBySkillIndex(i);
            string keyName = GetPlayerKeyNameByActionName(actionName);
            SkillSlot skillSlot = SkillSlots[i].GetComponent<SkillSlot>();
            skillSlot.SetControlKey(keyName);
            int skillId = playerSkillSet.GetSkillIdBySkillIndex(i);
            if (skillId == 0) { skillSlot.Disable(); }
            else
            {
                skillSlot.Enable();
                SkillData skillData = AssetDataManager.Instance.GetPlayerSkillById(skillId);
                Debug.Log(skillData);
                skillSlot.SetSkill(skillData);
            }
        }
    }
    void InitUltimateSlot()
    {

    }
    string GetPlayerKeyNameByActionName(string actionName)
    {
        string keyName = InputActionAsset.FindActionMap("Player").FindAction(actionName).GetBindingDisplayString(0);
        return keyName;
    }
}
