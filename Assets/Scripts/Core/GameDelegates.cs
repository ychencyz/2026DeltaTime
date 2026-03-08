using System;
using UnityEngine;

public class GameDelegates : MonoBehaviour
{
    public static GameDelegates Instance { get; private set; }
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
}
