namespace SpellCaster.WinForms.Models;

public sealed class BattleTickResult
{
    public List<string> Events { get; } = new();
    public BattleOutcome Outcome { get; set; } = BattleOutcome.InProgress;
}

public sealed class BattleEngine
{
    private readonly EnemyDefinition enemyDefinition;

    public BattleState State { get; } = new();
    public EnemyDefinition EnemyInfo => enemyDefinition;

    public BattleEngine(EnemyDefinition enemyDefinition)
    {
        this.enemyDefinition = enemyDefinition;
    }

    public BattleTickResult Start(DateTime nowUtc)
    {
        State.Player.MaxHp = 120;
        State.Player.MaxMana = 100;
        State.Player.Hp = State.Player.MaxHp;
        State.Player.Mana = State.Player.MaxMana;
        State.Player.HasShield = false;

        State.Enemy.MaxHp = enemyDefinition.MaxHp;
        State.Enemy.MaxMana = enemyDefinition.MaxMana;
        State.Enemy.Hp = enemyDefinition.MaxHp;
        State.Enemy.Mana = enemyDefinition.MaxMana;

        State.EnemyAttackCooldownMs = enemyDefinition.AttackEveryMs;
        State.NextEnemyAttackUtc = nowUtc.AddMilliseconds(enemyDefinition.AttackEveryMs);
        State.EnemySlowStacks = 0;
        State.EnemyActionCounter = 0;
        State.EnemyHasStoneShield = false;
        State.EnemyPreparingBurst = false;
        State.DarkMagusCopiedDamage = 0;
        State.DragonBreathReady = false;
        State.Outcome = BattleOutcome.InProgress;

        var result = new BattleTickResult();
        result.Events.Add($"Battle started vs {enemyDefinition.Name}");
        result.Outcome = State.Outcome;
        return result;
    }

    public BattleTickResult Tick(DateTime nowUtc)
    {
        var result = new BattleTickResult();
        if (State.Outcome != BattleOutcome.InProgress)
        {
            result.Outcome = State.Outcome;
            return result;
        }

        if (nowUtc >= State.NextEnemyAttackUtc)
        {
            ExecuteEnemyAction(result, nowUtc);
        }

        ResolveOutcome(result);
        return result;
    }

    public BattleTickResult ApplySuccessfulCast(SpellDefinition spell, DateTime nowUtc)
    {
        var result = new BattleTickResult();
        if (State.Outcome != BattleOutcome.InProgress)
        {
            result.Outcome = State.Outcome;
            return result;
        }

        State.Player.Mana = Math.Max(0, State.Player.Mana - spell.ManaCost);
        var damage = spell.BasePower;

        if (string.Equals(spell.School, enemyDefinition.VulnerableTo, StringComparison.OrdinalIgnoreCase))
        {
            damage = (int)Math.Round(damage * 1.3);
        }
        else if (string.Equals(spell.School, enemyDefinition.ResistantTo, StringComparison.OrdinalIgnoreCase))
        {
            damage = (int)Math.Round(damage * 0.7);
        }

        if (State.EnemyHasStoneShield && damage > 0)
        {
            damage = (int)Math.Floor(damage * 0.5);
            State.EnemyHasStoneShield = false;
            result.Events.Add("Stone Shield reduced the incoming damage.");
        }

        if (damage > 0)
        {
            State.Enemy.Hp = Math.Max(0, State.Enemy.Hp - damage);
            result.Events.Add($"Cast {spell.Name} for {damage} damage.");
        }
        else
        {
            result.Events.Add($"Cast {spell.Name}.");
        }

        switch (spell.Effect.ToLowerInvariant())
        {
            case "slow":
                State.EnemySlowStacks = Math.Min(3, State.EnemySlowStacks + 1);
                result.Events.Add("Effect: slow applied.");
                break;
            case "interrupt":
                // Old mechanic kept here on purpose:
                // interrupt-spells used to cancel EnemyPreparingBurst directly.
                // We disabled that behavior in favor of the dedicated STOP counter flow,
                // but this branch is intentionally left as a reminder so we can restore it later if needed.
                //
                // if (State.EnemyPreparingBurst)
                // {
                //     State.EnemyPreparingBurst = false;
                //     State.EnemyActionCounter = 0;
                //     State.NextEnemyAttackUtc = nowUtc.AddMilliseconds(enemyDefinition.AttackEveryMs + 400);
                //     result.Events.Add("Effect: interrupt canceled Fire Burst.");
                // }
                // else
                // {
                //     State.NextEnemyAttackUtc = nowUtc.AddMilliseconds(1500);
                //     result.Events.Add("Effect: interrupt enemy cast.");
                // }
                result.Events.Add("Effect: interrupt spell is currently reserved for future design.");
                break;
            case "shield":
                State.Player.HasShield = true;
                result.Events.Add("Effect: shield activated.");
                break;
        }

        if (enemyDefinition.Id == "dark_magus" && damage > 0 && !string.Equals(spell.School, "ward", StringComparison.OrdinalIgnoreCase))
        {
            State.DarkMagusCopiedDamage = Math.Max(State.DarkMagusCopiedDamage, Math.Max(6, damage / 2));
            result.Events.Add($"Dark Magus copied part of {spell.Name}.");
        }

        State.Player.Mana = Math.Min(State.Player.MaxMana, State.Player.Mana + 3);
        ResolveOutcome(result);
        return result;
    }

