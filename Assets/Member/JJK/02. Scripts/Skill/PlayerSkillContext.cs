namespace Member.JJK._02._Scripts.Skill
{
    public class PlayerSkillContext
    {
        public PlayerCombatState CombatState { get; }

        public PlayerSkillContext(PlayerCombatState combatState)
        {
            CombatState = combatState;
        }
    }
}
