using UnityEngine;
using UnityEngine.UI;

public class UIFloatingBarElement : MonoBehaviour
{
    [Header("Auto Init")]
    public GameObject go_bar;
    public UIBarComponent UIBarImage;
    [Header("Custom")]
    public Vector3 vOffset = Vector3.zero;
    [HideInInspector]
    public bool initialized = false;
    //[SerializeField]
    private int _hp;
    //[SerializeField]
    private int _maxHp;
    //[SerializeField]
    private string _displayNname;
    public void EnableBar()
    {
        go_bar.SetActive(true);
    }
    public void DisableBar()
    {
        go_bar.SetActive(false);
    }
    public void Init(GameObject barPrefab)
    {
        go_bar = Instantiate(barPrefab);
        go_bar.transform.SetParent(UIData.Instance.worldCanvas.transform);
        UIBarImage = go_bar.GetComponent<UIBarComponent>();
        if (gameObject.activeSelf)
        {
            EnableBar();
        }
        else
        {
            DisableBar();
        }
        UpdateHealthBar(_hp, _maxHp);
        InitBarName();
        initialized = true;
    }
    public void UpdateHealthBar(int newHP, int newMaxHP)
    {
        _hp = newHP;
        _maxHp = newMaxHP;
        foreach (Image img in UIBarImage.images)
        {
            float result = (float)newHP / (float)newMaxHP;
            result = Mathf.Clamp(result, 0, 1);
            img.fillAmount = result;
        }
    }
    public void InitBarName()
    {
        UIBarImage.displayText.text = _displayNname;
    }
    public void SetInitialHp(int min, int max)
    {
        _hp = min;
        _maxHp = max;
    }
    public void SetInitialName(string _name)
    {
        _displayNname = _name;
    }
}
