using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[RequireComponent(typeof(SkillSet))]
public class PlayerInputs : MonoBehaviour
{
    private SkillSet skillSet;
    private SkillController skillController;
    private StatusSystem statusSystem;
    private PlayerInput playerInput;
    private PlayerAnimations playerAnimations;
    void Start()
    {
        skillSet = GetComponent<SkillSet>();
        skillController = GetComponent<SkillController>();
        statusSystem = GetComponent<StatusSystem>();
        playerInput = GetComponent<PlayerInput>();
        playerAnimations = GetComponent<PlayerAnimations>();
    }

    public void InvokeAttack(InputAction.CallbackContext context)
    {
        //if (context.started)
        //{
        //    //Debug.Log("Attack --- context.started!");
        //    //    PlayerDelegates.Instance.OnAttackStart?.Invoke();
        //}
        if (context.performed)
        {
            //Debug.Log("Attack --- context.performed!");
            if (context.interaction is TapInteraction)
            {
                //Debug.Log("LightAttack --- Tap performed!");
                SkillData skillData = skillSet.GetSkillByActionName("LightAttack");
                skillController.TryCast(skillData, Vector3.zero);
            }
            else if (context.interaction is HoldInteraction)
            {
                SkillData heavySkillData = skillSet.GetSkillByActionName("HeavyAttack");
                if (heavySkillData != null)
                {
                    skillController.TryCast(heavySkillData, Vector3.zero);
                }
            }
        }
        //////�`:canceled�|�btap��hold����call�@��
        //if (context.canceled)
        //{
        //    Debug.Log("Attack --- context.canceled!");
        //    //    PlayerDelegates.Instance.OnAttackCancel?.Invoke();
        //}
    }
    public void InvokeBlock(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            skillController.Interrupt();
            statusSystem.isBlocking = true;
        }
        if (context.canceled)
        {
            statusSystem.isBlocking = false;
        }
    }
    public void InvokeSkill(InputAction.CallbackContext context)
    {
        SkillData skillData = skillSet.GetSkillByActionName(context.action.name);

        if (context.started)
        {
            skillController.TryCast(skillData, Vector3.zero);
            //Debug.Log(context.action.name + "pressed");
            //Debug.Log("skill first pressed!");
        }
    }
    public void InvokeBreakFree(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            statusSystem.BreakFree();
        }
    }

    public void ToggleInputSystem(InputAction.CallbackContext context)
    {
        string currentActionMap = playerInput.currentActionMap.name;
        if (context.started)
        {
            if (currentActionMap == "Player")
            {
                SwitchToUIMap();
            }
            else
            {
                SwitchToPlayerMap();
            }
        }
    }

    public void SwitchToPlayerMap()
    {
        // Switch to a new map, which automatically disables the previous one
        playerInput.SwitchCurrentActionMap("Player");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //Time.timeScale = 1f; // Resumes game time
        Debug.Log("ACTION MAP: switched to Player");
    }
    public void SwitchToUIMap()
    {
        // Switch to a new map, which automatically disables the previous one
        playerInput.SwitchCurrentActionMap("UI");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //Time.timeScale = 0f; // Pauses game time
        Debug.Log("ACTION MAP: switched to UI");
    }
    public void ToggleCombatPose(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerAnimations.ToggleCombatPose();
        }
    }
}
