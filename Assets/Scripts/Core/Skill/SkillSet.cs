using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillSet : MonoBehaviour
{
    // TODO: make skills map into dictionary
    public int skill_1_id = 0;
    public int skill_2_id = 0;
    public int skill_3_id = 0;
    public int skill_4_id = 0;
    public int skill_5_id = 0;
    public int skill_ult_id = 0;
    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
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
            _ => throw new Exception("skill Not Found"),
        };
        return skillId;
    }
    public int GetSkillIdByActionName(string actionName)
    {
        int skillId = actionName switch
        {
            "Skill1" => skill_1_id,
            "Skill2" => skill_2_id,
            "Skill3" => skill_3_id,
            "Skill4" => skill_4_id,
            "Skill5" => skill_5_id,
            _ => throw new Exception("skill Not Found"),
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
        else throw new Exception("skill Not Found");
    }
}
