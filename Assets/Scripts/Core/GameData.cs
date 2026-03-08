using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }
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
    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
    //const string PlayerActionNameSkill_1 = "Skill1";
    //const string PlayerActionNameSkill_2 = "Skill2";
    //const string PlayerActionNameSkill_3 = "Skill3";
    //const string PlayerActionNameSkill_4 = "Skill4";
    //const string PlayerActionNameSkill_5 = "Skill5";
    //Dictionary<string, string> PlayerActions = new Dictionary<string, string>
    //{
    //    { "Skill1", "London, Manchester, Birmingham" },
    //    { "Skill2", "Chicago, New York, Washington" },
    //    { "India", "Mumbai, New Delhi, Pune" }
    //};
    public string GetActionMapPlayerKeyNameByActionName(string actionName)
    {
        string keyName = InputActionAsset.FindActionMap("Player").FindAction(actionName).GetBindingDisplayString(0);
        return keyName;
    }
}
