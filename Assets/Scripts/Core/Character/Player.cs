using UnityEngine;

public class Player : MonoBehaviour
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
    public int hp
    {
        get => _hp;
        private set
        {
            PlayerDelegates.Instance.OnPlayerResourceChange?.Invoke(this);
        }
    }
    public int maxHp
    {
        get => _maxHp;
        private set
        {
            PlayerDelegates.Instance.OnPlayerResourceChange?.Invoke(this);
        }
    }
    public int mp
    {
        get => _mp;
        private set
        {
            PlayerDelegates.Instance.OnPlayerResourceChange?.Invoke(this);
        }
    }
    public int maxMp
    {
        get => _maxMp;
        private set
        {
            PlayerDelegates.Instance.OnPlayerResourceChange?.Invoke(this);
        }
    }
    public int st
    {
        get => _st;
        private set
        {
            PlayerDelegates.Instance.OnPlayerResourceChange?.Invoke(this);
        }
    }
    public int maxSt
    {
        get => _maxSt;
        private set
        {
            PlayerDelegates.Instance.OnPlayerResourceChange?.Invoke(this);
        }
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
