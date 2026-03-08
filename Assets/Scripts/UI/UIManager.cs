using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    //public GameObject TargetUI;
    //public GameObject MainActionUI;
    //public GameObject QuestUI;
    //public UIWindow InventoryUI;
    //public UIActionBar UIActionBar;
    public Action OnOpenDefaultLayout;
    //public Action OnSkillChanged;

    public enum States
    {
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
        get
        {
            return _state;
        }
    }
    //private void OnEnable()
    //{
    //}
    //private void OnDisable()
    //{
    //}

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
        //InventoryUI.Open();
        //CloseAllUI();
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
    public void CloseAllUI()
    {
        //foreach (Transform childTransform in transform)
        //{
        //    childTransform.gameObject.SetActive(false);
        //}
    }
}
