using UnityEngine;
using UnityEditor;
public class Skill : ScriptableObject
{

    public WarriorTypeSkill warriorTypeSkill;
    public MagicionTypeSkill magicionTypeSkill;
    public ArcherTypeSkill archerTypeSkill;
    public string description;
    public Sprite icon;
    [MenuItem("Rpg/Skill/Create Skill")]
    static void CreateNewSkillData()
    {
        Skill skill = ScriptableObject.CreateInstance<Skill>();
        AssetDatabase.CreateAsset(skill, "Assets/Skill.asset");
    }
}

