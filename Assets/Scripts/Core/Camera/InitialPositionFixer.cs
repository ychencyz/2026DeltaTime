using UnityEngine;

public class InitialPositionFixer : MonoBehaviour
{
    private CharacterController _controller;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _controller.enabled = false;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        _controller.enabled = true;

        Debug.Log("角色初始位置已校準，防止掉落。");
    }

}
