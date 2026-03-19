using System;
using UnityEngine;

public class EnemySkillSet : MonoBehaviour
{
    [SerializeField]
    int _light_attack_id = -1;
    [SerializeField]
    int _heavy_attack_id = -1;
    [SerializeField]
    int _skill_1_id = -1;
    [SerializeField]
    int _skill_2_id = -1;
    [SerializeField]
    int _skill_3_id = -1;
    [SerializeField]
    int _skill_4_id = -1;
    [SerializeField]
    int _skill_5_id = -1;
    [SerializeField]
    int _skill_ult_id = -1;

    [Header("Auto Init")]
    public SkillData light_attack;
    public SkillData heavy_attack;
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
            light_attack = AssetDataManager.Instance.GetSkillById(value);
            _light_attack_id = value;
        }
    }
    public int heavy_attack_id
    {
        get => _heavy_attack_id; set
        {
            heavy_attack = AssetDataManager.Instance.GetSkillById(value);
            _heavy_attack_id = value;
        }
    }
    public int skill_1_id
    {
        get => _skill_1_id; set
        {
            skill_1 = AssetDataManager.Instance.GetSkillById(value);
            _skill_1_id = value;
        }
    }
    public int skill_2_id
    {
        get => _skill_2_id; set
        {
            skill_2 = AssetDataManager.Instance.GetSkillById(value);
            _skill_2_id = value;
        }
    }
    public int skill_3_id
    {
        get => _skill_3_id; set
        {
            skill_3 = AssetDataManager.Instance.GetSkillById(value);
            _skill_3_id = value;
        }
    }
    public int skill_4_id
    {
        get => _skill_4_id; set
        {
            skill_4 = AssetDataManager.Instance.GetSkillById(value);
            _skill_4_id = value;
        }
    }
    public int skill_5_id
    {
        get => _skill_5_id; set
        {
            skill_5 = AssetDataManager.Instance.GetSkillById(value);
            _skill_5_id = value;
        }
    }
    public int skill_ult_id
    {
        get => _skill_ult_id; set
        {
            skill_ult = AssetDataManager.Instance.GetSkillById(value);
            _skill_ult_id = value;
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
        setAllSkils();
    }
    void setAllSkils()
    {
        light_attack_id = _light_attack_id;
        heavy_attack_id = _heavy_attack_id;
        skill_1_id = _skill_1_id;
        skill_2_id = _skill_2_id;
        skill_3_id = _skill_3_id;
        skill_4_id = _skill_4_id;
        skill_5_id = _skill_5_id;
        skill_ult_id = _skill_ult_id;
    }
}
