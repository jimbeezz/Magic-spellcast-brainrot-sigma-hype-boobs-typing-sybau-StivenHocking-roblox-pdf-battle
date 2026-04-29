namespace SpellCaster.WinForms.Models;

public sealed class SpellDefinition
{
    public string Name { get; init; } = string.Empty;
    public string Pattern { get; init; } = string.Empty;
    public int? CastWindowMs { get; init; }
    public string School { get; init; } = "arcane";
    public string Effect { get; init; } = "none";
    public int BasePower { get; init; } = 12;
    public int ManaCost { get; init; } = 10;
    public int Tier { get; init; } = 1;

    public override string ToString()
    {
        return $"T{Tier} {Name} - {Pattern}";
    }
}
