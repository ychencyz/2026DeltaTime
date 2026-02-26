using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;


public class PlayerInputs : MonoBehaviour
{
    private SkillSet skillSet;
    private SkillController skillController;
    private StatusSystem statusSystem;

    void Start()
    {
        skillSet = GetComponent<SkillSet>();
        skillController = GetComponent<SkillController>();
        statusSystem = GetComponent<StatusSystem>();
    }
    public void InvokeAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PlayerDelegates.Instance.OnAttackStart?.Invoke();
            //Debug.Log("Attack first pressed!");
        }
        if (context.performed)
        {
            if (context.interaction is TapInteraction)
            {
                PlayerDelegates.Instance.OnLightAttack?.Invoke();
                //Debug.Log("LightAttack Tap performed!");
            }
            else if (context.interaction is HoldInteraction)
            {
                PlayerDelegates.Instance.OnHeavyAttack?.Invoke();
                //Debug.Log("HeavyAttack Hold performed!");
            }
        }
        //注:canceled會在tap轉hold中間call一次
        if (context.canceled)
        {
            PlayerDelegates.Instance.OnAttackCancel?.Invoke();
            //Debug.Log("Attack was canceled!");
        }
    }
    public void InvokeBlock(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PlayerDelegates.Instance.OnBlockStart?.Invoke();
            Debug.Log("block first pressed!");
            skillController.Interrupt();
            //OnSkillCancel?.Invoke();
            //Debug.Log("skill canceled!");
        }
        if (context.performed)
        {
            Debug.Log("block was performed!");
        }
        if (context.canceled)
        {
            PlayerDelegates.Instance.OnBlockCancel?.Invoke();
            Debug.Log("block was canceled!");
        }
    }
    public void InvokeSkill(InputAction.CallbackContext context)
    {
        Debug.Log(context.action.name);
        int skillId = skillSet.GetSkillIdByActionName(context.action.name);
        skillController.data = AssetDataManager.Instance.GetPlayerSkillById(skillId);

        if (context.started)
        {
            skillController.TryCast();
            PlayerDelegates.Instance.OnSkillStart?.Invoke();
            Debug.Log("skill first pressed!");
        }
    }
    public void InvokeBreakFree(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("BreakFree pressed!");
        }
    }
}
