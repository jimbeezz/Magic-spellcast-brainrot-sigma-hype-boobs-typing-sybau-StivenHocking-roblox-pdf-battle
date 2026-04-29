namespace SpellCaster.WinForms.Models;

public sealed class EnemyDefinition
{
    public string Id { get; init; } = "training_dummy";
    public string Name { get; init; } = "Training Dummy";
    public string Type { get; init; } = "normal";
    public int Order { get; init; }
    public int RequiredWins { get; init; }
    public int RequiredTier { get; init; } = 1;
    public string Portrait { get; init; } = "images/5424927742693676019.jpg";
    public string Trait { get; init; } = "Practice target.";
    public int MaxHp { get; init; } = 120;
    public int MaxMana { get; init; } = 100;
    public int AttackDamage { get; init; } = 10;
    public int AttackEveryMs { get; init; } = 3000;
    public string VulnerableTo { get; init; } = "arcane";
    public string ResistantTo { get; init; } = "none";
    public int RewardXp { get; init; } = 40;
    public bool IsTraining => string.Equals(Type, "training", StringComparison.OrdinalIgnoreCase);

    public override string ToString()
    {
        var prefix = IsTraining ? "Training" : string.Equals(Type, "boss", StringComparison.OrdinalIgnoreCase) ? "Boss" : $"Level {Order}";
        return $"{prefix}: {Name}";
    }
}
