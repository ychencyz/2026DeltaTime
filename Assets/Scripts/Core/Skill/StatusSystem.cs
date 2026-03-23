using UnityEngine;

public class StatusSystem: MonoBehaviour
{
    public bool isStunned = false;
    public bool canMove = true;
    public bool canCast = true;
    public bool isBlocking = false;
    public bool isRolling = false;

    [Header("格擋設定")]
    [SerializeField] private float blockDamageReduction = 0.7f; // 格擋減傷比例 (70%)

    /// 格擋時的實際傷害 = 原傷害 * (1 - blockDamageReduction)
    public float GetBlockedDamage(float incomingDamage)
    {
        if (!isBlocking) return incomingDamage;
        return incomingDamage * (1f - blockDamageReduction);
    }

    /// 解除暈眩
    public void BreakFree()
    {
        if (!isStunned) return;
        isStunned = false;
        canMove = true;
        canCast = true;
        Debug.Log("BreakFree! Stun removed.");
    }
}