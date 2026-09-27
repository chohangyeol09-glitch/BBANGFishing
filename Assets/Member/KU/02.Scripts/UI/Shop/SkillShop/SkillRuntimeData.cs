using System;
using Member.JJK._02._Scripts.Skill;

[Serializable]
public class SkillRuntimeData
{
    public SkillSO Skill { get; private set; }

    // JJK의 SkillSO는 레벨 개념이 없어서 항상 1레벨/최대레벨로 취급한다.
    public int Level => 1;

    public bool IsMaxLevel => true;

    public SkillRuntimeData(SkillSO skill)
    {
        Skill = skill;
    }

    public bool Upgrade() => false;
}
