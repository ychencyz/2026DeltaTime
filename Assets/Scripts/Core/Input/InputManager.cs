using UnityEngine;
using UnityEngine.InputSystem;

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

    //void Start()
    //{
    //    //Debug.Log(playerInput.defaultActionMap);
    //    //TODO: ±±¨îplayer inputs map

    //    //    playerInput.currentActionMap?.Enable();
    //    //Debug.Log(playerInput.actions["Skill1"].ToShortString());
    //    //Debug.Log(playerInput.actions["Player"]);
    //    Debug.Log(playerInput.currentActionMap);
    //}
}