    public bool CanCast(SpellDefinition spell)
    {
        return State.Outcome == BattleOutcome.InProgress && State.Player.Mana >= spell.ManaCost;
    }

    public BattleTickResult ApplyEmergencyInterrupt(DateTime nowUtc)
    {
        var result = new BattleTickResult();
        if (State.Outcome != BattleOutcome.InProgress)
        {
            result.Outcome = State.Outcome;
            return result;
        }

        if (State.EnemyPreparingBurst)
        {
            State.EnemyPreparingBurst = false;
            State.EnemyActionCounter = 0;
            State.NextEnemyAttackUtc = nowUtc.AddMilliseconds(enemyDefinition.AttackEveryMs + 500);
            result.Events.Add("Emergency interrupt canceled Fire Burst.");
        }
        else
        {
            result.Events.Add("Emergency interrupt had no target.");
        }

        ResolveOutcome(result);
        return result;
    }

    private void ResolveOutcome(BattleTickResult result)
    {
        if (State.Player.Hp <= 0)
        {
            State.Outcome = BattleOutcome.Defeat;
            result.Events.Add("Defeat.");
        }
        else if (State.Enemy.Hp <= 0)
        {
            State.Outcome = BattleOutcome.Victory;
            result.Events.Add("Victory.");
        }

        result.Outcome = State.Outcome;
    }

    private void ExecuteEnemyAction(BattleTickResult result, DateTime nowUtc)
    {
        State.EnemyActionCounter++;

        if (enemyDefinition.Id == "goblin_pyro" && !State.EnemyPreparingBurst && State.EnemyActionCounter % 3 == 0)
        {
            State.EnemyPreparingBurst = true;
            State.NextEnemyAttackUtc = nowUtc.AddMilliseconds(1200);
            result.Events.Add("Goblin Pyromaniac prepares Fire Burst. Interrupt it now.");
            return;
        }

        var attackDamage = enemyDefinition.AttackDamage;
        var attackName = "Enemy attack";

        if (enemyDefinition.Id == "goblin_pyro" && State.EnemyPreparingBurst)
        {
            State.EnemyPreparingBurst = false;
            attackDamage = 22;
            attackName = "Fire Burst";
        }

        if (enemyDefinition.Id == "dark_magus" && State.DarkMagusCopiedDamage > 0)
        {
            attackDamage += State.DarkMagusCopiedDamage;
            attackName = $"Copied strike +{State.DarkMagusCopiedDamage}";
            State.DarkMagusCopiedDamage = 0;
        }

        if (enemyDefinition.Id == "ancient_dragon" && State.EnemyActionCounter % 3 == 0)
        {
            attackDamage = 42;
            attackName = "Inferno Breath";
        }

        if (State.Player.HasShield)
        {
            State.Player.HasShield = false;
            result.Events.Add($"Shield absorbed {attackName}.");
        }
        else
        {
            State.Player.Hp = Math.Max(0, State.Player.Hp - attackDamage);
            result.Events.Add($"{attackName} hits for {attackDamage}.");
        }

        if (enemyDefinition.Id == "ice_golem" && State.EnemyActionCounter % 2 == 0)
        {
            State.EnemyHasStoneShield = true;
            result.Events.Add("Ice Golem activates Stone Shield for the next incoming spell.");
        }

        var cooldown = enemyDefinition.AttackEveryMs + State.EnemySlowStacks * 400;
        State.EnemySlowStacks = Math.Max(0, State.EnemySlowStacks - 1);
        State.NextEnemyAttackUtc = nowUtc.AddMilliseconds(cooldown);
    }
}
