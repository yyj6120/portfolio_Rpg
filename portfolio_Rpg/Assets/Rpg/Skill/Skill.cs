using UnityEngine;
using UnityEditor;
public class Skill : ScriptableObject
{
    public SkillofWariior skillofWariior;
    public SkillofMagicion skillofMagicion;
    public SkillofArcher skillofArcher;
    public string description;
    public Sprite icon;
    public int skillID;

    [MenuItem("Rpg/Skill/Create Skill")]
    static void CreateNewSkillData()
    {
        Skill skill = ScriptableObject.CreateInstance<Skill>();
        AssetDatabase.CreateAsset(skill, "Assets/Skill.asset");
    }
}

