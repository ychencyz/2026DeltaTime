using System;
using UnityEngine;

public enum SkillTargetType
{
    Self,
    SelfFollow,
    Target,
    TargetFollow,
}

[Serializable]
public class AnimationGroup
{
    public int actionId;
    public AnimationClip actionClip;
}

[CreateAssetMenu(fileName = "NewSkillData", menuName = "Skills/SkillData")]
public class SkillData : AssetData
{
    public SkillState state = SkillState.Idle;
    public Vector3 position = Vector3.zero;
    public Coroutine skillRoutine;
    public string displayName = "default Name";
    public float anticipationTime = 0.15f; // �e�n
    public float recoveryTime = 0f;     // ��n
    public float cooldownTime = 2.0f;     // �N�o
    public float damage = 10f;
    public int aniticipationActionId = -1;
    public SkillTargetType targetType;
    public AnimationGroup[] animationGroups;
    public GameObject VFXPrefab;
    public Sprite icon;
}
