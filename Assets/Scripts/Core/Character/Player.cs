using UnityEngine;

public class Player : MonoBehaviour, ICombatTarget
{
    [SerializeField]
    private int _hp = 50;
    [SerializeField]
    private int _maxHp = 100;
    [SerializeField]
    private int _mp = 50;
    [SerializeField]
    private int _maxMp = 100;
    [SerializeField]
    private int _st = 50;
    [SerializeField]
    private int _maxSt = 100;
    [SerializeField]
    private float _defense = 10f;

    private bool _isDead = false;

    public int hp
    {
        get => _hp;
        private set
        {
            _hp = Mathf.Clamp(value, 0, _maxHp);
            PlayerDelegates.Instance.OnPlayerResourceChange?.Invoke(this);
        }
    }
    public int maxHp
    {
        get => _maxHp;
        private set
        {
            _maxHp = value;
            PlayerDelegates.Instance.OnPlayerResourceChange?.Invoke(this);
        }
    }
    public int mp
    {
        get => _mp;
        private set
        {
            _mp = value;
            PlayerDelegates.Instance.OnPlayerResourceChange?.Invoke(this);
        }
    }
    public int maxMp
    {
        get => _maxMp;
        private set
        {
            _maxMp = value;
            PlayerDelegates.Instance.OnPlayerResourceChange?.Invoke(this);
        }
    }
    public int st
    {
        get => _st;
        private set
        {
            _st = value;
            PlayerDelegates.Instance.OnPlayerResourceChange?.Invoke(this);
        }
    }
    public int maxSt
    {
        get => _maxSt;
        private set
        {
            _maxSt = value;
            PlayerDelegates.Instance.OnPlayerResourceChange?.Invoke(this);
        }
    }

    // ICombatTarget 實作
    public void TakeDamage(float damage, Transform attacker = null)
    {
        if (_isDead) return;

        float actualDamage = DamageSystem.Instance != null
            ? DamageSystem.Instance.CalculateDamage(damage, _defense)
            : damage;

        // 格擋減傷
        StatusSystem statusSystem = GetComponent<StatusSystem>();
        if (statusSystem != null)
            actualDamage = statusSystem.GetBlockedDamage(actualDamage);

        hp -= (int)actualDamage;
        Debug.Log($"玩家受到傷害: {actualDamage}hp，剩餘血量: {_hp}/{_maxHp}");

        if (_hp <= 0)
            Die();
    }

    public int GetCurrentHP() => _hp;
    public int GetMaxHP() => _maxHp;
    public bool IsDead() => _isDead;
    public Vector3 GetPosition() => transform.position;
    public Transform GetTransform() => transform;

    private void Die()
    {
        _isDead = true;
        Debug.Log("玩家死亡!");

        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.SetTrigger("Die");

        SkillController sc = GetComponent<SkillController>();
        if (sc != null) sc.enabled = false;
        PlayerInputs pi = GetComponent<PlayerInputs>();
        if (pi != null) pi.enabled = false;
        SkillSet ss = GetComponent<SkillSet>();
        if (ss != null) ss.enabled = false;
    }

    public void Revive(int hpAmount = -1)
    {
        _isDead = false;
        hp = hpAmount > 0 ? hpAmount : _maxHp;

        SkillController sc = GetComponent<SkillController>();
        if (sc != null) sc.enabled = true;
        PlayerInputs pi = GetComponent<PlayerInputs>();
        if (pi != null) pi.enabled = true;
        SkillSet ss = GetComponent<SkillSet>();
        if (ss != null) ss.enabled = true;

        Debug.Log("玩家復活!");
    }

    private void OnValidate()
    {
        if (PlayerDelegates.Instance != null)
        {
            hp = _hp;
            mp = _mp;
            st = _st;
            maxHp = _maxHp;
            maxMp = _maxMp;
            maxSt = _maxSt;
        }
    }
    void Start()
    {
        PlayerDelegates.Instance.OnPlayerResourceChange?.Invoke(this);
    }
}
