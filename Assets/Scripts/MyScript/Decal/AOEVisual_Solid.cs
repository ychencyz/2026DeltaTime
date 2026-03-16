using UnityEngine;

public class AOEVisual_Solid : MonoBehaviour
{
    [Header("扇形 AOE")]
    public GameObject sectorDecalPrefab; // 左鍵時生成的扇形 Prefab
    public Transform spawnPoint; // 生成位置（可不填，預設使用自己）
    public Vector3 spawnOffset = new Vector3(0f, 0.05f, 0f);
    public float decalDuration = 2f;

    [Header("相容舊用法（可選）")]
    public GameObject sectorDecal; // 若未設定 Prefab，退回顯示場景既有物件

    private bool isDecalActive = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TriggerSectorAoE();
        }
    }

    private void TriggerSectorAoE()
    {
        if (sectorDecalPrefab != null)
        {
            Vector3 basePosition = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion baseRotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;
            GameObject decalInstance = Instantiate(sectorDecalPrefab, basePosition + spawnOffset, baseRotation);

            if (decalDuration > 0f)
            {
                Destroy(decalInstance, decalDuration);
            }

            return;
        }

        // 相容舊邏輯：直接顯示/隱藏場景中的既有 Decal 物件
        if (sectorDecal == null)
        {
            Debug.LogWarning("AOEVisual_Solid: 請設定 sectorDecalPrefab 或 sectorDecal。", gameObject);
            return;
        }

        if (!isDecalActive)
        {
            sectorDecal.SetActive(true);
            isDecalActive = true;
            CancelInvoke(nameof(HideDecal));
            Invoke(nameof(HideDecal), decalDuration);
        }
    }

    private void HideDecal()
    {
        if (sectorDecal != null)
        {
            sectorDecal.SetActive(false);
        }
        isDecalActive = false;
    }
}