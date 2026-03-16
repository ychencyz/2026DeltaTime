using System.Collections.Generic;
using UnityEngine;

public enum SkillTargetType
{
    Self,
    SelfFollow,
    Target,
    TargetFollow,
}
[System.Serializable]
public class SkillAnimation
{
    public int actionId;
    public AnimationClip actionClip;
}
[CreateAssetMenu(fileName = "NewSkillData", menuName = "Skills/SkillData")]
public class SkillData : AssetData
{
    [HideInInspector]
    public SkillState state = SkillState.Idle;
    [HideInInspector]
    public Vector3 position = Vector3.zero;
    [HideInInspector]
    public Coroutine skillRoutine;
    public string displayName = "default Name";
    public float anticipationTime = 0.15f; // «e·n
    public float recoveryTime = 0f;     // «á·n
    public float cooldownTime = 2.0f;     // §N«o
    public float damage = 10f;
    public SkillTargetType targetType;
    public int actionId = -1;
    public AnimationClip actionClip;
    [HideInInspector]
    public int currentSkillAction;
    public List<SkillAnimation> skillActionList;
    public GameObject VFXPrefab;
    public Sprite icon;
}
