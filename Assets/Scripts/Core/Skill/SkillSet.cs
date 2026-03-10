using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class SkillSet : MonoBehaviour
{
    [SerializeField]
    int _light_attack_id = 4;
    [SerializeField]
    int _skill_1_id = 0;
    [SerializeField]
    int _skill_2_id = 0;
    [SerializeField]
    int _skill_3_id = 0;
    [SerializeField]
    int _skill_4_id = 0;
    [SerializeField]
    int _skill_5_id = 0;
    [SerializeField]
    int _skill_ult_id = 0;

    [Header("Auto Init")]
    public SkillData light_attack;
    //[HideInInspector]
    public SkillData skill_1;
    //[HideInInspector]
    public SkillData skill_2;
    //[HideInInspector]
    public SkillData skill_3;
    //[HideInInspector]
    public SkillData skill_4;
    //[HideInInspector]
    public SkillData skill_5;
    //[HideInInspector]
    public SkillData skill_ult;

    public int light_attack_id
    {
        get => _light_attack_id; set
        {
            light_attack = AssetDataManager.Instance.GetPlayerSkillById(value);
            _light_attack_id = value;
        }
    }
    public int skill_1_id
    {
        get => _skill_1_id; set
        {
            skill_1 = AssetDataManager.Instance.GetPlayerSkillById(value);
            _skill_1_id = value;
            PlayerDelegates.Instance.OnPlayerSkillChange.Invoke(this, 0);
        }
    }
    public int skill_2_id
    {
        get => _skill_2_id; set
        {
            skill_2 = AssetDataManager.Instance.GetPlayerSkillById(value);
            _skill_2_id = value;
            PlayerDelegates.Instance.OnPlayerSkillChange.Invoke(this, 1);
        }
    }
    public int skill_3_id
    {
        get => _skill_3_id; set
        {
            skill_3 = AssetDataManager.Instance.GetPlayerSkillById(value);
            _skill_3_id = value;
            PlayerDelegates.Instance.OnPlayerSkillChange.Invoke(this, 2);
        }
    }
    public int skill_4_id
    {
        get => _skill_4_id; set
        {
            skill_4 = AssetDataManager.Instance.GetPlayerSkillById(value);
            _skill_4_id = value;
            PlayerDelegates.Instance.OnPlayerSkillChange.Invoke(this, 3);
        }
    }
    public int skill_5_id
    {
        get => _skill_5_id; set
        {
            skill_5 = AssetDataManager.Instance.GetPlayerSkillById(value);
            _skill_5_id = value;
            PlayerDelegates.Instance.OnPlayerSkillChange.Invoke(this, 4);
        }
    }
    public int skill_ult_id
    {
        get => _skill_ult_id; set
        {
            skill_ult = AssetDataManager.Instance.GetPlayerSkillById(value);
            _skill_ult_id = value;
            //PlayerDelegates.Instance.OnPlayerSkillChange.Invoke(this, 5);
        }
    }

    private void OnValidate()
    {

        if (AssetDataManager.Instance != null)
        {
            setAllSkils();
        }
    }
    private void Start()
    {
        PlayerDelegates.Instance.OnPlayerSkillsLoaded?.Invoke(this);
        setAllSkils();
    }
    void setAllSkils()
    {
        light_attack_id = _light_attack_id;
        skill_1_id = _skill_1_id;
        skill_2_id = _skill_2_id;
        skill_3_id = _skill_3_id;
        skill_4_id = _skill_4_id;
        skill_5_id = _skill_5_id;
        skill_ult_id = _skill_ult_id;
    }
    public SkillData GetSkillByActionName(string actionName)
    {
        SkillData data = actionName switch
        {
            "LightAttack" => light_attack,
            "Skill1" => skill_1,
            "Skill2" => skill_2,
            "Skill3" => skill_3,
            "Skill4" => skill_4,
            "Skill5" => skill_5,
            _ => throw new Exception("skill Not Found by actionName: " + actionName),
        };
        return data;
    }
    public int GetSkillIdBySkillIndex(int index)
    {
        int skillId = index switch
        {
            0 => skill_1_id,
            1 => skill_2_id,
            2 => skill_3_id,
            3 => skill_4_id,
            4 => skill_5_id,
            5 => skill_ult_id,
            _ => throw new Exception("skill Not Found by index: " + index),
        };
        return skillId;
    }
    public string GetActionNameBySkillIndex(int index)
    {
        string actionName = index switch
        {
            0 => "Skill1",
            1 => "Skill2",
            2 => "Skill3",
            3 => "Skill4",
            4 => "Skill5",
            _ => throw new Exception("skill Not Found"),
        };
        return actionName;
    }
    public int GetSkillIndexBySkillId(int id)
    {
        if (id == skill_1_id) return 0;
        else if (id == skill_2_id) return 1;
        else if (id == skill_3_id) return 2;
        else if (id == skill_4_id) return 3;
        else if (id == skill_5_id) return 4;
        //else if (id == skill_ult_id) return 5;
        else {
            Debug.Log("GetSkillIndexBySkillId: "+ id);
            return -1;
            //throw new Exception("skill Not Found")
        };
    }
}
