using System.Collections;
using System.Linq;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private EnemyDelegates enemyDelegates;
    private Animator animator;
    [SerializeField]
    private GameObject playerWeaponHandSlot;
    [SerializeField]
    private GameObject playerWeaponBackSlot;

    private void Awake()
    {
        enemyDelegates = GetComponent<EnemyDelegates>();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        enemyDelegates.OnSkillStart += SkillAnimation;
        enemyDelegates.OnSkillInterrupted += InterruptAnimation;
    }
    private void OnDisable()
    {
        enemyDelegates.OnSkillStart -= SkillAnimation;
        enemyDelegates.OnSkillInterrupted -= InterruptAnimation;
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
    private void SkillAnimation(SkillData skillData)
    {
        if(skillData.animationGroups.Length < 1) return;

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
        animator.SetInteger("Action", _actionId);
        AnimationClip actionClip = skillData.animationGroups[__clipIndex].actionClip;
        float clipLength = actionClip.length - 0.2f;
        actionAnimationRoutine = StartCoroutine(CountdownRoutine(clipLength));
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
        animator.SetInteger("Action", -1);
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
            animator.SetInteger("Action", -1);
        }
    }

    public void FootL()
    {
        //¦û¦ì¨¾¤î³ø¿ù
    }
    public void FootR()
    {
        //¦û¦ì¨¾¤î³ø¿ù
    }
    public void Hit() //weapon hit frame
    {
        //¦û¦ì¨¾¤î³ø¿ù
    }
    public void OnFootstep()
    {
        //¦û¦ì¨¾¤î³ø¿ù
    }
    private void InterruptAnimation(SkillData skillData)
    {
        StopCoroutine(actionAnimationRoutine);
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
    }
}
