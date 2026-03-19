using Unity.VisualScripting;
using UnityEngine;
using static EnemyAI;

static public class EnemyAttacks
{
    public static void EnemyOne(EnemySkillController skillController, EnemySkillSet skillSet, float distance, AIState nowState, float nextFireTime)
    {
        float cooldownTime_lightAttack = 2f;  //輕擊技能冷卻時間
        float cooldownTime_heavyAttack = 2f;  //重擊技能冷卻時間
        skillController.TryCast(skillSet.light_attack);

        if (distance > 5f)
        {
            Debug.Log("輕擊!");
            LightAttack();
        }
        else if (distance >= 3f && distance <= 5f)
        {
            Debug.Log("重擊!");
            HeavyAttack();
        }
        else if (distance < 5f)
        {
            Debug.Log("追擊!!");
            nowState = AIState.Chase; //切換到追逐模式
        }

        void LightAttack()
        {
            Debug.Log("技能:輕擊，傷害-15");
            skillController.TryCast(skillSet.light_attack);
            nextFireTime = Time.time + cooldownTime_lightAttack;
        }

        void HeavyAttack()
        {
            Debug.Log("技能:重擊，傷害-30");
            skillController.TryCast(skillSet.heavy_attack);
            nextFireTime = Time.time + cooldownTime_heavyAttack;
        }

        void Interrupt() //打斷技能
        {
            Debug.Log("技能:打斷");
        }
    }
    public static void EnemyTwo(EnemySkillController skillController, EnemySkillSet skillSet, float distance, AIState nowState, float nextFireTime)
    {
        float cooldownTime_lightAttack = 2f;  //輕擊技能冷卻時間
        float cooldownTime_heavyAttack = 2f;  //重擊技能冷卻時間
        skillController.TryCast(skillSet.light_attack);

        if (distance > 5f)
        {
            Debug.Log("輕擊!");
            LightAttack();
        }
        else if (distance >= 3f && distance <= 5f)
        {
            Debug.Log("重擊!");
            HeavyAttack();
        }
        else if (distance < 5f)
        {
            Debug.Log("追擊!!");
            nowState = AIState.Chase; //切換到追逐模式
        }

        void LightAttack()
        {
            Debug.Log("技能:輕擊，傷害-15");
            skillController.TryCast(skillSet.light_attack);
            nextFireTime = Time.time + cooldownTime_lightAttack;
        }

        void HeavyAttack()
        {
            Debug.Log("技能:重擊，傷害-30");
            skillController.TryCast(skillSet.heavy_attack);
            nextFireTime = Time.time + cooldownTime_heavyAttack;
        }

        void Interrupt() //打斷技能
        {
            Debug.Log("技能:打斷");
        }
    }
}
