using UnityEngine;

/// 戰鬥目標接口 - 任何可以被攻擊的物體都應該實現這個接口
public interface ICombatTarget
{
 
    void TakeDamage(float damage, Transform attacker = null);  // 接受傷害(傷害值/攻擊者)

    
    //int GetCurrentHP();//獲取目前血量
   
    //int GetMaxHP(); // 獲取最大血量
    
    bool IsDead();//檢查是否死亡

    Vector3 GetPosition();//獲取位置

    Transform GetTransform();  //獲取Transform
}