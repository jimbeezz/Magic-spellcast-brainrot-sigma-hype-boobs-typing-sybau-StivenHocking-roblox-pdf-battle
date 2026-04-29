namespace SpellCaster.WinForms.Models;

public sealed class PlayerProgress
{
    public int Wins { get; set; }
    public int Xp { get; set; }

    public int HighestUnlockedTier
    {
        get
        {
            if (Wins >= 20 || Xp >= 1100)
            {
                return 4;
            }

            if (Wins >= 10 || Xp >= 520)
            {
                return 3;
            }

            if (Wins >= 5 || Xp >= 220)
            {
                return 2;
            }

            return 1;
        }
    }
}
