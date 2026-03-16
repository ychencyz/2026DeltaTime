using UnityEngine;

public class UIFloatingBarManager : MonoBehaviour
{
    [SerializeField]
    private UIFloatingBarElement[] floatingBarList;
    [SerializeField]
    private GameObject barPrefab;
    [SerializeField]
    private Camera mainCamera;
    private void Awake()
    {
        floatingBarList = FindObjectsByType<UIFloatingBarElement>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (mainCamera == null)
            mainCamera = Camera.main;
    }
    void Start()
    {
        for (int i = 0; i < floatingBarList.Length; i++)
        {
            UIFloatingBarElement barElement = floatingBarList[i];
            barElement.Init(barPrefab);
        }
    }
    private void LateUpdate()
    {
        if (floatingBarList != null)
        {
            foreach (UIFloatingBarElement barElement in floatingBarList)
            {
                if (barElement == null) continue;
                GameObject go_Npc = barElement.gameObject;
                if (go_Npc == null || !go_Npc.activeSelf) continue;
                UpdateTransformWorld(barElement, go_Npc);
            }
        }
    }
    //public void UpdateTransformOverlay(UIFloatingBarElement barElement, GameObject trackTarget)
    //{
    //    Vector3 vOffset = barElement.vOffset;
    //    Vector3 wPos = trackTarget.transform.position + vOffset;
    //    Vector3 spos = mainCamera.WorldToScreenPoint(wPos);
    //    //if (spos.z < 0.1f)
    //    //{
    //    //    if (trackTarget.activeSelf != false)
    //    //        barElement.Disable();
    //    //}
    //    //else
    //    //{
    //    //    if (trackTarget.activeSelf != true)
    //    //        barElement.Enable();
    //    //}
    //    barElement.go_bar.transform.position = spos;
    //}
    public void UpdateTransformWorld(UIFloatingBarElement barElement, GameObject trackTarget)
    {
        Vector3 vOffset = barElement.vOffset;
        Vector3 wPos = trackTarget.transform.position + vOffset;
        barElement.go_bar.transform.position = wPos;
        barElement.go_bar.transform.forward = mainCamera.transform.forward;
    }
}
