using System;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [Header("命中偵測")]
    [SerializeField] private float hitRadius = 2f; // 技能命中範圍
    [SerializeField] private LayerMask enemyLayer;  // 敵人 Layer (未設定則搜全部)

    private int HitLayer => enemyLayer.value != 0 ? enemyLayer.value : ~0;

    public void Attack(SkillData data)
    {
        Vector3 skillPosition = GetTargetPosition(data);
        data.position = skillPosition;

        // 生成 VFX
        if (data.VFXPrefab != null)
        {
            GameObject obj = Instantiate(data.VFXPrefab, skillPosition, Quaternion.identity);

            // SelfFollow / TargetFollow: VFX 跟隨施法者或目標
            if (data.targetType == SkillTargetType.SelfFollow)
            {
                obj.transform.SetParent(transform);
            }

            ParticleSystem ps = obj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(obj, ps.main.duration);
            }
            else
            {
                Destroy(obj, 3f);
            }
        }

        // 命中偵測 — OverlapSphere 找範圍內所有 ICombatTarget
        Collider[] hits = Physics.OverlapSphere(skillPosition, hitRadius, HitLayer);
        foreach (Collider hit in hits)
        {
            // 不打自己
            if (hit.transform.root == transform.root)
                continue;

            ICombatTarget target = hit.GetComponent<ICombatTarget>();
            if (target == null)
                target = hit.GetComponentInParent<ICombatTarget>();

            if (target != null && !target.IsDead())
            {
                float finalDamage = DamageSystem.Instance != null
                    ? DamageSystem.Instance.CalculateDamageFromSkill(data)
                    : data.damage;

                target.TakeDamage(finalDamage, transform);
            }
        }

        Debug.Log($"Skill [{data.displayName}] Executed! Hit {hits.Length} colliders.");
    }

    /// 根據 SkillTargetType 決定技能釋放位置
    private Vector3 GetTargetPosition(SkillData data)
    {
        switch (data.targetType)
        {
            case SkillTargetType.Self:
            case SkillTargetType.SelfFollow:
                return transform.position;

            case SkillTargetType.Target:
            case SkillTargetType.TargetFollow:
                Transform nearest = FindNearestEnemy();
                return nearest != null ? nearest.position : transform.position;

            default:
                throw new Exception("targetType not found, skill id:" + data.id + ", SkillTargetType:" + data.targetType);
        }
    }

    /// 找到最近的敵人 (ICombatTarget)
    private Transform FindNearestEnemy()
    {
        float searchRadius = 15f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, HitLayer);

        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (Collider col in colliders)
        {
            if (col.transform.root == transform.root)
                continue;

            ICombatTarget target = col.GetComponent<ICombatTarget>();
            if (target == null)
                target = col.GetComponentInParent<ICombatTarget>();

            if (target == null || target.IsDead())
                continue;

            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = target.GetTransform();
            }
        }

        return nearest;
    }
}
