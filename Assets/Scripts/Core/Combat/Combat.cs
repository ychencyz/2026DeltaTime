using System;
using Unity.VisualScripting;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public void Attack(SkillData data)
    {
        SetTargetPosition(out data.position, data);
        if (data.VFXPrefab != null)
        {
            GameObject obj = Instantiate(data.VFXPrefab, data.position, Quaternion.identity);
            float animationDuration = obj.GetComponent<ParticleSystem>().main.duration;
            Destroy(obj, animationDuration);
        }

        Debug.Log($"Skill [{data.displayName}] Excuted!");
    }
    private void SetTargetPosition(out Vector3 targetPosition, SkillData data)
    {
        // TODO: set target position
        if (data.targetType == SkillTargetType.Self)
        {
            targetPosition=transform.position;
        }
        else if (data.targetType == SkillTargetType.SelfFollow)
        {
            targetPosition = transform.position;
        }
        else if (data.targetType == SkillTargetType.Target)
        {
            targetPosition = transform.position;
        }
        else if (data.targetType == SkillTargetType.TargetFollow)
        {
            targetPosition = transform.position;
        }
        else
        {
            throw new Exception("targetType not found, skill id:" + data.id+ ", SkillTargetType:" + data.targetType);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
