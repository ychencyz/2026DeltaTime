using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

enum SkillSlotState
{
    Empty,
    Slotted
}
public enum SkillSlotType
{
    ActionBar,
    SkillBookDisplay,
    SkillBookPreviewBar,
}
public class SkillSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField]
    public SkillSlotType type;
    [Header("Shared Settings")]
    public int refSkillId;
    [SerializeField]
    private GameObject go_Empty;
    [SerializeField]
    private GameObject go_Image;
    [SerializeField]
    private Image Image;
    public int index = -1;
    [SerializeField]
    [Header("Type - ActionBar")]
    private Image cooldownMask;
    [SerializeField]
    private TMP_Text countDown;
    [SerializeField]
    private TMP_Text controlKey;
    [Header("Type - SkillBookDisplay")]
    [SerializeField]
    private TMP_Text displayName;
    [SerializeField]
    private Image ImageForDrag;

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
    private void OnEnable()
    {
        if (isSubscribingToSkillSet())
        {
            PlayerDelegates.Instance.OnPlayerSkillChange += OnSkillChange;
        }
    }
    private void OnDisable()
    {
        if (isSubscribingToSkillSet())
        {
            PlayerDelegates.Instance.OnPlayerSkillChange -= OnSkillChange;
        }
    }
    private void Awake()
    {
        ResetSkillSlot();
    }
    private void Start()
    {
        if (isSkillBookDisplay())
        {
            setSkillSlotById(refSkillId);
        }
    }
    void ResetSkillSlot()
    {
        if (go_Empty)
        {
            go_Empty.SetActive(true);
        }
        go_Image.SetActive(false);
        if (cooldownMask != null)
        {
            cooldownMask.fillAmount = 0f;
        }
        if (countDown != null)
        {
            countDown.text = "";
        }
    }
    void EnableSkillSlot()
    {
        if (go_Empty)
        {
            go_Empty.SetActive(false);
        }
        go_Image.SetActive(true);
        if (cooldownMask != null)
        {
            cooldownMask.fillAmount = 0f;
        }
        if (countDown != null)
        {
            countDown.text = "";
        }
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
    public void SetIcon(Sprite icon)
    {
        Image.sprite = icon;
        if (ImageForDrag != null)
        {
            ImageForDrag.sprite = icon;
        }
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
    public bool isActionBar()
    {
        return type == SkillSlotType.ActionBar;
    }
    public bool isSkillBookDisplay()
    {
        return type == SkillSlotType.SkillBookDisplay;
    }
    public bool isSkillBookPreviewBar()
    {
        return type == SkillSlotType.SkillBookPreviewBar;
    }
    public bool isSubscribingToSkillSet()
    {
        return isActionBar() || isSkillBookPreviewBar();
    }
    public void OnSkillChange(SkillSet skillSet, int changedSkillSlotIndex)
    {
        if (changedSkillSlotIndex == index)
        {
            setSkillSlot(skillSet, changedSkillSlotIndex);
        }
    }
    void setSkillSlot(SkillSet skillSet, int i)
    {
        int skillId = skillSet.GetSkillIdBySkillIndex(i);
        this.refSkillId = skillId;
        if (skillId == 0) { this.Disable(); }
        else
        {
            this.Enable();
            SkillData skillData = AssetDataManager.Instance.GetPlayerSkillById(skillId);
            this.SetIcon(skillData.icon);
        }
    }
    public void setSkillSlotById(int id)
    {
        this.Enable();
        SkillData skillData = AssetDataManager.Instance.GetPlayerSkillById(id);
        this.SetIcon(skillData.icon);
        this.displayName.text = skillData.displayName;
    }

    //======== https://www.patrykgalach.com/2019/05/09/drag-and-drop-in-unity/
    private Transform _originalParent;
    //private GameObject tempDragImage;
    public void OnBeginDrag(PointerEventData eventData)
    {
        //tempDragImage = Instantiate(Image.gameObject);

        Debug.Log("OnBeginDrag " + eventData);
        _originalParent = ImageForDrag.transform.parent;
        Canvas cv = UIData.Instance.mainCanvas;
        //transform.SetParent(cv.transform);
        ImageForDrag.transform.SetParent(cv.transform);
        //transform.SetAsLastSibling();
        ImageForDrag.transform.SetAsLastSibling();

        //ImageForDrag.transform.localPosition += new Vector3(eventData.delta.x, eventData.delta.y, 0) / transform.lossyScale.x;
        Vector3 spos = Input.mousePosition;
        Vector2 localPos = Vector2.zero;
        if (UIData.Instance.mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UIData.Instance.mainCanvasRT, spos, null, out localPos);
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UIData.Instance.mainCanvasRT, spos, UIData.Instance.mainCanvas.worldCamera, out localPos);
        }

        ImageForDrag.transform.localPosition = localPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag: " + eventData);
        Vector3 spos = Input.mousePosition;
        Vector2 localPos = Vector2.zero;
        if (UIData.Instance.mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UIData.Instance.mainCanvasRT, spos, null, out localPos);
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UIData.Instance.mainCanvasRT, spos, UIData.Instance.mainCanvas.worldCamera, out localPos);
        }

        float imageWidth = ImageForDrag.gameObject.GetComponent<RectTransform>().rect.width;
        float imageHeight = ImageForDrag.gameObject.GetComponent<RectTransform>().rect.height;
        Debug.Log("localPos" + localPos + " == imageWidth: " + imageWidth + " == imageHeight: " + imageHeight);
        //localPos.x = localPos.x - imageWidth / 2;
        //localPos.y = localPos.y + imageHeight / 2;
        ImageForDrag.transform.localPosition = localPos;
        //ImageForDrag.transform.localPosition += new Vector3(eventData.delta.x, eventData.delta.y, 0) / transform.lossyScale.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag: " + eventData);
        ImageForDrag.transform.SetParent(_originalParent);
        //transform.SetParent(_originalParent);
        //transform.localPosition = Vector3.zero;
        ImageForDrag.transform.localPosition = Vector3.zero;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (this.isSubscribingToSkillSet())
        {
            int DroppedSkillId = eventData.pointerDrag.GetComponent<SkillSlot>().refSkillId;
            Debug.Log("On Drop --- " + DroppedSkillId);
            Debug.Log("On Drop This --- Index" + this.index + " Type:" + this.type);
            SkillSet skillSet = PlayerManager.Instance.go_Player.GetComponent<SkillSet>();
            switch (this.index)
            {
                case 0:
                    skillSet.skill_1_id = DroppedSkillId;
                    break;
                case 1:
                    skillSet.skill_2_id = DroppedSkillId;
                    break;
                case 2:
                    skillSet.skill_3_id = DroppedSkillId;
                    break;
                case 3:
                    skillSet.skill_4_id = DroppedSkillId;
                    break;
                case 4:
                    skillSet.skill_5_id = DroppedSkillId;
                    break;
                case 5:
                    skillSet.skill_ult_id = DroppedSkillId;
                    break;
                default:
                    Debug.LogError("Skill not Found");
                    break;
            }
        }
        //GameObject go = eventData.pointerDrag;
        //if (go != null)
        //{
        //    go.transform.parent = transform;
        //    go.transform.localPosition = Vector3.zero;
        //}
    }
}
