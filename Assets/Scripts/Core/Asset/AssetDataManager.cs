using UnityEngine;
using System.Collections.Generic;

public class AssetDataManager : MonoBehaviour
{
    public static AssetDataManager Instance { get; private set; }

    [SerializeField]
    private List<AssetData> allItemsList;
    [SerializeField]
    private List<AssetData> allSkillsList;

    private Dictionary<int, AssetData> itemDictionary;
    private Dictionary<int, SkillData> skillDictionary;
    private Dictionary<int, SkillData> runtimePlayerSkillDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            InitializeDatabase();
        }
    }

    private void InitializeDatabase()
    {
        itemDictionary = new Dictionary<int, AssetData>();
        foreach (AssetData item in allItemsList)
        {
            if (!itemDictionary.ContainsKey(item.id))
            {
                itemDictionary.Add(item.id, item);
            }
            else
            {
                Debug.LogWarning($"Duplicate item ID found: {item.id} for item");
            }
        }
        skillDictionary = new Dictionary<int, SkillData>();
        foreach (SkillData item in allSkillsList)
        {
            if (!skillDictionary.ContainsKey(item.id))
            {
                skillDictionary.Add(item.id, item);
            }
            else
            {
                Debug.LogWarning($"Duplicate skill ID found: {item.id} for item");
            }
        }
        runtimePlayerSkillDictionary = new Dictionary<int, SkillData>();
        //暫時先用all skills當player skills
        foreach (SkillData item in allSkillsList)
        {
            if (!runtimePlayerSkillDictionary.ContainsKey(item.id))
            {
                runtimePlayerSkillDictionary.Add(item.id, Instantiate(item));
            }
            else
            {
                Debug.LogWarning($"Duplicate skill ID found: {item.id} for item");
            }
        }
    }

    public AssetData GetItemById(int id)
    {
        if (itemDictionary.TryGetValue(id, out AssetData item))
        {
            return item;
        }
        return null; // Return null if not found
    }
    public SkillData GetSkillById(int id)
    {
        if (skillDictionary.TryGetValue(id, out SkillData item))
        {
            return item;
        }
        return null; // Return null if not found
    }
    public SkillData GetPlayerSkillById(int id)
    {
        if (runtimePlayerSkillDictionary.TryGetValue(id, out SkillData item))
        {
            return item;
        }
        return null; // Return null if not found
    }
}
