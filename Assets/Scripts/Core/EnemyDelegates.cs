using System;
using UnityEngine;

public class EnemyDelegates : MonoBehaviour
{
    public Action<SkillData> OnSkillStart;
    public Action<SkillData> OnSkillInterrupted;
}
