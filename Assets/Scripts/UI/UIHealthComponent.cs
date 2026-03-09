using UnityEngine;

[RequireComponent(typeof(UIFloatingBarElement))]
public class UIHealthComponent : MonoBehaviour
{
    GameObject go_bar;
    private int _hp;
    private int _maxHp;
    public int hp { get => _hp; set
        {

        }
    }
    public int maxHp;
}
