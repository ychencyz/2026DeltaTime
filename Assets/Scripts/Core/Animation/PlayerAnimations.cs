using RPGCharacterAnims.Lookups;
using System;
using System.Drawing;
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
        PlayerDelegates.Instance.OnLightAttack += LightAttackAnimation;
    }
    private void OnDisable()
    {
        PlayerDelegates.Instance.OnLightAttack -= LightAttackAnimation;
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

        Debug.Log("weapon Switched");
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
    public void ActionDoneInCombat()
    {
        animator.SetInteger("Action", -1);
    }
    private void LightAttackAnimation()
    {
        if (animator.GetBool("inCombat") == true)
        {
            animator.SetInteger("Action", 1);
        }
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
