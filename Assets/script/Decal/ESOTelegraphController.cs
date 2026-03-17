using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(DecalProjector))]
public class ESOTelegraphController : MonoBehaviour
{
    [Header("時間設定")]
    [Tooltip("技能從出現到填滿的總時間（秒）")]
    public float duration = 3.0f;

    [Header("閃爍效果設定")]
    [Tooltip("進度達到多少百分比時開始閃爍 (0.0 ~ 1.0)")]
    [Range(0f, 1f)]
    public float blinkThreshold = 0.8f;

    [Tooltip("閃爍的速度")]
    public float blinkSpeed = 20f;

    [Header("完成時行為")]
    [Tooltip("勾選後只顯示預警效果，時間到直接消失，不在此腳本內扣血")]
    public bool visualOnly = true;

    [Tooltip("visualOnly 關閉時，觸發範圍傷害的半徑")]
    public float damageRadius = 5f;

    [Tooltip("visualOnly 關閉時，造成的傷害")]
    public float damageAmount = 20f;

    //public GameObject rockPrefab;

    private float currentTime = 0f;
    private DecalProjector projector;
    private Material instanceMaterial; // 用於存放每個實例私有的材質

    void Start()
    {
        // 獲取 Decal Projector 組件
        projector = GetComponent<DecalProjector>();

        if (projector.material != null)
        {
            // 重要：創建材質副本 (Instance)，確保多個預警圈可以有不同的進度
            instanceMaterial = new Material(projector.material);
            projector.material = instanceMaterial;
        }
        else
        {
            Debug.LogError("未在 DecalProjector 中找到材質！請指派一個 Shader Graph 材質。");
        }

        // 初始化狀態
        UpdateTelegraph(0f, 1f);
    }

    void Update()
    {
        if (currentTime < duration)
        {
            currentTime += Time.deltaTime;

            // 計算目前進度比例 (0 到 1)
            float ratio = Mathf.Clamp01(currentTime / duration);

            // 將進度映射到 Shader 使用的 0 到 0.5 (假設你的 Shader 邏輯如此)
            float progress = ratio * 0.5f;

            // 處理最後階段的閃爍效果
            float emission = 1.0f;
            if (ratio >= blinkThreshold)
            {
                // 使用 Sin 波在 0.5 到 1.5 之間震盪
                emission = 1.0f + Mathf.Sin(Time.time * blinkSpeed) * 0.5f;
            }

            // 更新材質參數
            UpdateTelegraph(progress, emission);
        }
        else
        {
            // 時間截止，執行完成邏輯
            OnTelegraphComplete();
        }
    }
    private void OnDisable()
    {
        currentTime = 0.0f;
    }

    /// 更新 Shader 中的屬性
    private void UpdateTelegraph(float progress, float emission)
    {
        if (instanceMaterial != null)
        {
            // 這些字串必須與你的 Shader Graph 變數的 Reference Name 完全一致
            instanceMaterial.SetFloat("_Progress", progress);
            instanceMaterial.SetFloat("_EmissionMult", emission);
        }
    }

    /// 當預警圈填滿時觸發
    void OnTelegraphComplete()
    {
        if (visualOnly)
        {
            gameObject.SetActive(false);
            return;
        }

        // 這裡可以播放音效或生成爆炸特效粒子
        // Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Debug.Log("技能預警結束，觸發傷害！");
        // 1. 在紅圈的位置生成石頭
        //if (rockPrefab != null)
        //{
        //    Instantiate(rockPrefab, transform.position + Vector3.up * 10f, Quaternion.identity);
        //}

        // 2. 可以在這裡加上「範圍傷害判定」
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (var hit in hitColliders)
        {
            ICombatTarget target = hit.GetComponentInParent<ICombatTarget>();
            if (target != null)
            {
                target.TakeDamage(damageAmount, transform);
            }
        }

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // 當物件銷毀時，釋放動態創建的材質記憶體
        if (instanceMaterial != null)
        {
            Destroy(instanceMaterial);
        }
    }
}