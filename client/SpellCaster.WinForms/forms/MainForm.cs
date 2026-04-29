using System.Text.Json;
using SpellCaster.WinForms.Models;
using System.Drawing.Drawing2D;

namespace SpellCaster.WinForms.Forms;

public partial class MainForm : Form
{
    private enum BattleEventKind
    {
        Info,
        Warning,
        CounterWindow,
        DefendNow,
        Success,
        Danger
    }

    private static readonly Color AppBackColor = Color.FromArgb(30, 39, 45);
    private static readonly Color SurfaceColor = Color.FromArgb(54, 69, 79);
    private static readonly Color SurfaceAltColor = Color.FromArgb(40, 51, 59);
    private static readonly Color BorderColor = Color.FromArgb(28, 36, 41);
    private static readonly Color AccentColor = Color.FromArgb(44, 57, 66);
    private static readonly Color AccentMutedColor = Color.FromArgb(68, 83, 94);
    private static readonly Color TextColor = Color.FromArgb(245, 222, 179);
    private static readonly Color SubtleTextColor = Color.FromArgb(228, 209, 172);

    private readonly System.Windows.Forms.Timer castTimer = new();
    private readonly SpellInputEngine inputEngine = new();
    private readonly List<SpellDefinition> allSpells = new();
    private readonly List<SpellDefinition> learnedSpells = new();
    private readonly List<SpellDefinition> selectedLoadout = new();
    private readonly List<SpellDefinition> battleSpellPool = new();
    private readonly List<SpellDefinition> examSpellPool = new();
    private readonly List<EnemyDefinition> arenaEnemies = new();
    private readonly List<Button> navButtons = new();
    private readonly Random random = new();
    private PlayerProgress progress = new();

    private BattleEngine battleEngine = new(new EnemyDefinition());
    private EnemyDefinition selectedEnemy = new();
    private Panel arenaPanel = null!;
    private ListBox arenaEnemyList = null!;
    private PictureBox arenaPortraitBox = null!;
    private Label arenaTitleLabel = null!;
    private Label arenaDetailsLabel = null!;
    private Label arenaRequirementLabel = null!;
    private Button arenaChooseButton = null!;
    private FlowLayoutPanel arenaTagPanel = null!;
    private Panel arenaLegendPanel = null!;
    private Label arenaLegendTitleLabel = null!;
    private Label arenaLegendLabel = null!;
    private Button navToggleButton = null!;
    private Panel battleCurrentSpellPanel = null!;
    private Label battleCurrentSpellNameLabel = null!;
    private Label battleNextSpellLabel = null!;
    private Label battleAfterNextSpellLabel = null!;
    private Panel battleNextSpellPanel = null!;
    private Panel battleAfterNextSpellPanel = null!;
    private Panel battleEmergencyPanel = null!;
    private Label battleEmergencyLabel = null!;
    private Image? cachedEnemyPortrait;
    private string? cachedEnemyPortraitKey;
    private SpellDefinition? nextBattleSpell;
    private SpellDefinition? afterNextBattleSpell;
    private SpellDefinition? selectedSpell;
    private string currentPattern = string.Empty;
    private string emergencyInterruptPattern = "stop";
    private int emergencyInterruptPosition;
    private int castWindowMs = 2200;
    private string currentScreen = "MainMenu";
    private DateTime nextAutoCastUtc;
    private DateTime nextSpellSwapUtc;
    private int spellSwapEveryMs = 4500;
    private const int AutoCastDelayMs = 1000;
    private bool battleRunning;
    private bool battleRewardGranted;
    private bool navCollapsed;
    private const int LoadoutMaxSpells = 4;
    private readonly HashSet<int> lastValidLoadoutIndices = new();
    private bool suppressLoadoutSelectionChanged;
    private bool examRunning;
    private bool examStrictMode;
    private DateTime examEndUtc;
    private int examScore;
    private int examCombo;
    private int examMaxCombo;
    private int examAttempts;
    private int examSuccesses;
    private DateTime examStartedUtc;
    private DateTime examCastStartedUtc;
    private DateTime failOverlayHideUtc;
    private DateTime battleEventHideUtc;
    private int examDurationSeconds = 60;
    private int examSpeedBonusTotal;
    private int examDifficultyBonusTotal;
    private int examSuccessfulHardPatterns;
    private int examBestDifficultyBand;

    public MainForm()
    {
        InitializeComponent();
        BuildArenaScreenControls();
        BuildBattleFocusControls();
        ConfigureResponsiveLayout();
        ApplyTheme();
        LoadPlayerProgress();
        LoadSpellsFromAssets();
        LoadEnemiesFromAssets();
        InitializeCastPrototype();
        SetScreen("MainMenu");
    }

    private void InitializeCastPrototype()
    {
        castTimer.Interval = 25;
        castTimer.Tick += CastTimer_Tick;
        timerLabel.Text = "Таймер: -";
        resultLabel.Text = "Результат: -";
        castStateLabel.Text = "Состояние: ожидание";
        resultsDetailsLabel.Text = "Здесь появятся итоги боя.";
        startCastButton.Enabled = true;
        startCastButton.Text = "Начать бой";
        examModeCombo.SelectedIndex = 0;
        ResetExamUi();
    }

    private void BuildArenaScreenControls()
    {
        arenaPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Visible = false
        };

        var headerLabel = new Label
        {
            Text = "Арена",
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            Location = new Point(24, 18),
            Size = new Size(900, 40)
        };

        var hint = new Label
        {
            Text = "Выбери бой. Манекен можно использовать, чтобы спокойно потренироваться без таймера.",
            Location = new Point(26, 66),
            Size = new Size(890, 28)
        };

        arenaEnemyList = new ListBox
        {
            Location = new Point(26, 110),
            Size = new Size(350, 390),
            ItemHeight = 25
        };
        arenaEnemyList.SelectedIndexChanged += ArenaEnemyList_SelectedIndexChanged;

        arenaPortraitBox = new PictureBox
        {
            Location = new Point(410, 112),
            Size = new Size(220, 220),
            SizeMode = PictureBoxSizeMode.Zoom,
            BorderStyle = BorderStyle.FixedSingle
        };

        arenaTitleLabel = new Label
        {
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            Location = new Point(650, 110),
            Size = new Size(280, 40)
        };

        arenaDetailsLabel = new Label
        {
            Location = new Point(652, 162),
            Size = new Size(280, 190)
        };

        arenaRequirementLabel = new Label
        {
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Location = new Point(410, 356),
            Size = new Size(520, 42)
        };

        arenaTagPanel = new FlowLayoutPanel
        {
            Location = new Point(650, 352),
            Size = new Size(282, 52),
            AutoScroll = false,
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight
        };

        arenaChooseButton = new Button
        {
            Text = "Выбрать набор",
            Location = new Point(410, 420),
            Size = new Size(220, 44)
        };
        arenaChooseButton.Click += ArenaChooseButton_Click;

        arenaLegendPanel = new Panel
        {
            Location = new Point(410, 478),
            Size = new Size(522, 76),
            BorderStyle = BorderStyle.FixedSingle
        };

        arenaLegendTitleLabel = new Label
        {
            Text = "Подсказка к бою",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Location = new Point(12, 8),
            Size = new Size(180, 22)
        };

        arenaLegendLabel = new Label
        {
            Text = "парирование = контра опасного каста | shield = блок следующего удара | slow = выиграть время | урон = наказывать в окно",
            Location = new Point(12, 34),
            Size = new Size(494, 34)
        };

        arenaLegendPanel.Controls.Add(arenaLegendTitleLabel);
        arenaLegendPanel.Controls.Add(arenaLegendLabel);

