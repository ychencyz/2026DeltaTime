using UnityEngine;
using UnityEngine.Rendering.Universal; // 必須引用 URP 命名空間

public class TelegraphTrigger : MonoBehaviour
{
    public float fillSpeed = 0.5f;
    private DecalProjector decal; // 改用 DecalProjector
    private Material targetMaterial;
    private float currentProgress = 0f;
    private bool isPlayerInside = false;

    void Start()
    {
        // 1. 抓取 Decal Projector 組件
        decal = GetComponent<DecalProjector>();

        if (decal != null && decal.material != null)
        {
            // 2. 實例化材質，避免修改到專案原始檔案
            targetMaterial = new Material(decal.material);
            decal.material = targetMaterial;

            // 初始進度歸零
            targetMaterial.SetFloat("_Progress", 0f);
        }
        else
        {
            Debug.LogError("找不到 Decal Projector 或材質！");
        }
    }

    void Update()
    {
        if (targetMaterial == null) return;

        // 3. 根據玩家是否在內控制進度
        if (isPlayerInside)
        {
            currentProgress += Time.deltaTime * fillSpeed;
        }
        else
        {
            currentProgress -= Time.deltaTime * fillSpeed;
        }

        currentProgress = Mathf.Clamp(currentProgress, 0f, 0.5f);
        targetMaterial.SetFloat("_Progress", currentProgress);
    }

    // 4. 觸發偵測（確保 Decal 物件下有加 Collider 並勾選 Is Trigger）
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}