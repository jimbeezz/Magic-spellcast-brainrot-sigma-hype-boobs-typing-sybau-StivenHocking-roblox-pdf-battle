namespace SpellCaster.WinForms.Models;

public enum BattleOutcome
{
    InProgress = 0,
    Victory = 1,
    Defeat = 2
}

public sealed class FighterState
{
    public int MaxHp { get; set; }
    public int MaxMana { get; set; }
    public int Hp { get; set; }
    public int Mana { get; set; }
    public bool HasShield { get; set; }

    public int HpPercent => MaxHp <= 0 ? 0 : (int)Math.Round(Hp * 100d / MaxHp);
    public int ManaPercent => MaxMana <= 0 ? 0 : (int)Math.Round(Mana * 100d / MaxMana);
}

public sealed class BattleState
{
    public FighterState Player { get; init; } = new();
    public FighterState Enemy { get; init; } = new();
    public DateTime NextEnemyAttackUtc { get; set; }
    public int EnemyAttackCooldownMs { get; set; }
    public int EnemySlowStacks { get; set; }
    public int EnemyActionCounter { get; set; }
    public bool EnemyHasStoneShield { get; set; }
    public bool EnemyPreparingBurst { get; set; }
    public int DarkMagusCopiedDamage { get; set; }
    public bool DragonBreathReady { get; set; }
    public BattleOutcome Outcome { get; set; } = BattleOutcome.InProgress;
}
