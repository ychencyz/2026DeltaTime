using System;
using System.Collections;
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
    SkillData heavyAttackSkillData;
    SkillData lightAttackSkillData;
    SkillData BashSkillData;
    SkillData BlockSkillData;
    int BlockSkillActionId;
    void Start()
    {
        skillSet = GetComponent<SkillSet>();
        skillController = GetComponent<SkillController>();
        statusSystem = GetComponent<StatusSystem>();
        playerInput = GetComponent<PlayerInput>();
        playerAnimations = GetComponent<PlayerAnimations>();
        heavyAttackSkillData = skillSet.GetSkillByActionName("HeavyAttack");
        lightAttackSkillData = skillSet.GetSkillByActionName("LightAttack");
        BashSkillData = AssetDataManager.Instance.GetPlayerSkillById(22);
        BlockSkillData = AssetDataManager.Instance.GetPlayerSkillById(23);
        BlockSkillActionId = BlockSkillData.animationGroups[0].actionId;
        SwitchToPlayerMap();
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        if (playerInput != null) SwitchToPlayerMap();
    }
    float nextHeavtCanAttackTime;
    Coroutine StartHeavyCoroutine;
    public void InvokeAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //Debug.Log(Time.time + ": Attack --- context.started!");
            if (!skillController.prevSkillOk) return;
            StartHeavyCoroutine = StartCoroutine(StartHeavyRoutine(0.15f));
            //    PlayerDelegates.Instance.OnAttackStart?.Invoke();
        }
        if (context.performed)
        {
            if (!skillController.prevSkillOk) return;
            //Debug.Log(Time.time + ": Attack --- context.performed!");
            if (StartHeavyCoroutine != null) StopCoroutine(StartHeavyCoroutine);
            if (context.interaction is TapInteraction)
            {
                //Debug.Log("LightAttack --- Tap performed!");
                skillController.Interrupt();
                skillController.TryCast(lightAttackSkillData);
            }
        }
        //if (context.canceled)
        //{
        //    Debug.Log(Time.time + ": Attack --- context.canceled!");
        //}

    }
    private IEnumerator StartHeavyRoutine(float interval)
    {
        yield return new WaitForSeconds(interval);
        nextHeavtCanAttackTime = Time.time + heavyAttackSkillData.anticipationTime;
        skillController.TryCast(heavyAttackSkillData);
    }
    Coroutine ContinueHeavyCoroutine;
    private IEnumerator ContinueHeavyRoutine(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            if (heavyAttackSkillData.state == SkillState.Idle && skillController.data.state == SkillState.Idle)
            {
                skillController.TryCast(heavyAttackSkillData);
            }
        }
    }
    public void InvokeHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!skillController.prevSkillOk) return;
            ContinueHeavyCoroutine = StartCoroutine(ContinueHeavyRoutine(0.15f));
            //Debug.Log(Time.time + ": HeavyAttack --- context.started!");
        }
        //if (context.performed)
        //{
        //    if (context.interaction is HoldInteraction)
        //    {
        //        Debug.Log(Time.time + ": HeavyAttack --- Hold performed!");
        //    }
        //}
        if (context.canceled)
        {
            if (ContinueHeavyCoroutine != null) StopCoroutine(ContinueHeavyCoroutine);
            if (Time.time < nextHeavtCanAttackTime && heavyAttackSkillData.state == SkillState.Anticipation)
            {
                skillController.Interrupt();
                skillController.TryCast(lightAttackSkillData);
            }
            //Debug.Log(Time.time + ": HeavyAttack --- context.canceled!");
            //    PlayerDelegates.Instance.OnAttackCancel?.Invoke();
        }
    }

    Coroutine ContinueBlockCoroutine;
    private IEnumerator ContinueBlockRoutine(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            if (skillController.prevSkillOk && playerAnimations.GetCurrentActionId() != BlockSkillActionId)
            {
                playerAnimations.StartAction(BlockSkillActionId);
            }
        }
    }
    public void InvokeBlock(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //Debug.Log("block started");
            StopAllCoroutines();
            skillController.ForceInterrupt();
            skillController.TryCast(skillSet.gcd);
            playerAnimations.StartAction(BlockSkillActionId);
            ContinueBlockCoroutine = StartCoroutine(ContinueBlockRoutine(0.5f));
            statusSystem.isBlocking = true;
        }
        if (context.canceled)
        {
            //Debug.Log("block canceled");
            if (ContinueBlockCoroutine != null) StopCoroutine(ContinueBlockCoroutine);
            if (playerAnimations.GetCurrentActionId() == BlockSkillActionId)
            {
                playerAnimations.EndAction();
            }
            statusSystem.isBlocking = false;
        }
    }
    public void InvokeSkill(InputAction.CallbackContext context)
    {
        SkillData skillData = skillSet.GetSkillByActionName(context.action.name);

        if (context.started)
        {
            skillController.TryCast(skillData);
            //Debug.Log(context.action.name + "pressed");
            //Debug.Log("skill first pressed!");
        }
    }
    public void InvokeRollDodge(InputAction.CallbackContext context)
    {
        if (playerAnimations.GetCurrentRollId() < 1)
        {
            PlayerDelegates.Instance.OnRollDodgeStart?.Invoke(context.action.name);
        }
    }
    public void InvokeBash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Debug.Log("bash performed!");
            StopAllCoroutines();
            skillController.Interrupt();
            skillController.TryCast(BashSkillData);
            PlayerDelegates.Instance.OnBash?.Invoke();
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
        if (context.performed)
        {
            string currentActionMap = playerInput.currentActionMap.name;
            if (currentActionMap == "Player")
            {
                SwitchToUIMap();
            }
            else
            {
                SwitchToPlayerMap();
            }
        }
        //if (context.canceled)
        //{
        //    //佔位防止報錯
        //}
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
            if (skillController.prevSkillOk)
            {
                skillController.TryCast(skillSet.gcd);
                playerAnimations.ToggleCombatPose();
            }
        }
    }
}
