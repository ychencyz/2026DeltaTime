using UnityEngine;

/// ���a�԰��t�� - �B�z���a�ˮ`�M��q�޲z
public class PlayerCombat : MonoBehaviour, ICombatTarget
{
    [SerializeField]
    private int _hp = 100;
    [SerializeField]
    private int _maxHp = 100;
    [SerializeField]
    private float _defense = 10f;  // ���m��

    private bool _isDead = false;
    private CharacterIdentifier characterIdentifier;

    public int HP
    {
        get => _hp;
        private set
        {
            _hp = Mathf.Clamp(value, 0, _maxHp);
            // Ĳ�o�ƥ�q��UI��s
            PlayerDelegates.Instance?.OnPlayerResourceChange?.Invoke(null);
        }
    }

    public int MaxHP
    {
        get => _maxHp;
        private set => _maxHp = value;
    }

    public float Defense
    {
        get => _defense;
        set => _defense = Mathf.Max(0, value);
    }

    private void Awake()
    {
        characterIdentifier = GetComponent<CharacterIdentifier>();

        // �p�G��CharacterIdentifier�A�q�������lHP
        if (characterIdentifier != null && characterIdentifier.data != null)
        {
            _hp = characterIdentifier.data.initialHp;
            _maxHp = characterIdentifier.data.initialMaxHp;
        }

        _isDead = false;
    }


    public void TakeDamage(float damage, Transform attacker = null)
    {
        if (_isDead)
            return;

        // 使用DamageSystem計算實際傷害
        float actualDamage = DamageSystem.Instance != null
            ? DamageSystem.Instance.CalculateDamage(damage, _defense)
            : damage;

        // 格擋減傷
        StatusSystem statusSystem = GetComponent<StatusSystem>();
        if (statusSystem != null)
        {
            actualDamage = statusSystem.GetBlockedDamage(actualDamage);
        }

        HP -= (int)actualDamage;

        Debug.Log($"玩家受到傷害: {actualDamage}hp，剩餘血量: {_hp}/{_maxHp}");

       

        // �ˬd���`
        if (_hp <= 0)
        {
            Die(attacker);
        }
    }

    /// �v��
    public void Heal(float healAmount)
    {
        if (_isDead)
            return;

        HP += (int)healAmount;
        Debug.Log($"���a��_: {healAmount}hp�A�ثe��q: {_hp}/{_maxHp}");
    }

    /// 玩家死亡處理
    private void Die(Transform killer)
    {
        _isDead = true;
        Debug.Log("玩家死亡!");

        // 停用所有操作
        GetComponent<SkillController>().enabled = false;
        GetComponent<PlayerInputs>().enabled = false;
        GetComponent<SkillSet>().enabled = false;

        // 播放死亡動畫
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }

    /// �_�����a
    public void Revive(int hpAmount = -1)
    {
        _isDead = false;
        HP = hpAmount > 0 ? hpAmount : _maxHp;

        GetComponent<SkillController>().enabled = true;
        GetComponent<PlayerInputs>().enabled = true;
        GetComponent<SkillSet>().enabled = true;

        Debug.Log("���a�_��!");
    }

    // ICombatTarget���f��{
    public int GetCurrentHP() => _hp;
    public int GetMaxHP() => _maxHp;
    public bool IsDead() => _isDead;
    public Vector3 GetPosition() => transform.position;
    public Transform GetTransform() => transform;

    /// OnValidate - ��K�b�s�边���ո�
    private void OnValidate()
    {
        if (_maxHp < 1)
            _maxHp = 1;
        if (_hp > _maxHp)
            _hp = _maxHp;
    }
}
