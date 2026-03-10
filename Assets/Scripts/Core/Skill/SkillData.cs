using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillData", menuName = "Skills/SkillData")]
public class SkillData : AssetData
{
    public SkillState state = SkillState.Idle;
    public Vector3 position = Vector3.zero;
    public Coroutine skillRoutine;
    public string displayName = "default Name";
    public float anticipationTime = 0.15f; // «e·n
    public float recoveryTime = 0f;     // «á·n
    public float cooldownTime = 2.0f;     // §N«o
    public float damage = 10f;
    public int actionId = -1;
    public AnimationClip actionClip;
    public GameObject VFXPrefab;
    public Sprite icon;
}
