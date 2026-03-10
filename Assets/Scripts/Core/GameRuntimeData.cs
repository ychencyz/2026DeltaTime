using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameRuntimeData : MonoBehaviour
{
    public static GameRuntimeData Instance { get; private set; }
    public InputActionAsset InputActionAsset;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public string GetActionMapPlayerKeyNameByActionName(string actionName)
    {
        string keyName = InputActionAsset.FindActionMap("Player").FindAction(actionName).GetBindingDisplayString(0);
        return keyName;
    }
}
