namespace SpellCaster.WinForms.Models;

public enum SpellInputState
{
    Idle = 0,
    InProgress = 1,
    Success = 2,
    FailedWrongChar = 3,
    FailedTimeout = 4
}

public sealed class SpellInputResult
{
    public SpellInputState State { get; init; }
    public int Position { get; init; }
    public char? Expected { get; init; }
    public char? Received { get; init; }
    public int RemainingMs { get; init; }
}

public sealed class SpellInputEngine
{
    private string pattern = string.Empty;
    private int castWindowMs;
    private int position;
    private DateTime startUtc;
    private bool isActive;

    public string Pattern => pattern;
    public int Position => position;
    public bool IsActive => isActive;

    public SpellInputResult Start(string newPattern, int newCastWindowMs, DateTime nowUtc)
    {
        pattern = newPattern ?? string.Empty;
        castWindowMs = Math.Max(1, newCastWindowMs);
        position = 0;
        startUtc = nowUtc;
        isActive = pattern.Length > 0;

        return new SpellInputResult
        {
            State = isActive ? SpellInputState.InProgress : SpellInputState.Idle,
            Position = position,
            RemainingMs = castWindowMs
        };
    }

    public SpellInputResult Tick(DateTime nowUtc)
    {
        if (!isActive)
        {
            return new SpellInputResult
            {
                State = SpellInputState.Idle,
                Position = position,
                RemainingMs = 0
            };
        }

        var remainingMs = GetRemainingMs(nowUtc);
        if (remainingMs <= 0)
        {
            isActive = false;
            return new SpellInputResult
            {
                State = SpellInputState.FailedTimeout,
                Position = position,
                RemainingMs = 0
            };
        }

        return new SpellInputResult
        {
            State = SpellInputState.InProgress,
            Position = position,
            RemainingMs = remainingMs
        };
    }

    public SpellInputResult Input(char typed, DateTime nowUtc)
    {
        if (!isActive)
        {
            return new SpellInputResult
            {
                State = SpellInputState.Idle,
                Position = position,
                Received = typed,
                RemainingMs = 0
            };
        }

        var remainingMs = GetRemainingMs(nowUtc);
        if (remainingMs <= 0)
        {
            isActive = false;
            return new SpellInputResult
            {
                State = SpellInputState.FailedTimeout,
                Position = position,
                Received = typed,
                RemainingMs = 0
            };
        }

        if (position >= pattern.Length)
        {
            isActive = false;
            return new SpellInputResult
            {
                State = SpellInputState.Success,
                Position = position,
                RemainingMs = remainingMs
            };
        }

        var expected = pattern[position];
        if (typed != expected)
        {
            isActive = false;
            return new SpellInputResult
            {
                State = SpellInputState.FailedWrongChar,
                Position = position,
                Expected = expected,
                Received = typed,
                RemainingMs = remainingMs
            };
        }

        position++;
        if (position >= pattern.Length)
        {
            isActive = false;
            return new SpellInputResult
            {
                State = SpellInputState.Success,
                Position = position,
                RemainingMs = remainingMs
            };
        }

        return new SpellInputResult
        {
            State = SpellInputState.InProgress,
            Position = position,
            RemainingMs = remainingMs
        };
    }

    public void Stop()
    {
        isActive = false;
    }

    public void ResetProgress()
    {
        position = 0;
        isActive = false;
    }

    private int GetRemainingMs(DateTime nowUtc)
    {
        var elapsedMs = (int)(nowUtc - startUtc).TotalMilliseconds;
        return Math.Max(0, castWindowMs - elapsedMs);
    }
}
