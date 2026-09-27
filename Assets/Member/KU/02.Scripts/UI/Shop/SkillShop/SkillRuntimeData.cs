using System;

[Serializable]
public class SkillRuntimeData
{
    public KU_SkillSO Skill { get; private set; }

    public int Level { get; private set; }


    public bool IsMaxLevel
    {
        get
        {
            if (Skill == null)
                return true;


            return Level >= Skill.maxLevel;
        }
    }


    public SkillRuntimeData(
        KU_SkillSO skill)
    {
        Skill = skill;

        Level = 1;
    }


    public bool Upgrade()
    {
        if (Skill == null)
            return false;


        if (IsMaxLevel)
            return false;


        Level++;

        return true;
    }
}