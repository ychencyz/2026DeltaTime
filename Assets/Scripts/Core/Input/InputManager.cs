using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public PlayerInput playerInput;
    //playerInput.onControlsChanged 

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Debug.Log(playerInput.defaultActionMap);
        //TODO: ±±¨îplayer inputs map

        //    playerInput.currentActionMap?.Enable();
        //Debug.Log(playerInput.actions["Skill1"].ToShortString());
        //Debug.Log(playerInput.actions["Player"]);
        Debug.Log(playerInput.currentActionMap);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
