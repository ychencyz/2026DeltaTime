using UnityEngine;

public class Enemy : MonoBehaviour
{
    private CharacterIdentifier characterIdentifier;
    private UIFloatingBarElement barElement;
    [Header("Auto Init")]
    [SerializeField]
    private int _hp = 0;
    [SerializeField]
    private int _maxHp = 0;
    public int hp
    {
        get => _hp;
        set
        {
            _hp = value;
            barElement.UpdateHealthBar(_hp, _maxHp);
        }
    }
    public int maxHp
    {
        get => _maxHp;
        set
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

    //private void FixedUpdate()
    //{
    //    hp -=1;
    //    //Debug.Log($"hp: {_hp}");
    //}
}
