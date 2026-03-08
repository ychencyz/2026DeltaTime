using UnityEngine;

public class UIFloatingBarElement : MonoBehaviour
{
    public GameObject go_bar;
    public Vector3 vOffset = Vector3.zero;

    public void Enable()
    {
        go_bar.SetActive(true);
    }
    public void Disable()
    {
        go_bar.SetActive(false);
    }
}
