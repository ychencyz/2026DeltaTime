using UnityEngine;

public class Enemy : MonoBehaviour, ICombatTarget
{
    private CharacterIdentifier characterIdentifier;
    private UIFloatingBarElement barElement;
    [Header("Auto Init")]
    [SerializeField]
    private int _hp = 0;
    [SerializeField]
    private int _maxHp = 0;
    private bool _isDead = false;
    public int hp
    {
        get => _hp;
        private set
        {
            _hp = value;
            barElement.UpdateHealthBar(_hp, _maxHp);
        }
    }
    public int maxHp
    {
        get => _maxHp;
        private set
        {
            _maxHp = value;
            barElement.UpdateHealthBar(_hp, _maxHp);
        }
    }
    private void OnValidate()
    {
        if (barElement != null && barElement.initialized)
        {
            hp = _hp;
            maxHp = _maxHp;
        }
    }
    private void Awake()
    {
        barElement = GetComponent<UIFloatingBarElement>();
        characterIdentifier = GetComponent<CharacterIdentifier>();
        _hp = characterIdentifier.data.initialHp;
        _maxHp = characterIdentifier.data.initialMaxHp;
        barElement.SetInitialHp(_hp, _maxHp);
        barElement.SetInitialName(characterIdentifier.data.displayName);
    }

    // ===================== ICombatTarget =====================

    public bool IsDead() => _isDead;
    public Vector3 GetPosition() => transform.position;
    public Transform GetTransform() => transform;

    public void TakeDamage(float damage, Transform attacker = null)
    {
        //if (_isDead) return;

        //// 只接受玩家來源傷害
        if (attacker != null)
        {
            if (!attacker.root.CompareTag("Player"))
            {
                return;
            }
        }

        //float actualDamage = DamageSystem.Instance != null
        //    ? DamageSystem.Instance.CalculateDamage(damage, defense)
        //    : damage;
        float actualDamage = damage;

        //_hp -= (int)actualDamage;
        Enemy test = GetComponent<Enemy>();
        test.hp = test.hp - (int)actualDamage;
        //Debug.Log($"{characterIdentifier?.data?.displayName} 受到傷害: {actualDamage}hp，剩餘血量: {_hp}/{_maxHp}", gameObject);
        Debug.Log($"{characterIdentifier?.data?.displayName} 受到傷害: {actualDamage}hp，剩餘血量: {hp}/{maxHp}", gameObject);

        //if (floatingBar != null && floatingBar.initialized)
        //    floatingBar.UpdateHealthBar(_hp, _maxHp);

        //// 被打到時立即偵測到玩家（仇恨）
        //if (!playerDetected && attacker != null)
        //{
        //    playerDetected = true;
        //}

        //if (enemyComponent.hp <= 0)
        //    Die();
    }

}
