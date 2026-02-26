using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject TargetUI;
    public GameObject MainActionUI;
    public GameObject QuestUI;
    public UIWindow InventoryUI;
    public UIActionBar UIActionBar;
    public Action OnOpenDefaultLayout;
    //public Action OnSkillChanged;

    public enum States {
        Default,
        Inventory,
        Quest,
    }
    States _state = States.Default;

    public States State
    {
        set
        {

        }
        get {
            return _state;
        }
    }
    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InventoryUI.Open();
        //CloseAllUI();
        UIActionBar.InitUIActionBar();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
    public void CloseAllUI()
    {
        foreach (Transform childTransform in transform)
        {
            childTransform.gameObject.SetActive(false);
        }
    }
}
