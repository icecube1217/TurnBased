// PlayerUnit.cs

public class PlayerUnit : Unit
{
 
    // New attributes for skills
    public int skillDamage;
    public int ultimateDamage;

    // Methods for using skills
    public void UseSkill(EnemyUnit enemy)
    {
        // Example skill behavior
        enemy.TakeDamage(skillDamage);
    }

    // Methods for using ultimate
    public void UseUltimate(EnemyUnit enemy)
    {
        // Example ultimate behavior
        enemy.TakeDamage(ultimateDamage);
    }
}
