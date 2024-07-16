// EnemyUnit.cs

public class EnemyUnit : Unit
{
 
    // New attributes for skills
    public int skillDamage;
    public int ultimateDamage;

    // Methods for using skills
    public void UseSkill(PlayerUnit player)
    {
        // Example skill behavior
        player.TakeDamage(skillDamage);
    }

    // Methods for using ultimate
    public void UseUltimate(PlayerUnit player)
    {
        // Example ultimate behavior
        player.TakeDamage(ultimateDamage);
    }
    public void EnemySkillAttack(PlayerUnit player)
    {
        player.TakeDamage(skillDamage);
    }
}
