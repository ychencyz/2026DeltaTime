using UnityEngine;

public class PlayerRuntimeData : MonoBehaviour
{
    public static PlayerRuntimeData Instance { get; private set; }
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
