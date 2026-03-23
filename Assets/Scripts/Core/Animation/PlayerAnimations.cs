using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    private GameObject playerWeaponHandSlot;
    [SerializeField]
    private GameObject playerWeaponBackSlot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        PlayerDelegates.Instance.OnSkillStart += OnSkillStart;
        PlayerDelegates.Instance.OnSkillExecute += OnSkillExecute;
        PlayerDelegates.Instance.OnSkillInterrupted += InterruptAnimation;
        PlayerDelegates.Instance.OnRollDodgeStart += OnRollDodgeStart;
        PlayerDelegates.Instance.OnRollDodgeEnd += OnRollDodgeEnd;
    }
    private void OnDisable()
    {
        PlayerDelegates.Instance.OnSkillStart -= OnSkillStart;
        PlayerDelegates.Instance.OnSkillExecute -= OnSkillExecute;
        PlayerDelegates.Instance.OnSkillInterrupted -= InterruptAnimation;
        PlayerDelegates.Instance.OnRollDodgeStart -= OnRollDodgeStart;
        PlayerDelegates.Instance.OnRollDodgeEnd -= OnRollDodgeEnd;

    }
    private Coroutine actionAnimationRoutine;
    private int prevSkillId = -1;
    private int _clipIndex = -1;
    private Coroutine resetActionRoutine;
    private IEnumerator resetActionCountdownRoutine(float _countDownFrom)
    {
        float currentTime = _countDownFrom;
        float interval = 0.1f;
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(interval);
            currentTime -= interval;
        }
        // reset index
        _clipIndex = -1;
    }
    private void OnSkillStart(SkillSet SkillSet, SkillData skillData)
    {
        if (skillData.aniticipationActionId > 0)
        {
            StartAction(skillData.aniticipationActionId);

            return;
        }

        SkillAnimation(SkillSet, skillData);
    }
    private void OnSkillExecute(SkillSet SkillSet, SkillData skillData)
    {
        if (skillData.aniticipationActionId < 1) return;

        SkillAnimation(SkillSet, skillData);
    }
    private void SkillAnimation(SkillSet SkillSet, SkillData skillData)
    {
        if (skillData.animationGroups.Length < 1) return;
        // if (animator.GetBool("inCombat") != true) return;

        // get index
        _clipIndex = _clipIndex + 1;

        // reset to 0 if (1. prev skill Id changed, )
        if (prevSkillId != skillData.id)
        {
            _clipIndex = 0;
        }
        prevSkillId = skillData.id;

        // (3. exceed 1s)
        if (resetActionRoutine != null)
        {
            StopCoroutine(resetActionRoutine);
        }
        resetActionRoutine = StartCoroutine(resetActionCountdownRoutine(2f));

        // (2. index out of range)
        if (_clipIndex > skillData.animationGroups.Length - 1 || _clipIndex < 0)
        {
            _clipIndex = 0;
        }

        // get clip
        int __clipIndex = _clipIndex == -1 ? 0 : _clipIndex;
        int _actionId = skillData.animationGroups[__clipIndex].actionId;
        // if id < 0 return
        if (_actionId < 0) return;
        StartAction(_actionId);
        AnimationClip actionClip = skillData.animationGroups[__clipIndex].actionClip;
        float clipLength = actionClip.length - 0.2f;
        actionAnimationRoutine = StartCoroutine(CountdownRoutine(clipLength));
    }
    private void OnRollDodgeStart(String actionName)
    {
        switch (actionName)
        {
            case "DodgeFoward":
                animator.SetInteger("Roll", 1);
                break;
            case "DodgeRight":
                animator.SetInteger("Roll", 2);

                break;
            case "DodgeBackward":
                animator.SetInteger("Roll", 3);

                break;
            case "DodgeLeft":
                animator.SetInteger("Roll", 4);

                break;
            default:
                throw new ArgumentException("RollDodge wrongfully called，input action name: " + actionName);
        }
    }
    private void OnRollDodgeEnd()
    {
        animator.SetInteger("Roll", -1);
    }
    private IEnumerator CountdownRoutine(float _countDownFrom)
    {
        float currentTime = _countDownFrom;
        float interval = 0.1f;
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(interval);
            currentTime -= interval;
        }
        EndAction();
    }
    enum WeaponState
    {
        Sheath = -1,
        UnSheath = 1,
    }
    public void WeaponSwitch()
    {
        int weaponState = animator.GetInteger("m_Weapon");
        int newWeaponState = weaponState switch
        {
            (int)WeaponState.Sheath => (int)WeaponState.UnSheath,
            (int)WeaponState.UnSheath => (int)WeaponState.Sheath,
            _ => throw new System.Exception("m_Weapon variable Error")
        };
        animator.SetInteger("m_Weapon", newWeaponState);
        if (newWeaponState == (int)WeaponState.UnSheath)
        {
            playerWeaponHandSlot.SetActive(true);
            playerWeaponBackSlot.SetActive(false);
        }
        if (newWeaponState == (int)WeaponState.Sheath)
        {
            playerWeaponHandSlot.SetActive(false);
            playerWeaponBackSlot.SetActive(true);
            EndAction();
        }
    }
    public void FootL()
    {
        //佔位防止報錯
    }
    public void FootR()
    {
        //佔位防止報錯
    }
    public void Hit() //weapon hit frame
    {
        //佔位防止報錯
    }
    public void Shoot() //weapon hit frame
    {
        //佔位防止報錯
    }
    private void InterruptAnimation(SkillSet SkillSet, SkillData skillData)
    {
        //StopCoroutine(actionAnimationRoutine);
        StopAllCoroutines();
        animator.SetInteger("Action", -1);
    }
    public void ToggleCombatPose()
    {
        bool inCombat = animator.GetBool("inCombat");
        if (inCombat)
        {
            animator.SetBool("inCombat", false);
        }
        else
        {
            animator.SetBool("inCombat", true);
            //TODO: if in combat state cannnot switch to idle
        }
        StartCoroutine(ToggleCombatRoutine(0.5f));

        IEnumerator ToggleCombatRoutine(float interval)
        {
            yield return new WaitForSeconds(interval);
        }
    }
    public void StartAction(int actionId)
    {
        animator.SetInteger("Action", actionId);
    }
    public void EndAction()
    {
        animator.SetInteger("Action", -1);
    }
    public int GetCurrentActionId() {
        return animator.GetInteger("Action");
    }
    public int GetCurrentRollId()
    {
        return animator.GetInteger("Roll");
    }
}
