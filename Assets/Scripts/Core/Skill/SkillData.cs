using UnityEngine;

// 1. 數據層：使用 ScriptableObject 達成數據與邏輯解耦
[CreateAssetMenu(fileName = "NewSkillData", menuName = "Skills/SkillData")]
public class SkillData : AssetData
{
    public float anticipationTime = 0.5f; // 前搖
    public float recoveryTime = 0.3f;     // 後搖
    public float cooldownTime = 2.0f;     // 冷卻
    public float damage = 10f;
    public GameObject projectilePrefab;
    public Sprite icon;
}