        arenaPanel.Controls.Add(headerLabel);
        arenaPanel.Controls.Add(hint);
        arenaPanel.Controls.Add(arenaEnemyList);
        arenaPanel.Controls.Add(arenaPortraitBox);
        arenaPanel.Controls.Add(arenaTitleLabel);
        arenaPanel.Controls.Add(arenaDetailsLabel);
        arenaPanel.Controls.Add(arenaRequirementLabel);
        arenaPanel.Controls.Add(arenaTagPanel);
        arenaPanel.Controls.Add(arenaChooseButton);
        arenaPanel.Controls.Add(arenaLegendPanel);
        contentPanel.Controls.Add(arenaPanel);
        arenaPanel.BringToFront();
    }

    private void BuildBattleFocusControls()
    {
        navButtons.AddRange(new[] { menuButton, loadoutButton, progressButton, battleButton, examButton, resultsButton });

        navToggleButton = new Button
        {
            Text = "|||",
            Size = new Size(40, 34)
        };
        navToggleButton.Click += NavToggleButton_Click;
        navPanel.Controls.Add(navToggleButton);
        navToggleButton.BringToFront();

        battleCurrentSpellPanel = new Panel
        {
            BorderStyle = BorderStyle.FixedSingle,
            Size = new Size(460, 136)
        };
        battleCurrentSpellNameLabel = new Label
        {
            Font = new Font("Segoe UI", 15F, FontStyle.Bold),
            Location = new Point(18, 14),
            Size = new Size(420, 30),
            Text = "Текущее заклинание"
        };
        battleCurrentSpellPanel.Controls.Add(battleCurrentSpellNameLabel);
        battlePanel.Controls.Add(battleCurrentSpellPanel);
        battleCurrentSpellPanel.BringToFront();

        battleNextSpellPanel = new Panel
        {
            BorderStyle = BorderStyle.FixedSingle,
            Size = new Size(320, 72)
        };
        battleNextSpellLabel = new Label
        {
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            Location = new Point(16, 18),
            Size = new Size(288, 34),
            Text = "Следующее"
        };
        battleNextSpellPanel.Controls.Add(battleNextSpellLabel);
        battlePanel.Controls.Add(battleNextSpellPanel);
        battleNextSpellPanel.BringToFront();

        battleAfterNextSpellPanel = new Panel
        {
            BorderStyle = BorderStyle.FixedSingle,
            Size = new Size(270, 62)
        };
        battleAfterNextSpellLabel = new Label
        {
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Location = new Point(16, 16),
            Size = new Size(238, 28),
            Text = "Потом"
        };
        battleAfterNextSpellPanel.Controls.Add(battleAfterNextSpellLabel);
        battlePanel.Controls.Add(battleAfterNextSpellPanel);
        battleAfterNextSpellPanel.BringToFront();

        battleEmergencyPanel = new Panel
        {
            BorderStyle = BorderStyle.FixedSingle,
            Size = new Size(540, 82),
            Visible = false
        };
        battleEmergencyLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "НАБЕРИ STOP, ЧТОБЫ ПРЕРВАТЬ"
        };
        battleEmergencyPanel.Controls.Add(battleEmergencyLabel);
        battlePanel.Controls.Add(battleEmergencyPanel);
        battleEmergencyPanel.BringToFront();

        battleCurrentSpellPanel.Controls.Add(patternBox);
        battleCurrentSpellPanel.Controls.Add(timerLabel);
        battleCurrentSpellPanel.Controls.Add(resultLabel);
        battleCurrentSpellPanel.Controls.Add(castStateLabel);
        battleCurrentSpellPanel.Controls.Add(startCastButton);
        battleCurrentSpellPanel.Controls.Add(resetCastButton);
    }

    private void ConfigureResponsiveLayout()
    {
        titleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        statusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        navPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        contentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        foreach (var panel in new[] { loadoutPanel, progressPanel, battlePanel, examPanel, resultsPanel, arenaPanel })
        {
            panel.AutoScroll = true;
        }

        menuIntroLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        loadoutHintLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        learnedSpellsList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        learnedSpellsList.Size = new Size(900, 430);
        selectedSpellLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        selectedSpellLabel.Location = new Point(20, 490);
        selectedSpellLabel.Size = new Size(700, 56);
        loadoutStartButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
        loadoutStartButton.Location = new Point(740, 500);
        loadoutStartButton.BringToFront();

        progressTitleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        progressTierLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        progressXpBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        progressStatsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        progressNextLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        battleEventPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        battleEventLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        battleEventTypeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        playerGroup.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        enemyGroup.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        playerHpBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        playerManaBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        enemyHpBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        enemyManaBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        enemyAttackLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        enemyModelPanel.Anchor = AnchorStyles.None;
        castGroup.Visible = false;
        hintLabel.Visible = false;
        patternBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        resultLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        castStateLabel.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
        startCastButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        resetCastButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        combatLogList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        battleCurrentSpellPanel.Anchor = AnchorStyles.Top;
        battleNextSpellPanel.Anchor = AnchorStyles.Top;
        battleAfterNextSpellPanel.Anchor = AnchorStyles.Top;
        battleEmergencyPanel.Anchor = AnchorStyles.Top;

        examHintLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        examSpellList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        examPatternBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        examTimerLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        examResultLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        examStatsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        examStartButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
        examResetButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

        resultsTitleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        resultsDetailsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        arenaEnemyList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        arenaPortraitBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        arenaTitleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        arenaDetailsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        arenaRequirementLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        arenaTagPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        arenaChooseButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        arenaLegendPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        arenaLegendTitleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        arenaLegendLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        Resize += MainForm_Resize;
        ApplyNavigationState();
        UpdateResponsiveScreenLayout();
        ApplyBattleRoundedStyles();
    }

    private void MainForm_Resize(object? sender, EventArgs e)
    {
        UpdateResponsiveScreenLayout();
    }

    private void UpdateResponsiveScreenLayout()
    {
        UpdateNavigationState();
        UpdateArenaResponsiveLayout();
        UpdateLoadoutResponsiveLayout();
        UpdateBattleResponsiveLayout();
        ApplyBattleRoundedStyles();
    }

    private static string TranslateScreenName(string screenName)
    {
        return screenName switch
        {
            "MainMenu" => "главное меню",
            "Loadout" => "набор",
            "Progress" => "прогресс",
            "Arena" => "арена",
            "Battle" => "бой",
            "Exam" => "экзамен",
            "Results" => "итоги",
            _ => screenName
        };
    }

    private void UpdateLoadoutResponsiveLayout()
    {
        var panelWidth = loadoutPanel.ClientSize.Width;
        var panelHeight = loadoutPanel.ClientSize.Height;
        if (panelWidth <= 0 || panelHeight <= 0)
        {
            return;
        }

        var rightButtonWidth = loadoutStartButton.Width;
        var bottomY = Math.Max(500, panelHeight - 62);
        loadoutStartButton.Location = new Point(Math.Max(20, panelWidth - rightButtonWidth - 24), bottomY);
        selectedSpellLabel.Location = new Point(20, bottomY - 6);
        selectedSpellLabel.Size = new Size(Math.Max(260, panelWidth - rightButtonWidth - 72), 48);
        learnedSpellsList.Size = new Size(Math.Max(320, panelWidth - 40), Math.Max(280, bottomY - 74));
    }

    private void UpdateArenaResponsiveLayout()
    {
        var panelWidth = arenaPanel.ClientSize.Width;
        if (panelWidth <= 0)
        {
            return;
        }

        const int leftColumnWidth = 350;
        const int portraitWidth = 220;
        var rightStart = 410;
        var detailsStart = 650;
        var panelRight = Math.Max(930, panelWidth - 24);
        var rightWidth = Math.Max(250, panelRight - rightStart);
        var detailsWidth = Math.Max(220, panelRight - detailsStart);

        arenaEnemyList.Size = new Size(leftColumnWidth, Math.Max(280, arenaPanel.ClientSize.Height - 160));
        arenaPortraitBox.Location = new Point(rightStart, 112);
        arenaPortraitBox.Size = new Size(portraitWidth, 220);
        arenaTitleLabel.Location = new Point(detailsStart, 110);
        arenaTitleLabel.Size = new Size(detailsWidth, 40);
        arenaDetailsLabel.Location = new Point(detailsStart, 162);
        arenaDetailsLabel.Size = new Size(detailsWidth, 190);
        arenaRequirementLabel.Location = new Point(rightStart, 356);
        arenaRequirementLabel.Size = new Size(Math.Min(220, rightWidth), 58);
        arenaTagPanel.Location = new Point(detailsStart, 352);
        arenaTagPanel.Size = new Size(detailsWidth, 62);
        arenaChooseButton.Location = new Point(rightStart, 426);
        arenaLegendPanel.Location = new Point(rightStart, 482);
        arenaLegendPanel.Size = new Size(rightWidth, 76);
        arenaLegendLabel.Size = new Size(Math.Max(220, rightWidth - 28), 36);
    }

    private void UpdateBattleResponsiveLayout()
    {
        var panelWidth = battlePanel.ClientSize.Width;
        if (panelWidth <= 0)
        {
            return;
        }

        playerGroup.Location = new Point(48, 18);
        playerGroup.Size = new Size(410, 118);
        playerHpBar.Location = new Point(14, 42);
        playerHpBar.Size = new Size(360, 14);
        playerManaBar.Location = new Point(14, 86);
        playerManaBar.Size = new Size(360, 12);
        playerHpLabel.Location = new Point(12, 22);
        playerHpLabel.Size = new Size(180, 18);
        playerManaLabel.Location = new Point(12, 64);
        playerManaLabel.Size = new Size(180, 18);
        playerShieldLabel.Location = new Point(240, 22);
        playerShieldLabel.Size = new Size(134, 18);

        enemyGroup.Location = new Point(Math.Max(440, panelWidth - 458), 18);
        enemyGroup.Size = new Size(410, 118);
        enemyHpBar.Location = new Point(14, 42);
        enemyHpBar.Size = new Size(360, 14);
        enemyManaBar.Location = new Point(14, 86);
        enemyManaBar.Size = new Size(360, 12);
        enemyHpLabel.Location = new Point(12, 22);
        enemyHpLabel.Size = new Size(120, 18);
        enemyManaLabel.Location = new Point(12, 64);
        enemyManaLabel.Size = new Size(120, 18);
        enemySlowLabel.Location = new Point(196, 22);
        enemySlowLabel.Size = new Size(76, 18);
        enemyAttackLabel.Location = new Point(272, 22);
        enemyAttackLabel.Size = new Size(102, 18);

        battleEventPanel.Location = new Point(64, 148);
        battleEventPanel.Size = new Size(Math.Max(560, panelWidth - 128), 56);
        battleEventLabel.Size = new Size(Math.Max(420, battleEventPanel.Width - 40), 24);

        var centerX = panelWidth / 2;
        battleCurrentSpellPanel.Location = new Point(centerX - 250, 214);
        battleCurrentSpellPanel.Size = new Size(500, 182);
        battleCurrentSpellNameLabel.Size = new Size(420, 30);
        patternBox.Location = new Point(18, 52);
        patternBox.Size = new Size(464, 42);
        timerLabel.Location = new Point(20, 106);
        timerLabel.Size = new Size(180, 24);
        castStateLabel.Location = new Point(20, 130);
        castStateLabel.Size = new Size(220, 24);
        resultLabel.Location = new Point(20, 154);
        resultLabel.Size = new Size(290, 24);
        startCastButton.Location = new Point(344, 110);
        startCastButton.Size = new Size(138, 30);
        resetCastButton.Location = new Point(344, 146);
        resetCastButton.Size = new Size(138, 30);

        battleNextSpellPanel.Location = new Point(centerX - 170, 414);
        battleNextSpellPanel.Size = new Size(340, 78);
        battleNextSpellLabel.Location = new Point(18, 20);
        battleNextSpellLabel.Size = new Size(304, 34);
        battleAfterNextSpellPanel.Location = new Point(centerX - 145, 504);
        battleAfterNextSpellPanel.Size = new Size(290, 68);
        battleAfterNextSpellLabel.Location = new Point(18, 18);
        battleAfterNextSpellLabel.Size = new Size(254, 28);
        battleEmergencyPanel.Location = new Point(centerX - 270, 150);

        enemyModelPanel.Location = new Point(centerX - 540, 130);
        enemyModelPanel.Size = new Size(220, 220);

        combatLogList.Location = new Point(64, 560);
        combatLogList.Size = new Size(Math.Max(560, panelWidth - 128), 58);
    }

    private void ApplyBattleRoundedStyles()
    {
        ApplyRoundedRegion(battleCurrentSpellPanel, 18);
        ApplyRoundedRegion(battleNextSpellPanel, 22);
        ApplyRoundedRegion(battleAfterNextSpellPanel, 22);
        ApplyRoundedRegion(playerGroup, 18);
        ApplyRoundedRegion(enemyGroup, 18);
        ApplyRoundedRegion(enemyModelPanel, 18);
        ApplyRoundedRegion(playerHpBar, 8);
        ApplyRoundedRegion(playerManaBar, 8);
        ApplyRoundedRegion(enemyHpBar, 8);
        ApplyRoundedRegion(enemyManaBar, 8);
    }

    private static void ApplyRoundedRegion(Control control, int radius)
    {
        if (control.Width <= 0 || control.Height <= 0)
        {
            return;
        }

        var diameter = radius * 2;
        var rect = new Rectangle(0, 0, control.Width, control.Height);
        using var path = new GraphicsPath();
        path.StartFigure();
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        control.Region?.Dispose();
        control.Region = new Region(path);
    }

    private void ApplyNavigationState()
    {
        foreach (var button in navButtons)
        {
            button.Tag ??= button.Text;
        }
    }

    private void UpdateNavigationState()
    {
        var expandedWidth = 180;
        var collapsedWidth = 56;
        var width = navCollapsed ? collapsedWidth : expandedWidth;
        navPanel.Size = new Size(width, Math.Max(400, Height - 96));
        navToggleButton.Location = new Point(8, 12);
        navToggleButton.Text = navCollapsed ? ">>" : "|||";

        var top = 58;
        foreach (var button in navButtons)
        {
            button.Location = new Point(navCollapsed ? 8 : 14, top);
            button.Size = new Size(navCollapsed ? 38 : 148, 42);
            button.Text = navCollapsed
                ? (((string?)button.Tag) ?? button.Text).Substring(0, 1)
                : ((string?)button.Tag) ?? button.Text;
            top += 50;
        }

        contentPanel.Location = new Point(navPanel.Left + navPanel.Width + 10, 82);
        contentPanel.Size = new Size(Math.Max(820, ClientSize.Width - contentPanel.Left - 18), Math.Max(520, ClientSize.Height - 96));
    }

    private void NavToggleButton_Click(object? sender, EventArgs e)
    {
        navCollapsed = !navCollapsed;
        UpdateResponsiveScreenLayout();
    }

    private void ApplyTheme()
    {
        BackColor = AppBackColor;
        ForeColor = TextColor;

        titleLabel.ForeColor = TextColor;
        statusLabel.ForeColor = SubtleTextColor;

        navPanel.BackColor = SurfaceColor;
        navPanel.ForeColor = TextColor;
        contentPanel.BackColor = SurfaceColor;
        contentPanel.ForeColor = TextColor;

        foreach (var panel in new[] { menuPanel, loadoutPanel, progressPanel, arenaPanel, battlePanel, examPanel, resultsPanel })
        {
            panel.BackColor = SurfaceColor;
            panel.ForeColor = TextColor;
        }

        foreach (var group in new[] { playerGroup, enemyGroup, castGroup })
        {
            group.BackColor = SurfaceColor;
            group.ForeColor = TextColor;
        }

        foreach (var button in new[]
        {
            menuButton, loadoutButton, progressButton, battleButton, examButton, resultsButton,
            startCastButton, resetCastButton, examStartButton, examResetButton, rematchButton, resetProgressButton, arenaChooseButton, loadoutStartButton, navToggleButton
        })
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = BorderColor;
            button.FlatAppearance.MouseDownBackColor = AccentMutedColor;
            button.FlatAppearance.MouseOverBackColor = AccentMutedColor;
            button.BackColor = AccentColor;
            button.ForeColor = TextColor;
        }

        foreach (var list in new[] { learnedSpellsList, examSpellList, combatLogList, arenaEnemyList })
        {
            list.BackColor = SurfaceAltColor;
            list.ForeColor = TextColor;
            list.BorderStyle = BorderStyle.FixedSingle;
        }

        foreach (var box in new[] { patternBox, examPatternBox })
        {
            box.BackColor = SurfaceAltColor;
            box.ForeColor = TextColor;
            box.BorderStyle = BorderStyle.FixedSingle;
        }

        enemyModelPanel.BackColor = SurfaceAltColor;
        arenaPortraitBox.BackColor = SurfaceAltColor;
        battleCurrentSpellPanel.BackColor = SurfaceAltColor;
        battleNextSpellPanel.BackColor = SurfaceAltColor;
        battleAfterNextSpellPanel.BackColor = SurfaceAltColor;
        battleEmergencyPanel.BackColor = Color.FromArgb(85, 55, 37);
        battleEmergencyLabel.ForeColor = TextColor;
        battleCurrentSpellNameLabel.ForeColor = TextColor;
        battleNextSpellLabel.ForeColor = TextColor;
        battleAfterNextSpellLabel.ForeColor = TextColor;

        foreach (var bar in new[] { playerHpBar, playerManaBar, enemyHpBar, enemyManaBar })
        {
            bar.Style = ProgressBarStyle.Continuous;
        }

        menuIntroLabel.ForeColor = SubtleTextColor;
        loadoutHintLabel.ForeColor = SubtleTextColor;
        hintLabel.ForeColor = SubtleTextColor;
        examHintLabel.ForeColor = SubtleTextColor;
        resultsDetailsLabel.ForeColor = SubtleTextColor;
        progressTitleLabel.ForeColor = TextColor;
        progressStatsLabel.ForeColor = TextColor;
        progressTierLabel.ForeColor = TextColor;
        progressNextLabel.ForeColor = SubtleTextColor;
        menuIntroLabel.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
        loadoutHintLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        hintLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        examHintLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        examStatsLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        timerLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        examTimerLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        progressTitleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        progressStatsLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
        progressTierLabel.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        progressNextLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        selectedSpellLabel.ForeColor = TextColor;
        resultsTitleLabel.ForeColor = TextColor;
        arenaTitleLabel.ForeColor = TextColor;
        arenaDetailsLabel.ForeColor = SubtleTextColor;
        arenaRequirementLabel.ForeColor = TextColor;
        arenaTagPanel.BackColor = Color.Transparent;
        arenaLegendPanel.BackColor = SurfaceAltColor;
        arenaLegendTitleLabel.ForeColor = TextColor;
        arenaLegendLabel.ForeColor = SubtleTextColor;

        failOverlayPanel.BackColor = Color.FromArgb(220, 31, 23, 20);
        failOverlayLabel.ForeColor = Color.FromArgb(255, 245, 222, 179);
        battleEventTypeLabel.ForeColor = Color.White;
        battleEventLabel.ForeColor = TextColor;
    }

    private static string GetProgressPath()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SpellCasterArena");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, "progress.json");
    }

    private void LoadPlayerProgress()
    {
        try
        {
            var path = GetProgressPath();
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                progress = JsonSerializer.Deserialize<PlayerProgress>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new PlayerProgress();
            }
        }
        catch
        {
            progress = new PlayerProgress();
        }
    }

    private void SavePlayerProgress()
    {
        try
        {
            var json = JsonSerializer.Serialize(progress, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(GetProgressPath(), json);
        }
        catch
        {
        }
    }

    private void UpdateProgressHints()
    {
        var nextGoal = progress.HighestUnlockedTier switch
        {
            1 => "Tier 2: 5 побед или 220 XP",
            2 => "Tier 3: 10 побед или 520 XP",
            3 => "Tier 4: 20 побед или 1100 XP",
            _ => "Все tier-ы открыты"
        };

        loadoutHintLabel.Text =
            $"Выбери до {LoadoutMaxSpells} заклинаний в набор. " +
            "парирование = контра, shield = защита, slow = контроль темпа. " +
            $"Прогресс: {progress.Wins} побед, {progress.Xp} XP, открыт Tier {progress.HighestUnlockedTier}. " +
            nextGoal;
        UpdateProgressPanel();
    }

    private void GrantVictoryReward()
    {
        if (battleEngine.EnemyInfo.IsTraining)
        {
            AddLog("Тренировка завершена: без XP и без прогресса побед.");
            return;
        }

        var previousTier = progress.HighestUnlockedTier;
        progress.Wins++;
        progress.Xp += Math.Max(0, battleEngine.EnemyInfo.RewardXp);
        SavePlayerProgress();

        var newTier = progress.HighestUnlockedTier;
        if (newTier > previousTier)
        {
            AddLog($"Прогресс: открыт Tier {newTier}.");
        }
        AddLog($"Прогресс: +{battleEngine.EnemyInfo.RewardXp} XP, побед={progress.Wins}, всего XP={progress.Xp}.");
        RefreshUnlockedSpells();
        UpdateProgressPanel();
        RefreshArenaList();
    }

    private void RefreshUnlockedSpells()
    {
        var selectedNames = selectedLoadout.Select(spell => spell.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        learnedSpells.Clear();
        learnedSpells.AddRange(allSpells.Where(spell => spell.Tier <= progress.HighestUnlockedTier));

        selectedLoadout.RemoveAll(spell => spell.Tier > progress.HighestUnlockedTier);
        learnedSpellsList.Items.Clear();
        lastValidLoadoutIndices.Clear();

        foreach (var spell in learnedSpells)
        {
            learnedSpellsList.Items.Add(spell);
        }

        suppressLoadoutSelectionChanged = true;
        for (var i = 0; i < learnedSpellsList.Items.Count; i++)
        {
            if (learnedSpellsList.Items[i] is SpellDefinition spell && selectedNames.Contains(spell.Name))
            {
                learnedSpellsList.SetSelected(i, true);
                lastValidLoadoutIndices.Add(i);
            }
        }
        suppressLoadoutSelectionChanged = false;

        UpdateLoadoutSummary();
        UpdateProgressHints();
    }

    private void ResetProgressToNewPlayer()
    {
        progress = new PlayerProgress();
        SavePlayerProgress();
        selectedLoadout.Clear();
        RefreshUnlockedSpells();
        resultsDetailsLabel.Text = "Прогресс сброшен. Доступны только простые заклинания Tier 1.";
        UpdateProgressPanel();
        RefreshArenaList();
    }

    private void LoadSpellsFromAssets()
    {
        allSpells.Clear();
        learnedSpells.Clear();
        learnedSpellsList.Items.Clear();

        try
        {
            var assetPath = Path.Combine(AppContext.BaseDirectory, "assets", "spells.json");
            if (File.Exists(assetPath))
            {
                var json = File.ReadAllText(assetPath);
                var loaded = JsonSerializer.Deserialize<List<SpellDefinition>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loaded is not null)
                {
                    allSpells.AddRange(loaded.Where(s => !string.IsNullOrWhiteSpace(s.Name) && !string.IsNullOrWhiteSpace(s.Pattern)));
                }
            }
        }
        catch
        {
        }

        if (allSpells.Count == 0)
        {
            allSpells.Add(new SpellDefinition { Name = "Flint", Pattern = "f", CastWindowMs = 2200, School = "fire", BasePower = 8, ManaCost = 4, Tier = 1 });
            allSpells.Add(new SpellDefinition { Name = "Mist", Pattern = "m", CastWindowMs = 2200, School = "ice", BasePower = 8, ManaCost = 4, Tier = 1 });
            allSpells.Add(new SpellDefinition { Name = "Spark", Pattern = "s", CastWindowMs = 2200, School = "arcane", BasePower = 8, ManaCost = 4, Tier = 1 });
        }

        learnedSpells.AddRange(allSpells.Where(spell => spell.Tier <= progress.HighestUnlockedTier));

        foreach (var spell in learnedSpells)
        {
            learnedSpellsList.Items.Add(spell);
        }

        // Default loadout to first spells so battle can start immediately.
        selectedLoadout.Clear();
        selectedLoadout.AddRange(learnedSpells.Take(Math.Min(LoadoutMaxSpells, learnedSpells.Count)));
        for (var i = 0; i < Math.Min(LoadoutMaxSpells, learnedSpellsList.Items.Count); i++)
        {
            learnedSpellsList.SetSelected(i, true);
            lastValidLoadoutIndices.Add(i);
        }
        UpdateLoadoutSummary();
        UpdateProgressHints();
    }

    private void LoadEnemiesFromAssets()
    {
        arenaEnemies.Clear();
        try
        {
            var assetPath = Path.Combine(AppContext.BaseDirectory, "assets", "enemies.json");
            if (File.Exists(assetPath))
            {
                var json = File.ReadAllText(assetPath);
                var loaded = JsonSerializer.Deserialize<List<EnemyDefinition>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loaded is { Count: > 0 })
                {
                    arenaEnemies.AddRange(loaded.OrderBy(enemy => enemy.Order));
                }
            }
        }
        catch
        {
        }

        if (arenaEnemies.Count == 0)
        {
            arenaEnemies.Add(new EnemyDefinition());
        }

        selectedEnemy = arenaEnemies[0];
        battleEngine = new BattleEngine(selectedEnemy);
        RefreshArenaList();
    }

    private void RefreshArenaList()
    {
        if (arenaEnemyList is null)
        {
            return;
        }

        var previousId = selectedEnemy.Id;
        arenaEnemyList.Items.Clear();
        foreach (var enemy in arenaEnemies)
        {
            var lockMark = IsEnemyUnlocked(enemy) ? "" : " [locked]";
            var bossMark = string.Equals(enemy.Type, "boss", StringComparison.OrdinalIgnoreCase) ? " BOSS" : "";
            arenaEnemyList.Items.Add($"{enemy}{bossMark}{lockMark}");
        }

        var selectedIndex = Math.Max(0, arenaEnemies.FindIndex(enemy => enemy.Id == previousId));
        if (selectedIndex >= arenaEnemyList.Items.Count)
        {
            selectedIndex = 0;
        }

        if (arenaEnemyList.Items.Count > 0)
        {
            arenaEnemyList.SelectedIndex = selectedIndex;
        }
        else
        {
            UpdateArenaDetails();
        }
    }

    private bool IsEnemyUnlocked(EnemyDefinition enemy)
    {
        return progress.Wins >= enemy.RequiredWins && progress.HighestUnlockedTier >= enemy.RequiredTier;
    }

    private void UpdateArenaDetails()
    {
        arenaTitleLabel.Text = selectedEnemy.Name;
        arenaDetailsLabel.Text =
            $"Тип: {selectedEnemy.Type}\r\n" +
            $"HP: {selectedEnemy.MaxHp}\r\n" +
            $"Урон: {selectedEnemy.AttackDamage}\r\n" +
            $"Атака: {selectedEnemy.AttackEveryMs / 1000.0:F1}с\r\n" +
            $"Слабость: {selectedEnemy.VulnerableTo}\r\n" +
            $"Сопротивление: {selectedEnemy.ResistantTo}\r\n" +
            $"Награда: {selectedEnemy.RewardXp} XP\r\n\r\n" +
            selectedEnemy.Trait;

        arenaRequirementLabel.Text = IsEnemyUnlocked(selectedEnemy)
            ? selectedEnemy.IsTraining
                ? "Режим тренировки: без таймера ввода, без награды XP."
                : "Открыто. Выбери набор и начни бой."
            : $"Закрыто: нужно {selectedEnemy.RequiredWins} побед и Tier {selectedEnemy.RequiredTier}.";
        arenaRequirementLabel.ForeColor = IsEnemyUnlocked(selectedEnemy) ? TextColor : Color.FromArgb(255, 180, 115);
        arenaChooseButton.Enabled = IsEnemyUnlocked(selectedEnemy);
        PopulateArenaTags(selectedEnemy);
        arenaLegendLabel.Text = BuildArenaLegendText(selectedEnemy);
        LoadArenaPortrait(selectedEnemy);
        EnsureEnemyPortraitLoaded(selectedEnemy);
    }

    private void LoadArenaPortrait(EnemyDefinition enemy)
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "assets", enemy.Portrait);
            if (File.Exists(path))
            {
                var previous = arenaPortraitBox.Image;
                arenaPortraitBox.Image = Image.FromFile(path);
                previous?.Dispose();
                return;
            }
        }
        catch
        {
        }

        var oldImage = arenaPortraitBox.Image;
        arenaPortraitBox.Image = null;
        oldImage?.Dispose();
    }

    private void EnsureEnemyPortraitLoaded(EnemyDefinition enemy)
    {
        var portraitKey = enemy.Portrait ?? string.Empty;
        if (string.Equals(cachedEnemyPortraitKey, portraitKey, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        cachedEnemyPortrait?.Dispose();
        cachedEnemyPortrait = null;
        cachedEnemyPortraitKey = portraitKey;

        try
        {
            var portraitPath = Path.Combine(AppContext.BaseDirectory, "assets", portraitKey);
            if (File.Exists(portraitPath))
            {
                using var stream = File.OpenRead(portraitPath);
                cachedEnemyPortrait = Image.FromStream(stream);
            }
        }
        catch
        {
            cachedEnemyPortrait?.Dispose();
            cachedEnemyPortrait = null;
        }
    }

    private void ArenaEnemyList_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var index = arenaEnemyList.SelectedIndex;
        if (index < 0 || index >= arenaEnemies.Count)
        {
            return;
        }

        selectedEnemy = arenaEnemies[index];
        EnsureEnemyPortraitLoaded(selectedEnemy);
        UpdateArenaDetails();
    }

    private void ArenaChooseButton_Click(object? sender, EventArgs e)
    {
        if (!IsEnemyUnlocked(selectedEnemy))
        {
            return;
        }

        battleEngine = new BattleEngine(selectedEnemy);
        SetScreen("Loadout");
    }

    private void SetScreen(string screenName)
    {
        currentScreen = screenName;
        if (screenName != "Battle" && screenName != "Exam")
        {
            failOverlayPanel.Visible = false;
        }
        if (screenName != "Battle")
        {
            battleEventPanel.Visible = false;
        }

        menuPanel.Visible = screenName == "MainMenu";
        loadoutPanel.Visible = screenName == "Loadout";
        progressPanel.Visible = screenName == "Progress";
        arenaPanel.Visible = screenName == "Arena";
        battlePanel.Visible = screenName == "Battle";
        examPanel.Visible = screenName == "Exam";
        resultsPanel.Visible = screenName == "Results";
        statusLabel.Text = $"Экран: {TranslateScreenName(screenName)}";

        menuButton.Enabled = screenName != "MainMenu";
        loadoutButton.Enabled = screenName != "Loadout";
        progressButton.Enabled = screenName != "Progress";
        battleButton.Enabled = screenName != "Arena";
        examButton.Enabled = screenName != "Exam";
        resultsButton.Enabled = screenName != "Results";

        if (screenName == "Arena")
        {
            RefreshArenaList();
            StopCast();
            return;
        }

        if (screenName == "Battle")
        {
            PrepareBattleScreen();
            FocusCapture();
            return;
        }

        if (screenName == "Exam")
        {
            PrepareExamScreen();
            FocusCapture();
            return;
        }

        if (screenName == "Progress")
        {
            UpdateProgressPanel();
            StopCast();
            return;
        }

        StopCast();
    }

    private void StartBattleSession()
    {
        if (selectedLoadout.Count == 0)
        {
            selectedLoadout.AddRange(learnedSpells.Take(Math.Min(LoadoutMaxSpells, learnedSpells.Count)));
        }

        var battleStart = battleEngine.Start(DateTime.UtcNow);
        combatLogList.Items.Clear();
        AddLogEvents(battleStart.Events);

        if (battleEngine.EnemyInfo.IsTraining)
        {
            AddLog("Манекен: без таймера ввода и без награды XP.");
            ShowBattleEvent("МАНЕКЕН: безопасная тренировка без таймера.", BattleEventKind.Info, 2600);
        }
        else
        {
            AddLog($"Правило врага: атака каждые {battleEngine.EnemyInfo.AttackEveryMs / 1000.0:F1}с на {battleEngine.EnemyInfo.AttackDamage} урона.");
            AddLog("Shield блокирует один удар врага. Slow добавляет +0.4с к следующей атаке врага.");
            ShowBattleEvent(BuildEnemyStartBanner(), BattleEventKind.Info, 2400);
        }
        AddLog("Автокаст: следующее заклинание начинается через 1.0с после текущего.");

        RollBattleSpellPool();

        resultLabel.Text = "Результат: -";
        resultLabel.ForeColor = SystemColors.ControlText;
        castStateLabel.Text = "Состояние: ожидание автокаста";
        nextAutoCastUtc = DateTime.UtcNow.AddMilliseconds(AutoCastDelayMs);
        resetCastButton.Enabled = true;
        battleRunning = true;
        battleRewardGranted = false;
        startCastButton.Text = "Бой идет";
        startCastButton.Enabled = false;
        castTimer.Start();
        UpdateBattleUi();
        UpdateCastReadinessIndicator(DateTime.UtcNow);
    }

    private void PrepareBattleScreen()
    {
        StopCast();
        battleRunning = false;
        startCastButton.Text = "Начать бой";
        startCastButton.Enabled = true;
        resetCastButton.Enabled = false;
        resultLabel.Text = "Результат: нажми Начать бой";
        resultLabel.ForeColor = SystemColors.ControlText;
        castStateLabel.Text = "Состояние: готов к старту";
        castStateLabel.ForeColor = SystemColors.ControlText;
        combatLogList.Items.Clear();
        enemyGroup.Text = $"Враг - {selectedEnemy.Name}";
        battleEventPanel.Visible = false;
        battleEmergencyPanel.Visible = false;
        emergencyInterruptPosition = 0;
        EnsureEnemyPortraitLoaded(selectedEnemy);
        enemyModelPanel.Invalidate();
    }

    private void RollBattleSpellPool()
    {
        battleSpellPool.Clear();
        if (selectedLoadout.Count == 0)
        {
            return;
        }

        battleSpellPool.AddRange(selectedLoadout.OrderBy(_ => random.Next()));
        spellSwapEveryMs = random.Next(3500, 6001);
        PrimeBattleSpellQueue("Стартовое заклинание");
        AddLog($"Набор на этот бой: {string.Join(", ", battleSpellPool.Select(s => s.Name))}");
        AddLog($"Автосмена заклинаний каждые {spellSwapEveryMs / 1000.0:F1}с.");
    }

    private void StartCast()
    {
        if (currentScreen != "Battle" || selectedSpell is null || string.IsNullOrEmpty(currentPattern))
        {
            return;
        }

        if (!battleRunning || battleEngine.State.Outcome != BattleOutcome.InProgress || inputEngine.IsActive || DateTime.UtcNow < nextAutoCastUtc)
        {
            return;
        }

        if (!battleEngine.CanCast(selectedSpell))
        {
            resultLabel.Text = "Результат: ОШИБКА - не хватает маны";
            resultLabel.ForeColor = Color.DarkRed;
            castStateLabel.Text = "Состояние: смена спелла (мана)";
            SelectRandomBattleSpell(forceDifferent: false, reason: "Смена из-за маны");
            nextAutoCastUtc = DateTime.UtcNow.AddMilliseconds(AutoCastDelayMs);
            return;
        }

        var effectiveCastWindowMs = selectedEnemy.IsTraining ? int.MaxValue : castWindowMs;
        inputEngine.Start(currentPattern, effectiveCastWindowMs, DateTime.UtcNow);
        resultLabel.Text = "Результат: набор...";
        resultLabel.ForeColor = SystemColors.ControlText;
        castStateLabel.Text = selectedEnemy.IsTraining
            ? $"Состояние: тренировка {selectedSpell.Name}"
            : $"Состояние: набор {selectedSpell.Name}";
        UpdatePatternDisplay();
        FocusCapture();
    }

    private void StopCast()
    {
        inputEngine.Stop();
        if (!examRunning)
        {
            castTimer.Stop();
        }
        timerLabel.Text = "Timer: -";
        if (currentScreen != "Battle")
        {
            castStateLabel.Text = "State: idle";
        }

        UpdatePatternDisplay();
    }

    private void FailCast(string reason, bool showOverlay)
    {
        inputEngine.Stop();
        var isTimeout = string.Equals(reason, "timeout", StringComparison.OrdinalIgnoreCase);
        resultLabel.Text = isTimeout ? "Результат: ПРОМАХ - время вышло" : $"Результат: ОШИБКА - {reason}";
        resultLabel.ForeColor = Color.DarkRed;
        castStateLabel.Text = isTimeout ? "Состояние: промах" : "Состояние: ошибка";
        castStateLabel.ForeColor = Color.IndianRed;
        if (showOverlay)
        {
            ShowFailOverlay("ОШИБКА");
        }
        nextAutoCastUtc = DateTime.UtcNow.AddMilliseconds(AutoCastDelayMs);
        SelectRandomBattleSpell(forceDifferent: true, reason: "После ошибки");
    }

    private void SucceedCast()
    {
        inputEngine.Stop();
        resultLabel.Text = "Результат: УСПЕХ";
        resultLabel.ForeColor = Color.DarkGreen;
        castStateLabel.Text = "Состояние: успех";
        castStateLabel.ForeColor = Color.DarkGreen;
        ApplySpellSuccess();
        nextAutoCastUtc = DateTime.UtcNow.AddMilliseconds(AutoCastDelayMs);
        SelectRandomBattleSpell(forceDifferent: true, reason: "После успешного каста");
    }

    private void ApplySpellSuccess()
    {
        if (selectedSpell is null)
        {
            return;
        }

        var beforeEnemyHp = battleEngine.State.Enemy.Hp;
        var beforePlayerHp = battleEngine.State.Player.Hp;
        var battleResult = battleEngine.ApplySuccessfulCast(selectedSpell, DateTime.UtcNow);
        AddLogEvents(battleResult.Events);

        var enemyDamage = Math.Max(0, beforeEnemyHp - battleEngine.State.Enemy.Hp);
        if (enemyDamage > 0)
        {
            resultLabel.Text = $"Результат: УСПЕХ - нанесено {enemyDamage} урона";
            resultLabel.ForeColor = Color.DarkGreen;
        }

        var playerDamage = Math.Max(0, beforePlayerHp - battleEngine.State.Player.Hp);
        if (playerDamage > 0)
        {
            AddLog($"Ответное действие врага нанесло игроку {playerDamage} урона.");
        }

        UpdateBattleUi();
        ShowSelectedSpellContext();
        CheckBattleEnd();
    }

    private void CastTimer_Tick(object? sender, EventArgs e)
    {
        if (examRunning)
        {
            TickExam(DateTime.UtcNow);
            return;
        }

        var nowUtc = DateTime.UtcNow;
        var beforeHp = battleEngine.State.Player.Hp;
        var battleTick = battleEngine.Tick(nowUtc);
        AddLogEvents(battleTick.Events);

        var afterHp = battleEngine.State.Player.Hp;
        if (afterHp < beforeHp)
        {
            var amount = beforeHp - afterHp;
            resultLabel.Text = $"Результат: УДАР ВРАГА -{amount} HP";
            resultLabel.ForeColor = Color.DarkRed;
        }

        TryRotateSpellByTimer(nowUtc);
        StartCast();

        var inputState = inputEngine.Tick(nowUtc);
        timerLabel.Text = inputEngine.IsActive
            ? selectedEnemy.IsTraining ? "Таймер: без ограничения" : $"Таймер: {inputState.RemainingMs} мс"
            : $"Таймер: следующий автокаст через {Math.Max(0, (int)(nextAutoCastUtc - nowUtc).TotalMilliseconds)} мс";

        if (inputState.State == SpellInputState.FailedTimeout)
        {
            FailCast("timeout", showOverlay: false);
        }
        else
        {
            UpdateCastReadinessIndicator(nowUtc);
        }

        UpdateBattleUi();
        CheckBattleEnd();
        UpdateFailOverlay(nowUtc);
        UpdateBattleEvent(nowUtc);
    }

    private void ShowFailOverlay(string text)
    {
        failOverlayLabel.Text = text;
        failOverlayPanel.Visible = true;
        failOverlayPanel.BringToFront();
        failOverlayHideUtc = DateTime.UtcNow.AddMilliseconds(420);
    }

    private void UpdateFailOverlay(DateTime nowUtc)
    {
        if (failOverlayPanel.Visible && nowUtc >= failOverlayHideUtc)
        {
            failOverlayPanel.Visible = false;
        }
    }

    private void ShowBattleEvent(string text, BattleEventKind kind, int durationMs = 1800)
    {
        var style = ResolveBattleEventStyle(kind);
        battleEventTypeLabel.Text = style.Title;
        battleEventLabel.Text = text;
        battleEventPanel.BackColor = style.BackColor;
        battleEventAccentPanel.BackColor = style.AccentColor;
        battleEventPanel.Visible = true;
        battleEventPanel.BringToFront();
        battleEventHideUtc = DateTime.UtcNow.AddMilliseconds(durationMs);
    }

    private void UpdateBattleEvent(DateTime nowUtc)
    {
        if (battleEventPanel.Visible && nowUtc >= battleEventHideUtc)
        {
            battleEventPanel.Visible = false;
        }
    }

    private void PrepareExamScreen()
    {
        StopCast();
        BuildExamSpellPool();
        examSpellList.Items.Clear();
        foreach (var spell in examSpellPool)
        {
            examSpellList.Items.Add($"{spell.Name}  [{GetDifficultyLabel(spell)}]  {spell.Pattern}");
        }

        ResetExamUi();
        FocusCapture();
    }

    private void ResetExamUi()
    {
        examRunning = false;
        inputEngine.Stop();
        examPatternBox.Clear();
        examTimerLabel.Text = "Осталось времени: -";
        examResultLabel.Text = "Результат: -";
        examResultLabel.ForeColor = SystemColors.ControlText;
        examScore = 0;
        examCombo = 0;
        examMaxCombo = 0;
        examAttempts = 0;
        examSuccesses = 0;
        examSpeedBonusTotal = 0;
        examDifficultyBonusTotal = 0;
        examSuccessfulHardPatterns = 0;
        examBestDifficultyBand = 0;
        examCastStartedUtc = DateTime.UtcNow;
        UpdateExamStats();
    }

    private void StartExam()
    {
        if (learnedSpells.Count == 0)
        {
            return;
        }

        examStrictMode = examModeCombo.SelectedIndex == 2;
        examDurationSeconds = examModeCombo.SelectedIndex == 1 ? 90 : 60;
        examRunning = true;
        examStartedUtc = DateTime.UtcNow;
        examEndUtc = examStartedUtc.AddSeconds(examDurationSeconds);
        examScore = 0;
        examCombo = 0;
        examMaxCombo = 0;
        examAttempts = 0;
        examSuccesses = 0;
        examSpeedBonusTotal = 0;
        examDifficultyBonusTotal = 0;
        examSuccessfulHardPatterns = 0;
        examBestDifficultyBand = 0;
        examResultLabel.Text = "Результат: экзамен начался";
        examResultLabel.ForeColor = Color.DarkGreen;
        examTimerLabel.Text = $"Осталось времени: {examDurationSeconds:0.0}с";
        UpdateExamStats();
        SelectNextExamSpell();
        castTimer.Start();
        FocusCapture();
    }

    private void TickExam(DateTime nowUtc)
    {
        var remaining = Math.Max(0, (int)(examEndUtc - nowUtc).TotalMilliseconds);
        examTimerLabel.Text = $"Осталось времени: {remaining / 1000.0:F1}с";

        if (remaining <= 0)
        {
            FinishExam("время вышло");
            return;
        }

        UpdateFailOverlay(nowUtc);
    }

    private void SelectNextExamSpell()
    {
        if (examSpellPool.Count == 0)
        {
            BuildExamSpellPool();
        }

        var desiredBand = ResolveExamBand();
        var candidates = examSpellPool.Where(spell => GetDifficultyBand(spell) <= desiredBand).ToList();
        if (candidates.Count == 0)
        {
            candidates = examSpellPool;
        }

        var spell = candidates[random.Next(candidates.Count)];
        selectedSpell = spell;
        currentPattern = spell.Pattern;
        var nowUtc = DateTime.UtcNow;
        examCastStartedUtc = nowUtc;
        inputEngine.Start(currentPattern, int.MaxValue, nowUtc);
        examResultLabel.Text = $"Результат: цель {spell.Name} ({GetDifficultyLabel(spell)})";
        examResultLabel.ForeColor = Color.SteelBlue;
        UpdateExamPatternDisplay();
        HighlightExamSpell(spell);
    }

    private void UpdateExamPatternDisplay()
    {
        examPatternBox.Clear();
        if (string.IsNullOrEmpty(currentPattern))
        {
            return;
        }

        var index = Math.Clamp(inputEngine.Position, 0, currentPattern.Length);
        var okPrefix = currentPattern[..index];
        var rest = currentPattern[index..];

        if (okPrefix.Length > 0)
        {
            examPatternBox.SelectionColor = Color.DarkGreen;
            examPatternBox.AppendText(okPrefix);
        }

        if (rest.Length > 0)
        {
            examPatternBox.SelectionColor = SystemColors.ControlText;
            examPatternBox.AppendText(rest);
        }
    }

    private void HandleExamSuccess()
    {
        examAttempts++;
        examSuccesses++;
        examCombo++;
        examMaxCombo = Math.Max(examMaxCombo, examCombo);

        if (selectedSpell is not null)
        {
            var difficultyBand = GetDifficultyBand(selectedSpell);
            examBestDifficultyBand = Math.Max(examBestDifficultyBand, difficultyBand);
            examDifficultyBonusTotal += GetDifficultyBonus(selectedSpell);
            if (difficultyBand >= 3)
            {
                examSuccessfulHardPatterns++;
            }

            var castWindowMs = ResolveExamCastWindowMs(selectedSpell);
            var elapsedMs = Math.Max(1, (int)(DateTime.UtcNow - examCastStartedUtc).TotalMilliseconds);
            var castTimePercent = elapsedMs * 100 / castWindowMs;
            examSpeedBonusTotal += Math.Max(0, 35 - castTimePercent);
        }

        RecalculateExamScore();
        examResultLabel.Text = $"Результат: УСПЕХ ({selectedSpell?.Name})";
        examResultLabel.ForeColor = Color.DarkGreen;
        UpdateExamStats();
        SelectNextExamSpell();
    }

    private void HandleExamFailure(string reason)
    {
        examAttempts++;
        var isTimeout = string.Equals(reason, "timeout", StringComparison.OrdinalIgnoreCase);
        examResultLabel.Text = isTimeout ? "Результат: ПРОМАХ - время вышло" : $"Результат: ОШИБКА - {reason}";
        examResultLabel.ForeColor = Color.DarkRed;
        if (!isTimeout)
        {
            ShowFailOverlay("ОШИБКА");
        }

        if (examStrictMode)
        {
            FinishExam("строгий режим: ошибка");
            return;
        }

        examCombo = 0;
        RecalculateExamScore();
        UpdateExamStats();
        SelectNextExamSpell();
    }

    private void FinishExam(string reason)
    {
        examRunning = false;
        inputEngine.Stop();
        castTimer.Stop();
        examTimerLabel.Text = "Осталось времени: 0.0с";
        examResultLabel.Text = $"Результат: завершено ({reason})";
        examResultLabel.ForeColor = Color.DarkBlue;
        RecalculateExamScore();
        UpdateExamStats();
        failOverlayPanel.Visible = false;
    }

    private void UpdateExamStats()
    {
        var accuracy = examAttempts == 0 ? 0 : examSuccesses * 100.0 / examAttempts;
        var elapsed = examRunning ? DateTime.UtcNow - examStartedUtc : TimeSpan.FromSeconds(examDurationSeconds);
        var minutes = Math.Max(0.01, elapsed.TotalMinutes);
        var cpm = (int)Math.Round(examSuccesses / minutes);
        var streakBonus = examMaxCombo * 25;
        examStatsLabel.Text =
            $"Счёт: {examScore}\r\n" +
            $"Серия: {examCombo} (макс {examMaxCombo}) | Бонус серии: {streakBonus}\r\n" +
            $"Точность: {accuracy:F1}% ({examSuccesses}/{examAttempts})\r\n" +
            $"CPM: {cpm} | Сложных паттернов: {examSuccessfulHardPatterns}\r\n" +
            $"Бонус скорости: {examSpeedBonusTotal} | Бонус сложности: {examDifficultyBonusTotal}\r\n" +
            $"Лучшая сложность: {GetDifficultyBandLabel(examBestDifficultyBand)} | Режим: {GetExamModeLabel()}";
    }

    private void UpdateCastReadinessIndicator(DateTime nowUtc)
    {
        if (!battleRunning || battleEngine.State.Outcome != BattleOutcome.InProgress)
        {
            return;
        }

        if (inputEngine.IsActive)
        {
            castStateLabel.Text = "Состояние: идет набор";
            castStateLabel.ForeColor = Color.DeepSkyBlue;
            return;
        }

        var remainingMs = (int)Math.Max(0, (nextAutoCastUtc - nowUtc).TotalMilliseconds);
        if (remainingMs > 0)
        {
            castStateLabel.Text = $"Состояние: окно подготовки {remainingMs} мс";
            castStateLabel.ForeColor = Color.Goldenrod;
            return;
        }

        castStateLabel.Text = "Состояние: готов к следующему касту";
        castStateLabel.ForeColor = Color.DarkGreen;
    }

    private void TryRotateSpellByTimer(DateTime nowUtc)
    {
        if (battleSpellPool.Count <= 1 || nowUtc < nextSpellSwapUtc || inputEngine.IsActive)
        {
            return;
        }

        SelectRandomBattleSpell(forceDifferent: true, reason: "Timed rotation");
    }

    private void SelectRandomBattleSpell(bool forceDifferent, string reason)
    {
        if (battleSpellPool.Count == 0)
        {
            selectedSpell = null;
            nextBattleSpell = null;
            afterNextBattleSpell = null;
            currentPattern = string.Empty;
            inputEngine.ResetProgress();
            battleCurrentSpellNameLabel.Text = "Нет заклинания";
            battleNextSpellLabel.Text = "Следующее: -";
            battleAfterNextSpellLabel.Text = "Потом: -";
            return;
        }

        if (selectedSpell is null || nextBattleSpell is null)
        {
            PrimeBattleSpellQueue(reason);
            return;
        }

        var next = nextBattleSpell;
        nextBattleSpell = afterNextBattleSpell ?? DrawRandomBattleSpell(next);
        afterNextBattleSpell = DrawRandomBattleSpell(nextBattleSpell, next);
        ApplySelectedSpell(next);
        nextSpellSwapUtc = DateTime.UtcNow.AddMilliseconds(spellSwapEveryMs);
        AddLog($"{reason}: {next.Name} [{next.Pattern}]");
        UpdateBattlePreviewUi();
        ShowSelectedSpellContext();
    }

    private void ApplySelectedSpell(SpellDefinition selected)
    {
        selectedSpell = selected;
        currentPattern = selected.Pattern;
        castWindowMs = ResolveCastWindow(currentPattern, selected.CastWindowMs);
        inputEngine.ResetProgress();
        battleCurrentSpellNameLabel.Text = $"{selected.Name}  |  {selected.Effect}  |  мана {selected.ManaCost}";
        castStateLabel.Text = "Состояние: готов";
        UpdatePatternDisplay();
        UpdateBattlePreviewUi();
    }

    private void PrimeBattleSpellQueue(string reason)
    {
        var current = DrawRandomBattleSpell();
        if (current is null)
        {
            return;
        }

        nextBattleSpell = DrawRandomBattleSpell(current);
        afterNextBattleSpell = DrawRandomBattleSpell(nextBattleSpell, current);
        ApplySelectedSpell(current);
        nextSpellSwapUtc = DateTime.UtcNow.AddMilliseconds(spellSwapEveryMs);
        AddLog($"{reason}: {current.Name} [{current.Pattern}]");
        UpdateBattlePreviewUi();
    }

    private SpellDefinition? DrawRandomBattleSpell(params SpellDefinition?[] avoid)
    {
        if (battleSpellPool.Count == 0)
        {
            return null;
        }

        var avoided = avoid.Where(spell => spell is not null).Select(spell => spell!.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var candidates = battleSpellPool.Where(spell => !avoided.Contains(spell.Name)).ToList();
        if (candidates.Count == 0)
        {
            candidates = battleSpellPool;
        }

        return candidates[random.Next(candidates.Count)];
    }

    private void UpdateBattlePreviewUi()
    {
        battleNextSpellLabel.Text = nextBattleSpell is null
            ? "Следующее: -"
            : $"Следующее: {nextBattleSpell.Name}  [{nextBattleSpell.Pattern}]";
        battleAfterNextSpellLabel.Text = afterNextBattleSpell is null
            ? "Потом: -"
            : $"Потом: {afterNextBattleSpell.Name}  [{afterNextBattleSpell.Pattern}]";
    }

    private void CheckBattleEnd()
    {
        if (battleEngine.State.Outcome == BattleOutcome.InProgress)
        {
            return;
        }

        StopCast();
        battleRunning = false;
        resetCastButton.Enabled = false;

        var player = battleEngine.State.Player;
        var enemy = battleEngine.State.Enemy;
        if (battleEngine.State.Outcome == BattleOutcome.Victory)
        {
            resultsTitleLabel.Text = "Победа";
            if (!battleRewardGranted)
            {
                battleRewardGranted = true;
                GrantVictoryReward();
            }
        }
        else
        {
            resultsTitleLabel.Text = "Поражение";
        }

        resultsDetailsLabel.Text =
            $"HP игрока: {player.Hp}/{player.MaxHp}\r\n" +
            $"Мана игрока: {player.Mana}/{player.MaxMana}\r\n" +
            $"HP врага: {enemy.Hp}/{enemy.MaxHp}\r\n" +
            $"Правило врага: {battleEngine.EnemyInfo.AttackDamage} урона каждые {battleEngine.EnemyInfo.AttackEveryMs / 1000.0:F1}с.\r\n" +
            $"Прогресс: {progress.Wins} побед, {progress.Xp} XP, открыт Tier {progress.HighestUnlockedTier}.";

        SetScreen("Results");
    }

    private void UpdateBattleUi()
    {
        var state = battleEngine.State;
        playerHpBar.Value = Math.Clamp(state.Player.HpPercent, 0, 100);
        playerManaBar.Value = Math.Clamp(state.Player.ManaPercent, 0, 100);
        enemyHpBar.Value = Math.Clamp(state.Enemy.HpPercent, 0, 100);
        enemyManaBar.Value = Math.Clamp(state.Enemy.ManaPercent, 0, 100);

        playerHpLabel.Text = $"HP: {state.Player.Hp}/{state.Player.MaxHp}";
        playerManaLabel.Text = $"Мана: {state.Player.Mana}/{state.Player.MaxMana}";
        enemyHpLabel.Text = $"HP: {state.Enemy.Hp}/{state.Enemy.MaxHp}";
        enemyManaLabel.Text = $"Мана: {state.Enemy.Mana}/{state.Enemy.MaxMana}";
        playerShieldLabel.Text = state.Player.HasShield ? "Щит: ВКЛ" : "Щит: ВЫКЛ";

        var enemyFlags = new List<string> { $"Slow x{state.EnemySlowStacks}" };
        if (state.EnemyHasStoneShield)
        {
            enemyFlags.Add("Каменный щит");
        }
        if (state.EnemyPreparingBurst)
        {
            enemyFlags.Add("Burst готов");
        }
        if (state.DarkMagusCopiedDamage > 0)
        {
            enemyFlags.Add($"Копия +{state.DarkMagusCopiedDamage}");
        }
        enemySlowLabel.Text = string.Join(" | ", enemyFlags);

        if (battleEngine.EnemyInfo.IsTraining)
        {
            enemyAttackLabel.Text = "Тренировка";
        }
        else
        {
            var seconds = Math.Max(0, (state.NextEnemyAttackUtc - DateTime.UtcNow).TotalSeconds);
            enemyAttackLabel.Text = $"Атака через: {seconds:F1}с";
        }
        UpdateEmergencyInterruptPrompt();
    }

    private string BuildBattleHint()
    {
        return selectedEnemy.Id switch
        {
            "goblin_pyro" => "Каждое 3-е действие врага превращается в Fire Burst. Используй парирование в окно подготовки.",
            "ice_golem" => "Каждые 2 действия враг включает каменный щит. Важны тайминг и темп.",
            "dark_magus" => "Dark Magus копирует часть урона твоих атакующих заклинаний в следующий удар.",
            "ancient_dragon" => "Каждая 3-я атака это Inferno Breath. Держи shield к пиковому урону.",
            _ => $"Враг атакует каждые {battleEngine.EnemyInfo.AttackEveryMs / 1000.0:F1}с на {battleEngine.EnemyInfo.AttackDamage} урона. Shield блокирует один удар."
        };
    }

    private void UpdateLoadoutSummary()
    {
        if (selectedLoadout.Count == 0)
        {
            selectedSpellLabel.Text = $"Набор: выбери 1-{LoadoutMaxSpells} заклинания";
            return;
        }

        var names = string.Join(", ", selectedLoadout.Select(s => s.Name));
        selectedSpellLabel.Text = $"Набор ({selectedLoadout.Count}/{LoadoutMaxSpells}): {names}";
    }

    private void UpdateProgressPanel()
    {
        if (progressTitleLabel is null)
        {
            return;
        }

        var tier = progress.HighestUnlockedTier;
        var unlockedCount = allSpells.Count(spell => spell.Tier <= tier);
        var totalCount = Math.Max(1, allSpells.Count);
        var nextXp = GetNextTierXpTarget();
        var nextWins = GetNextTierWinsTarget();
        var xpProgressPercent = nextXp <= progress.Xp
            ? 100
            : Math.Clamp((int)Math.Round(progress.Xp * 100d / nextXp), 0, 100);

        progressTitleLabel.Text = "Прогресс мага";
        progressTierLabel.Text = $"Уровень доступа: Tier {tier}";
        progressStatsLabel.Text =
            $"Опыт: {progress.Xp} XP\r\n" +
            $"Победы: {progress.Wins}\r\n" +
            $"Открыто заклинаний: {unlockedCount}/{totalCount}";
        progressNextLabel.Text = tier >= 4
            ? "Все tier-ы открыты. Дальше можно полировать билд и проходить арену на результат."
            : $"До следующего tier-а: {Math.Max(0, nextXp - progress.Xp)} XP или {Math.Max(0, nextWins - progress.Wins)} побед.";

        progressXpBar.Value = xpProgressPercent;
    }

    private int GetNextTierXpTarget()
    {
        return progress.HighestUnlockedTier switch
        {
            1 => 220,
            2 => 520,
            3 => 1100,
            _ => Math.Max(1, progress.Xp)
        };
    }

    private int GetNextTierWinsTarget()
    {
        return progress.HighestUnlockedTier switch
        {
            1 => 5,
            2 => 10,
            3 => 20,
            _ => Math.Max(1, progress.Wins)
        };
    }

    private void AddLogEvents(IEnumerable<string> events)
    {
        foreach (var text in events)
        {
            AddLog(text);
            MaybeShowBattleEvent(text);
        }
    }

    private void MaybeShowBattleEvent(string text)
    {
        if (currentScreen != "Battle")
        {
            return;
        }

        if (text.Contains("prepares Fire Burst", StringComparison.OrdinalIgnoreCase))
        {
            ShowBattleEvent("FIRE BURST: срочно нужно парирование.", BattleEventKind.CounterWindow, 2400);
            return;
        }

        if (text.Contains("interrupt canceled Fire Burst", StringComparison.OrdinalIgnoreCase))
        {
            ShowBattleEvent("ПАРИРОВАНИЕ УСПЕШНО: Fire Burst отменен.", BattleEventKind.Success, 1800);
            return;
        }

        if (text.Contains("Emergency interrupt canceled Fire Burst", StringComparison.OrdinalIgnoreCase))
        {
            ShowBattleEvent("STOP УСПЕШЕН: Fire Burst отменен.", BattleEventKind.Success, 2000);
            return;
        }

        if (text.Contains("Stone Shield reduced", StringComparison.OrdinalIgnoreCase))
        {
            ShowBattleEvent("КАМЕННЫЙ ЩИТ: этот удар ослаблен.", BattleEventKind.Warning, 1700);
            return;
        }

        if (text.Contains("activates Stone Shield", StringComparison.OrdinalIgnoreCase))
        {
            ShowBattleEvent("КАМЕННЫЙ ЩИТ АКТИВЕН: следующий урон слабее.", BattleEventKind.Warning, 2200);
            return;
        }

        if (text.Contains("shield activated", StringComparison.OrdinalIgnoreCase))
        {
            ShowBattleEvent("ЗАЩИТА ГОТОВА: shield заблокирует следующий удар.", BattleEventKind.DefendNow, 1700);
            return;
        }

        if (text.Contains("Shield absorbed", StringComparison.OrdinalIgnoreCase))
        {
            ShowBattleEvent("БЛОК: твой shield поглотил удар.", BattleEventKind.Success, 1800);
            return;
        }

        if (text.Contains("Dark Magus copied part", StringComparison.OrdinalIgnoreCase))
        {
            ShowBattleEvent("DARK MAGUS СКОПИРОВАЛ УРОН: готовь защиту.", BattleEventKind.Warning, 2200);
            return;
        }

        if (text.Contains("Inferno Breath", StringComparison.OrdinalIgnoreCase))
        {
            ShowBattleEvent("INFERNO BREATH: сейчас будет сильный урон.", BattleEventKind.Danger, 2200);
            return;
        }

        if (text.Equals("Victory.", StringComparison.OrdinalIgnoreCase))
        {
            ShowBattleEvent("ПОБЕДА", BattleEventKind.Success, 2200);
            return;
        }

        if (text.Equals("Defeat.", StringComparison.OrdinalIgnoreCase))
        {
            ShowBattleEvent("ПОРАЖЕНИЕ", BattleEventKind.Danger, 2200);
        }
    }

    private void ShowSelectedSpellContext()
    {
        if (currentScreen != "Battle" || selectedSpell is null || !battleRunning)
        {
            return;
        }

        if (selectedEnemy.IsTraining)
        {
            ShowBattleEvent($"PRACTICE: cast {selectedSpell.Name} by typing {selectedSpell.Pattern}.", BattleEventKind.Info, 1800);
            return;
        }

        if (battleEngine.State.EnemyPreparingBurst && string.Equals(selectedSpell.Effect, "interrupt", StringComparison.OrdinalIgnoreCase))
        {
            ShowBattleEvent($"USE {selectedSpell.Name.ToUpperInvariant()} NOW: it will stop Fire Burst.", BattleEventKind.CounterWindow, 2100);
            return;
        }

        if (battleEngine.State.EnemyPreparingBurst)
        {
            ShowBattleEvent("ОКНО ПАРИРОВАНИЯ: нужен спелл для парирования.", BattleEventKind.CounterWindow, 1800);
            return;
        }

        if (battleEngine.State.EnemyHasStoneShield && selectedSpell.BasePower > 0)
        {
            ShowBattleEvent("STONE SHIELD ACTIVE: this spell will hit weaker.", BattleEventKind.Warning, 1700);
            return;
        }

        if (selectedEnemy.Id == "ancient_dragon" &&
            (battleEngine.State.EnemyActionCounter + 1) % 3 == 0 &&
            string.Equals(selectedSpell.Effect, "shield", StringComparison.OrdinalIgnoreCase))
        {
            ShowBattleEvent($"SAVE {selectedSpell.Name.ToUpperInvariant()}: next dragon hit can spike hard.", BattleEventKind.DefendNow, 1900);
            return;
        }
    }

    private void UpdateEmergencyInterruptPrompt()
    {
        if (currentScreen != "Battle" || !battleRunning || selectedEnemy.IsTraining)
        {
            battleEmergencyPanel.Visible = false;
            emergencyInterruptPosition = 0;
            return;
        }

        if (!battleEngine.State.EnemyPreparingBurst)
        {
            battleEmergencyPanel.Visible = false;
            emergencyInterruptPosition = 0;
            return;
        }

        battleEmergencyPanel.Visible = true;
        var typed = emergencyInterruptPattern[..Math.Clamp(emergencyInterruptPosition, 0, emergencyInterruptPattern.Length)].ToUpperInvariant();
        var rest = emergencyInterruptPattern[Math.Clamp(emergencyInterruptPosition, 0, emergencyInterruptPattern.Length)..].ToUpperInvariant();
        battleEmergencyLabel.Text = emergencyInterruptPosition == 0
            ? "НАБЕРИ STOP, ЧТОБЫ ПРЕРВАТЬ"
            : $"НАБЕРИ {typed}[{rest}], ЧТОБЫ ПРЕРВАТЬ";
        battleEmergencyPanel.BringToFront();
    }

    private bool TryHandleEmergencyInterruptKey(char typedChar)
    {
        if (currentScreen != "Battle" || !battleRunning || !battleEngine.State.EnemyPreparingBurst)
        {
            return false;
        }

        var lower = char.ToLowerInvariant(typedChar);
        var expected = emergencyInterruptPattern[emergencyInterruptPosition];
        if (lower == expected)
        {
            if (emergencyInterruptPosition == 0)
            {
                inputEngine.Stop();
                resultLabel.Text = "Результат: контра активирована";
                resultLabel.ForeColor = Color.Goldenrod;
                castStateLabel.Text = "Состояние: экстренная контра";
            }

            emergencyInterruptPosition++;
            UpdateEmergencyInterruptPrompt();

            if (emergencyInterruptPosition >= emergencyInterruptPattern.Length)
            {
                emergencyInterruptPosition = 0;
                var interruptResult = battleEngine.ApplyEmergencyInterrupt(DateTime.UtcNow);
                AddLogEvents(interruptResult.Events);
                resultLabel.Text = "Результат: STOP сработал";
                resultLabel.ForeColor = Color.DarkGreen;
                castStateLabel.Text = "Состояние: каст врага сорван";
                nextAutoCastUtc = DateTime.UtcNow.AddMilliseconds(AutoCastDelayMs);
                SelectRandomBattleSpell(forceDifferent: true, reason: "После STOP-контры");
                UpdateBattleUi();
                CheckBattleEnd();
            }

            return true;
        }

        if (lower == emergencyInterruptPattern[0])
        {
            emergencyInterruptPosition = 1;
            inputEngine.Stop();
            resultLabel.Text = "Результат: контра активирована";
            resultLabel.ForeColor = Color.Goldenrod;
            castStateLabel.Text = "Состояние: экстренная контра";
            UpdateEmergencyInterruptPrompt();
            return true;
        }

        emergencyInterruptPosition = 0;
        UpdateEmergencyInterruptPrompt();
        return true;
    }

    private static (string Title, Color BackColor, Color AccentColor) ResolveBattleEventStyle(BattleEventKind kind)
    {
        return kind switch
        {
            BattleEventKind.Warning => ("ВНИМАНИЕ", Color.FromArgb(225, 96, 82, 40), Color.FromArgb(255, 231, 176, 77)),
            BattleEventKind.CounterWindow => ("ОКНО КОНТРЫ", Color.FromArgb(230, 131, 52, 26), Color.FromArgb(255, 255, 146, 96)),
            BattleEventKind.DefendNow => ("ЗАЩИЩАЙСЯ", Color.FromArgb(225, 34, 100, 128), Color.FromArgb(255, 112, 210, 255)),
            BattleEventKind.Success => ("УСПЕХ", Color.FromArgb(225, 28, 118, 88), Color.FromArgb(255, 107, 232, 171)),
            BattleEventKind.Danger => ("ОПАСНО", Color.FromArgb(225, 128, 38, 38), Color.FromArgb(255, 255, 122, 122)),
            _ => ("ИНФО", Color.FromArgb(220, 44, 72, 110), Color.FromArgb(255, 145, 193, 255))
        };
    }

    private string BuildEnemyStartBanner()
    {
        return selectedEnemy.Id switch
        {
            "goblin_pyro" => "GOBLIN PYROMANIAC: следи за Fire Burst и отвечай парированием.",
            "ice_golem" => "ICE GOLEM: Stone Shield appears often, so timing matters.",
            "dark_magus" => "DARK MAGUS: сильные удары копируются в следующую атаку врага.",
            "ancient_dragon" => "ANCIENT DRAGON: every third hit spikes hard, keep shield in mind.",
            _ => $"{selectedEnemy.Name}: learn the rhythm and block dangerous hits with shield."
        };
    }

    private static string BuildArenaLegendText(EnemyDefinition enemy)
    {
        return enemy.Id switch
        {
            "training_dummy" => "Здесь можно спокойно учить паттерны, тестировать набор и привыкать к ритму без давления по времени.",
            "goblin_pyro" => "Парирование нужно, чтобы сбивать Fire Burst. Shield пригодится как подстраховка, если не успел в окно.",
            "ice_golem" => "Slow помогает выиграть время. В каменный щит урон проходит хуже, поэтому важен правильный момент для атаки.",
            "dark_magus" => "Сильные удары враг может скопировать. Лучше чередовать давление и защиту, а не бездумно спамить уроном.",
            "ancient_dragon" => "Shield лучше беречь под сильные удары. Slow даёт передышку, а в безопасные окна можно вносить основной урон.",
            _ => "Парирование нужно против опасных кастов, shield спасает от следующего удара, slow помогает выиграть время."
        };
    }

    private void PopulateArenaTags(EnemyDefinition enemy)
    {
        arenaTagPanel.SuspendLayout();
        arenaTagPanel.Controls.Clear();

        foreach (var (text, backColor, foreColor) in BuildArenaTags(enemy))
        {
            var tagLabel = new Label
            {
                AutoSize = true,
                Margin = new Padding(0, 0, 8, 8),
                Padding = new Padding(10, 5, 10, 5),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Text = text,
                BackColor = backColor,
                ForeColor = foreColor
            };
            arenaTagPanel.Controls.Add(tagLabel);
        }

        arenaTagPanel.ResumeLayout();
    }

    private static List<(string Text, Color BackColor, Color ForeColor)> BuildArenaTags(EnemyDefinition enemy)
    {
        var white = Color.White;
        var tags = new List<(string Text, Color BackColor, Color ForeColor)>();

        if (enemy.IsTraining)
        {
            tags.Add(("Тренировка", Color.FromArgb(52, 102, 123), white));
            tags.Add(("Без таймера", Color.FromArgb(46, 126, 125), white));
            tags.Add(("Без XP", Color.FromArgb(78, 92, 110), white));
            return tags;
        }

        tags.Add(("Нужен урон", Color.FromArgb(110, 67, 50), white));

        switch (enemy.Id)
        {
            case "goblin_pyro":
                tags.Add(("Контра", Color.FromArgb(141, 71, 48), white));
                tags.Add(("Нужно парирование", Color.FromArgb(159, 87, 39), white));
                tags.Add(("Опасный burst", Color.FromArgb(121, 53, 34), white));
                break;
            case "ice_golem":
                tags.Add(("Щит врага", Color.FromArgb(104, 91, 48), white));
                tags.Add(("Темп", Color.FromArgb(54, 101, 121), white));
                tags.Add(("Ровный урон", Color.FromArgb(70, 85, 113), white));
                break;
            case "dark_magus":
                tags.Add(("Копирует урон", Color.FromArgb(90, 72, 122), white));
                tags.Add(("Нужна защита", Color.FromArgb(47, 99, 122), white));
                tags.Add(("Контроль", Color.FromArgb(82, 80, 109), white));
                break;
            case "ancient_dragon":
                tags.Add(("Сильный удар", Color.FromArgb(136, 61, 42), white));
                tags.Add(("Жми shield", Color.FromArgb(49, 103, 126), white));
                tags.Add(("Босс", Color.FromArgb(133, 77, 33), white));
                break;
            default:
                tags.Add(("Обычный бой", Color.FromArgb(77, 93, 112), white));
                tags.Add(("Щит полезен", Color.FromArgb(49, 103, 126), white));
                break;
        }

        if (string.Equals(enemy.Type, "boss", StringComparison.OrdinalIgnoreCase))
        {
            tags.Add(("Босс", Color.FromArgb(128, 69, 35), white));
        }

        return tags;
    }

    private void AddLog(string text)
    {
        combatLogList.Items.Insert(0, $"[{DateTime.Now:HH:mm:ss.fff}] {text}");
        while (combatLogList.Items.Count > 90)
        {
            combatLogList.Items.RemoveAt(combatLogList.Items.Count - 1);
        }
    }

    private void UpdatePatternDisplay()
    {
        patternBox.Clear();
        if (string.IsNullOrEmpty(currentPattern))
        {
            return;
        }

        var index = Math.Clamp(inputEngine.Position, 0, currentPattern.Length);
        var okPrefix = currentPattern[..index];
        var rest = currentPattern[index..];

        if (okPrefix.Length > 0)
        {
            patternBox.SelectionColor = inputEngine.IsActive ? Color.DarkGreen : SystemColors.ControlText;
            patternBox.AppendText(okPrefix);
        }

        if (rest.Length > 0)
        {
            patternBox.SelectionColor = SystemColors.ControlText;
            patternBox.AppendText(rest);
        }

        patternBox.SelectionStart = patternBox.TextLength;
        patternBox.SelectionLength = 0;
    }

    private static int ResolveCastWindow(string pattern, int? explicitWindowMs)
    {
        if (explicitWindowMs.HasValue && explicitWindowMs.Value > 0)
        {
            return explicitWindowMs.Value;
        }

        return pattern.Length switch
        {
            <= 1 => 2300,
            <= 2 => 2200,
            <= 4 => 2100,
            _ => 1900
        };
    }

    private void BuildExamSpellPool()
    {
        examSpellPool.Clear();
        examSpellPool.AddRange(learnedSpells.OrderBy(GetDifficultyBand).ThenBy(spell => spell.Pattern.Length).ThenBy(spell => spell.Name));
    }

    private void HighlightExamSpell(SpellDefinition spell)
    {
        var index = examSpellPool.FindIndex(item => ReferenceEquals(item, spell));
        if (index >= 0 && index < examSpellList.Items.Count)
        {
            examSpellList.SelectedIndex = index;
        }
    }

    private int ResolveExamBand()
    {
        if (examSuccesses >= 8 || examCombo >= 4)
        {
            return 3;
        }

        if (examSuccesses >= 3 || examCombo >= 2)
        {
            return 2;
        }

        return 1;
    }

    private static int ResolveExamCastWindowMs(SpellDefinition spell)
    {
        var baseWindow = spell.CastWindowMs.GetValueOrDefault(ResolveCastWindow(spell.Pattern, null));
        return Math.Max(1400, (int)Math.Round(baseWindow * 0.9));
    }

    private static int GetDifficultyBand(SpellDefinition spell)
    {
        var pattern = spell.Pattern;
        var specialCount = pattern.Count(ch => !char.IsLetterOrDigit(ch));
        var score = pattern.Length + specialCount * 2;

        if (score >= 13)
        {
            return 3;
        }

        if (score >= 8)
        {
            return 2;
        }

        return 1;
    }

    private static int GetDifficultyBonus(SpellDefinition spell)
    {
        return GetDifficultyBand(spell) switch
        {
            3 => 40,
            2 => 15,
            _ => 0
        };
    }

    private static string GetDifficultyLabel(SpellDefinition spell)
    {
        return GetDifficultyBandLabel(GetDifficultyBand(spell));
    }

    private static string GetDifficultyBandLabel(int band)
    {
        return band switch
        {
            3 => "Hard",
            2 => "Medium",
            1 => "Easy",
            _ => "-"
        };
    }

    private string GetExamModeLabel()
    {
        return examModeCombo.SelectedIndex switch
        {
            1 => "Длинная тренировка 90с",
            2 => "Строгий режим 60с",
            _ => "Обычный режим 60с"
        };
    }

    private void RecalculateExamScore()
    {
        examScore = examSuccesses * 100 + examSpeedBonusTotal + examDifficultyBonusTotal + examMaxCombo * 25;
    }

    private void FocusCapture()
    {
        ActiveControl = null;
    }

    private void MenuButton_Click(object sender, EventArgs e) => SetScreen("MainMenu");
    private void LoadoutButton_Click(object sender, EventArgs e) => SetScreen("Loadout");
    private void ProgressButton_Click(object sender, EventArgs e) => SetScreen("Progress");
    private void BattleButton_Click(object sender, EventArgs e) => SetScreen("Arena");
    private void ExamButton_Click(object sender, EventArgs e) => SetScreen("Exam");
    private void ResultsButton_Click(object sender, EventArgs e) => SetScreen("Results");
    private void ResetProgressButton_Click(object sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            "Сбросить XP, победы и открытые tier-ы? После сброса будут доступны только простые заклинания Tier 1.",
            "Сброс прогресса",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm == DialogResult.Yes)
        {
            ResetProgressToNewPlayer();
        }
    }
    private void StartCastButton_Click(object sender, EventArgs e)
    {
        if (currentScreen != "Battle")
        {
            return;
        }

        StartBattleSession();
    }
    private void RematchButton_Click(object sender, EventArgs e) => SetScreen("Battle");

    private void LearnedSpellsList_DoubleClick(object sender, EventArgs e)
    {
        // Keep double click as a quick toggle for current row.
        LearnedSpellsList_SelectedIndexChanged(sender, e);
    }

    private void LearnedSpellsList_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (suppressLoadoutSelectionChanged)
        {
            return;
        }

        if (learnedSpellsList.SelectedIndices.Count > LoadoutMaxSpells)
        {
            suppressLoadoutSelectionChanged = true;
            learnedSpellsList.ClearSelected();
            foreach (var index in lastValidLoadoutIndices.OrderBy(i => i))
            {
                if (index >= 0 && index < learnedSpellsList.Items.Count)
                {
                    learnedSpellsList.SetSelected(index, true);
                }
            }
            suppressLoadoutSelectionChanged = false;
            loadoutHintLabel.Text = $"Лимит loadout: {LoadoutMaxSpells} заклинания.";
            return;
        }

        selectedLoadout.Clear();
        lastValidLoadoutIndices.Clear();
        foreach (var item in learnedSpellsList.SelectedItems)
        {
            if (item is SpellDefinition spell)
            {
                selectedLoadout.Add(spell);
            }
        }
        foreach (int index in learnedSpellsList.SelectedIndices)
        {
            lastValidLoadoutIndices.Add(index);
        }

        UpdateLoadoutSummary();
    }

    private void StartBattleButton_Click(object sender, EventArgs e)
    {
        if (selectedLoadout.Count == 0)
        {
            loadoutHintLabel.Text = $"Выбери от 1 до {LoadoutMaxSpells} заклинаний перед стартом боя.";
            return;
        }

        SetScreen("Battle");
    }

    private void ResetCastButton_Click(object sender, EventArgs e)
    {
        inputEngine.ResetProgress();
        resultLabel.Text = "Result: reset";
        resultLabel.ForeColor = SystemColors.ControlText;
        castStateLabel.Text = "State: waiting next auto-cast";
        nextAutoCastUtc = DateTime.UtcNow.AddMilliseconds(AutoCastDelayMs);
        UpdatePatternDisplay();
        FocusCapture();
    }

    private void HandleTypedChar(char typedChar)
    {
        if (examRunning)
        {
            var examState = inputEngine.Input(typedChar, DateTime.UtcNow);
            UpdateExamPatternDisplay();
            if (examState.State == SpellInputState.FailedWrongChar)
            {
                HandleExamFailure($"expected '{examState.Expected}', got '{examState.Received}'");
                return;
            }

            if (examState.State == SpellInputState.FailedTimeout)
            {
                HandleExamFailure("timeout");
                return;
            }

            if (examState.State == SpellInputState.Success)
            {
                HandleExamSuccess();
            }

            return;
        }

        if (!inputEngine.IsActive)
        {
            return;
        }

        var state = inputEngine.Input(typedChar, DateTime.UtcNow);
        UpdatePatternDisplay();
        if (state.State == SpellInputState.FailedWrongChar)
        {
            FailCast($"expected '{state.Expected}', got '{state.Received}' at pos {state.Position + 1}/{currentPattern.Length}", showOverlay: true);
            return;
        }
        if (state.State == SpellInputState.FailedTimeout)
        {
            FailCast("timeout", showOverlay: false);
            return;
        }

        if (state.State == SpellInputState.Success)
        {
            SucceedCast();
        }

        UpdateFailOverlay(DateTime.UtcNow);
    }

    private void MainForm_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (char.IsControl(e.KeyChar))
        {
            return;
        }

        var shouldCapture =
            examRunning ||
            (currentScreen == "Battle" && battleRunning && inputEngine.IsActive);

        if (!shouldCapture)
        {
            return;
        }

        if (currentScreen == "Battle" && TryHandleEmergencyInterruptKey(e.KeyChar))
        {
            e.Handled = true;
            return;
        }

        HandleTypedChar(e.KeyChar);
        e.Handled = true;
    }

    private void EnemyModelPanel_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        if (cachedEnemyPortrait is not null)
        {
            g.DrawImage(cachedEnemyPortrait, new Rectangle(8, 8, enemyModelPanel.Width - 16, enemyModelPanel.Height - 16));
            return;
        }

        var hp = battleEngine.State.Enemy.HpPercent;
        var bodyColor = hp > 60 ? Color.MediumPurple : hp > 30 ? Color.OrangeRed : Color.DarkRed;
        using var bodyBrush = new SolidBrush(bodyColor);
        using var eyeBrush = new SolidBrush(Color.WhiteSmoke);
        using var pupilBrush = new SolidBrush(Color.Black);
        using var outlinePen = new Pen(Color.Black, 2f);

        g.FillEllipse(bodyBrush, 40, 38, 100, 100);
        g.DrawEllipse(outlinePen, 40, 38, 100, 100);
        g.FillEllipse(eyeBrush, 65, 72, 16, 16);
        g.FillEllipse(eyeBrush, 99, 72, 16, 16);
        g.FillEllipse(pupilBrush, 70, 77, 6, 6);
        g.FillEllipse(pupilBrush, 104, 77, 6, 6);
        g.DrawArc(outlinePen, 72, 92, 36, 22, 0, 180);
        g.DrawLine(outlinePen, 90, 138, 90, 175);
        g.DrawLine(outlinePen, 90, 160, 65, 175);
        g.DrawLine(outlinePen, 90, 160, 115, 175);
    }

    private void ExamSpellList_DoubleClick(object sender, EventArgs e)
    {
        if (examRunning)
        {
            return;
        }

        var selectedIndex = examSpellList.SelectedIndex;
        if (selectedIndex >= 0 && selectedIndex < examSpellPool.Count)
        {
            var spell = examSpellPool[selectedIndex];
            selectedSpell = spell;
            currentPattern = spell.Pattern;
            castWindowMs = ResolveExamCastWindowMs(spell);
            inputEngine.ResetProgress();
            examResultLabel.Text = $"Preview: {spell.Name} ({GetDifficultyLabel(spell)})";
            examResultLabel.ForeColor = Color.SteelBlue;
            UpdateExamPatternDisplay();
        }
    }

    private void ExamStartButton_Click(object sender, EventArgs e)
    {
        StartExam();
    }

    private void ExamResetButton_Click(object sender, EventArgs e)
    {
        ResetExamUi();
        if (currentScreen == "Exam")
        {
            FocusCapture();
        }
    }
}
