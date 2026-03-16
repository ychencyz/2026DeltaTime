//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

///// <summary>
///// UIResourceBars 修復版本 - 簡化版
///// 直接使用 UIBarComponent 引用，不需要 GameObject
///// 在 Inspector 中只需要拖入三個 UIBarComponent 即可
///// </summary>
//public class UIBar : MonoBehaviour
//{
//    [Header("直接拖入這三個 UIBarComponent")]
//    [SerializeField] private UIBarComponent healthUIBar;
//    [SerializeField] private UIBarComponent magickaUIBar;
//    [SerializeField] private UIBarComponent staminaUIBar;

//    private void OnEnable()
//    {
//        // 註冊事件監聽
//        if (PlayerDelegates.Instance != null)
//        {
//            PlayerDelegates.Instance.OnPlayerResourceChange += UpdateHealthBar;
//            PlayerDelegates.Instance.OnPlayerResourceChange += UpdateMagickaBar;
//            PlayerDelegates.Instance.OnPlayerResourceChange += UpdateStaminaBar;
//        }
//    }

//    private void OnDisable()
//    {
//        // 取消事件監聽
//        if (PlayerDelegates.Instance != null)
//        {
//            PlayerDelegates.Instance.OnPlayerResourceChange -= UpdateHealthBar;
//            PlayerDelegates.Instance.OnPlayerResourceChange -= UpdateMagickaBar;
//            PlayerDelegates.Instance.OnPlayerResourceChange -= UpdateStaminaBar;
//        }
//    }


//    /// <summary>
//    /// 更新玩家血條
//    /// </summary>
//    public void UpdateHealthBar(Player player)
//    {
//        if (player == null)
//        {
//            Debug.LogWarning("玩家物件為 null，無法更新血條", gameObject);
//            return;
//        }

//        if (healthUIBar == null)
//        {
//            Debug.LogWarning("血條 UI 未初始化，請檢查 Inspector 設置", gameObject);
//            return;
//        }

//        if (healthUIBar.images == null || healthUIBar.images.Count == 0)
//        {
//            Debug.LogWarning("血條 images 列表為空", gameObject);
//            return;
//        }

//        SetImages(player.hp, player.maxHp, healthUIBar.images);
//        SetTexts(player.hp, player.maxHp, healthUIBar);
//    }

//    /// <summary>
//    /// 更新玩家魔法值條
//    /// </summary>
//    public void UpdateMagickaBar(Player player)
//    {
//        if (player == null)
//            return;

//        if (magickaUIBar == null)
//            return;

//        if (magickaUIBar.images == null || magickaUIBar.images.Count == 0)
//            return;

//        SetImages(player.mp, player.maxMp, magickaUIBar.images);
//        SetTexts(player.mp, player.maxMp, magickaUIBar);
//    }

//    /// <summary>
//    /// 更新玩家耐力值條
//    /// </summary>
//    public void UpdateStaminaBar(Player player)
//    {
//        if (player == null)
//            return;

//        if (staminaUIBar == null)
//            return;

//        if (staminaUIBar.images == null || staminaUIBar.images.Count == 0)
//            return;

//        SetImages(player.st, player.maxSt, staminaUIBar.images);
//        SetTexts(player.st, player.maxSt, staminaUIBar);
//    }

//    /// <summary>
//    /// 設置血條圖片填滿量
//    /// </summary>
//    private void SetImages(int current, int max, List<Image> images)
//    {
//        if (images == null || max <= 0)
//            return;

//        foreach (Image img in images)
//        {
//            if (img == null)
//                continue;

//            float fillAmount = (float)current / (float)max;
//            fillAmount = Mathf.Clamp01(fillAmount);
//            img.fillAmount = fillAmount;
//        }
//    }

//    /// <summary>
//    /// 設置血條文字
//    /// </summary>
//    private void SetTexts(int current, int max, UIBarComponent uiBar)
//    {
//        if (uiBar == null || uiBar.displayText == null || uiBar.displayText2 == null)
//            return;

//        if (max <= 0)
//            return;

//        // 格式化數字文字
//        string numberString;
//        if (current < 10000)
//        {
//            numberString = current.ToString("N0");
//        }
//        else
//        {
//            numberString = (current / 1000.0).ToString("0.00") + "k";
//        }

//        // 計算百分比
//        float percent = (float)current / (float)max * 100f;
//        string percentString = percent > 9
//            ? ((int)percent).ToString()
//            : percent.ToString("F1");

//        // 更新文字
//        uiBar.displayText.text = numberString;
//        uiBar.displayText2.text = percentString + "%";
//    }
//}