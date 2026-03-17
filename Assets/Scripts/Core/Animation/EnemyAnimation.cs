using System.Collections;
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
    private void SkillAnimation(SkillData skillData)
    {
        //if (animator.GetBool("inCombat") != true) return;
        //TODO: 有裝態機後改掉:
        if (animator.GetBool("inCombat") != true)
        {
            animator.SetBool("inCombat", true);
        }

        if (skillData.actionId > 0)
        {
            animator.SetInteger("Action", skillData.actionId);
            AnimationClip actionClip = skillData.actionClip;
            float clipLength = actionClip.length - 0.2f;
            actionAnimationRoutine = StartCoroutine(CountdownRoutine(clipLength));
        }
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
