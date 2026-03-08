using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum SkillSlotState
{
    Empty,
    Slotted
}
public class SkillSlot : MonoBehaviour
{
    [SerializeField]
    private GameObject go_Empty;
    [SerializeField]
    private GameObject go_Image;
    [SerializeField]
    private Image cooldownMask;
    [SerializeField]
    private TMP_Text countDown;
    [SerializeField]
    private TMP_Text controlKey;
    private Image Image;
    public Color imageDiabledColor = new Color(0.76f, 0.76f, 0.76f, 1f);
    private float _countDownFrom;
    private bool isTimerRunning = false;
    SkillSlotState _State = SkillSlotState.Empty;
    SkillSlotState State
    {
        get => _State;
        set
        {
            if (_State == value) return;
            if (value == SkillSlotState.Empty)
            {
                ResetSkillSlot();
                _State = SkillSlotState.Empty;
            }
            if (value == SkillSlotState.Slotted)
            {
                EnableSkillSlot();
                _State = SkillSlotState.Slotted;
            }
        }
    }
    private void Awake()
    {
        ResetSkillSlot();
    }
    private void Start()
    {
        Image = go_Image.GetComponent<Image>();
    }
    void ResetSkillSlot()
    {
        go_Empty.SetActive(true);
        go_Image.SetActive(false);
        cooldownMask.fillAmount = 0f;
        countDown.text = "";
    }
    void EnableSkillSlot()
    {
        go_Empty.SetActive(false);
        go_Image.SetActive(true);
        cooldownMask.fillAmount = 0f;
        countDown.text = "";
    }
    public void Enable()
    {
        State = SkillSlotState.Slotted;
    }
    public void Disable()
    {
        State = SkillSlotState.Empty;
    }
    public void SetControlKey(string text)
    {
        controlKey.text = text;
    }
    public void SetSkill(SkillData data)
    {
        Image.sprite = data.icon;
    }
    public void StartCountDown(float seconds)
    {
        _countDownFrom = seconds;
        StartCoroutine(CountdownRoutine());
    }
    private IEnumerator CountdownRoutine()
    {
        StartCountDown();
        float currentTime = _countDownFrom;
        float interval = 0.05f;
        while (currentTime > 0)
        {
            countDown.text = currentTime.ToString("0.0"); // Formats to a whole number
            float newFillAmount = Mathf.InverseLerp(0, _countDownFrom, currentTime);
            cooldownMask.fillAmount = newFillAmount;
            yield return new WaitForSeconds(interval);
            currentTime -= interval;
        }

        EndCountdown();
    }

    // Example of how to stop the coroutine from another function
    public void StopCountdown()
    {
        if (isTimerRunning)
        {
            StopCoroutine(CountdownRoutine());
            EndCountdown();

            Debug.Log("Countdown stopped prematurely.");
        }
    }

    void StartCountDown()
    {
        isTimerRunning = true;

        cooldownMask.fillAmount = 1f;
        Image.color = imageDiabledColor;
    }
    void EndCountdown()
    {
        countDown.text = "";
        cooldownMask.fillAmount = 0f;
        Image.color = Color.white;

        isTimerRunning = false;
    }
}
