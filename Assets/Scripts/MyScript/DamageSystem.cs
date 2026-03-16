using UnityEngine;

/// 傷害系統 - 處理所有傷害相關的計算邏輯
public class DamageSystem : MonoBehaviour
{
    public static DamageSystem Instance { get; private set; }

    [Header("傷害計算參數")]
    [SerializeField]
    private float baseDamageMultiplier = 1f; // 基礎傷害倍率
    [SerializeField]
    private float lightAttackMultiplier = 1.2f; // 輕擊傷害加成

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

    //計算最終傷害
    public float CalculateDamage(float baseDamage, float defenseValue = 0f, bool isLightAttack = false)
    {
        // 基礎傷害計算
        float finalDamage = baseDamage * baseDamageMultiplier;

        // 防禦值計算 (減少傷害)
        if (defenseValue > 0)
        {
            float defenseReduction = 1f - (defenseValue / (defenseValue + 100f));
            finalDamage *= defenseReduction;
        }

        // 輕擊加成計算
        if (isLightAttack)
        {
            finalDamage *= lightAttackMultiplier;
            Debug.Log($"輕擊傷害: {finalDamage}");
        }

        // 確保傷害不為負值
        finalDamage = Mathf.Max(1f, finalDamage);

        return finalDamage;
    }

    /// 根據技能數據計算傷害
    public float CalculateDamageFromSkill(SkillData skillData, float defenseValue = 0f, bool isLightAttack = false)
    {
        if (skillData == null)
            return 0f;

        return CalculateDamage(skillData.damage, defenseValue, isLightAttack);
    }

    /// 設置輕擊加成倍率
    public void SetLightAttackMultiplier(float multiplier)
    {
        lightAttackMultiplier = multiplier;
    }

    /// 設置基礎傷害倍率
    public void SetBaseDamageMultiplier(float multiplier)
    {
        baseDamageMultiplier = multiplier;
    }
}