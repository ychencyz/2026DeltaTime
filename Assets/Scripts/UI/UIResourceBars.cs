using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class UIResourceBars : MonoBehaviour
{
    [SerializeField]
    private GameObject go_healthBar;
    [SerializeField]
    UIBarComponent healthUIBar;
    [SerializeField]
    private GameObject go_magickaBar;
    [SerializeField]
    UIBarComponent magickaUIBar;
    [SerializeField]
    private GameObject go_staminaBar;
    [SerializeField]
    UIBarComponent staminaUIBar;
    private void OnEnable()
    {
        PlayerDelegates.Instance.OnPlayerResourceChange += UpdateHealthBar;
        PlayerDelegates.Instance.OnPlayerResourceChange += UpdateMagickaBar;
        PlayerDelegates.Instance.OnPlayerResourceChange += UpdateStaminaBar;
    }
    private void OnDisable()
    {
        PlayerDelegates.Instance.OnPlayerResourceChange -= UpdateHealthBar;
        PlayerDelegates.Instance.OnPlayerResourceChange -= UpdateMagickaBar;
        PlayerDelegates.Instance.OnPlayerResourceChange -= UpdateStaminaBar;
    }
    private void Awake()
    {
        healthUIBar = go_healthBar.GetComponent<UIBarComponent>();
        magickaUIBar = go_magickaBar.GetComponent<UIBarComponent>();
        staminaUIBar = go_staminaBar.GetComponent<UIBarComponent>();
    }

    public void UpdateHealthBar(Player player)
    {
        SetImages(player.hp, player.maxHp, healthUIBar.images);
        SetTexts(player.hp, player.maxHp, healthUIBar);
    }
    public void UpdateMagickaBar(Player player)
    {
        SetImages(player.mp, player.maxMp, magickaUIBar.images);
        SetTexts(player.mp, player.maxMp, magickaUIBar);
    }
    public void UpdateStaminaBar(Player player)
    {
        SetImages(player.st, player.maxSt, staminaUIBar.images);
        SetTexts(player.st, player.maxSt, staminaUIBar);
    }
    void SetImages(int min, int max, List<Image> imgs)
    {
        foreach (Image img in imgs)
        {
            float result = (float)min / (float)max;
            result = Mathf.Clamp(result, 0, 1);
            img.fillAmount = result;
        }
    }
    void SetTexts(int min, int max, UIBarComponent UIBar)
    {
        string numberString;
        if (min < 10000)
        {
            numberString = min.ToString("N0");
        }
        else
        {
            numberString = (min / 1000.0).ToString("0.00") + "k";
        }
        string percentString;
        float percent = (float)min / (float)max * 100;
        if (percent > 9)
        {
            percentString = ((float)Math.Round(percent)).ToString();
        }
        else
        {
            percentString = percent.ToString("F");
        }
        UIBar.displayText.text = numberString;
        UIBar.displayText2.text = percentString + "%";
    }
}
