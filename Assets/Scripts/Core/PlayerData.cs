using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }
    //public PlayerInput PlayerInput;
    //public SkillController SkillController;
    //public SkillSet SkillSet;
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

    //private void OnEnable()
    //{
    //    GameDelegates.Instance.OnPlayerAwake += OnPlayerAwake;
    //}
    //private void OnDisable()
    //{
    //    GameDelegates.Instance.OnPlayerAwake -= OnPlayerAwake;
    //}
    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{

    //}



    //// Update is called once per frame
    //void Update()
    //{

    //}
    //void OnPlayerAwake(GameObject player)
    //{
    //    PlayerInput = player.GetComponent<PlayerInput>();
    //    SkillController = player.GetComponent<SkillController>();
    //    SkillSet = player.GetComponent<SkillSet>();
    //}
}
