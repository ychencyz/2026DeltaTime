using UnityEngine;

public class PlayerFSM : MonoBehaviour
{
    public enum playerState
    {
        Idle,
        Move,
        Combat, 
        Die
    }
    public playerState nowState= playerState.Idle;
    private void FixedUpdate()
    {
        switch (nowState)
        {
            case playerState.Idle:
                break;

            case playerState.Move:
                break;

            case playerState.Combat:
                break;

            case playerState.Die:
                break;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (nowState == playerState.Idle)
        {

        }
        if (nowState == playerState.Move)
        {

        }
        if (nowState == playerState.Combat)
        {

        }
        
        if (nowState == playerState.Die)
        {

        }
    }
}
