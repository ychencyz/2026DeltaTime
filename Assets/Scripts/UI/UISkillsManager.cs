using System;
using System.Collections.Generic;
using UnityEngine;

public class UISkillsManager : MonoBehaviour
{
    [SerializeField]
    SkillSlot[] allSkillSlots;
    //private List<GameObject> allSkillSlots;
    void Awake()
    {
        allSkillSlots = FindObjectsByType<SkillSlot>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        //foreach (var item in allSkillSlots)
        //{
        //    this.allSkillSlots.Add(item.gameObject);
        //}
    }
    private void OnEnable()
    {
        PlayerDelegates.Instance.OnPlayerSkillsLoaded += InitSkillSetSubscribingSkillSlots;
    }
    private void OnDisable()
    {
        PlayerDelegates.Instance.OnPlayerSkillsLoaded -= InitSkillSetSubscribingSkillSlots;
    }
    public void InitSkillSetSubscribingSkillSlots(SkillSet skillSet)
    {
        if (allSkillSlots.Length < 1) throw new System.Exception("SkillSlots not assigned!"); ;
        for (int i = 0; i < allSkillSlots.Length; i++)
        {
            SkillSlot skillSlot = allSkillSlots[i];
            //set key name for ActionBar & SkillBookPreviewBar
            if (skillSlot.isSubscribingToSkillSet())
            {
                string actionName = skillSet.GetActionNameBySkillIndex(skillSlot.index);
                string keyName = GameRuntimeData.Instance.GetActionMapPlayerKeyNameByActionName(actionName);
                skillSlot.SetControlKey(keyName);
                skillSlot.Disable();
            }
        }
    }
}
