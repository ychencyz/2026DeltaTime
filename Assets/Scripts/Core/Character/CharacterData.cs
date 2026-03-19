using UnityEngine;

public enum CharacterTypes
{
    Enemy,
    EnemyMiniBoss,
    EnemyBoss,
    Friendly,
    Player,
}
[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Characters/CharacterData")]
public class CharacterData : AssetData
{
    public CharacterTypes type;
    public string displayName = "default Name";
    public int initialHp;
    public int initialMaxHp;
    public float attackRange = 2f;
    public float detectionRadius = 10f;
}
