using System.Drawing;
using System.Windows.Forms;

namespace SpellCaster.WinForms.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    private Label titleLabel;
    private Label statusLabel;
    private Panel navPanel;
    private Button menuButton;
    private Button loadoutButton;
    private Button progressButton;
    private Button battleButton;
    private Button examButton;
    private Button resultsButton;
    private Panel contentPanel;
    private Panel menuPanel;
    private Panel loadoutPanel;
    private Panel progressPanel;
    private Panel battlePanel;
    private Panel examPanel;
    private Panel resultsPanel;
    private Label menuIntroLabel;
    private Label loadoutHintLabel;
    private ListBox learnedSpellsList;
    private Label selectedSpellLabel;
    private Button loadoutStartButton;
    private Label progressTitleLabel;
    private Label progressTierLabel;
    private Label progressStatsLabel;
    private Label progressNextLabel;
    private ProgressBar progressXpBar;
    private Button resetProgressButton;
    private GroupBox playerGroup;
    private GroupBox enemyGroup;
    private ProgressBar playerHpBar;
    private ProgressBar playerManaBar;
    private ProgressBar enemyHpBar;
    private ProgressBar enemyManaBar;
    private Label playerHpLabel;
    private Label playerManaLabel;
    private Label enemyHpLabel;
    private Label enemyManaLabel;
    private Label playerShieldLabel;
    private Label enemySlowLabel;
    private Label enemyAttackLabel;
    private Panel enemyModelPanel;
    private GroupBox castGroup;
    private RichTextBox patternBox;
    private Label hintLabel;
    private Label timerLabel;
    private Label resultLabel;
    private Label castStateLabel;
    private Button startCastButton;
    private Button resetCastButton;
    private Panel battleEventPanel;
    private Panel battleEventAccentPanel;
    private Label battleEventTypeLabel;
    private Label battleEventLabel;
    private ListBox combatLogList;
    private Label examHintLabel;
    private ListBox examSpellList;
    private RichTextBox examPatternBox;
    private Label examTimerLabel;
    private Label examResultLabel;
    private Label examStatsLabel;
    private Label examModeLabel;
    private ComboBox examModeCombo;
    private Button examStartButton;
    private Button examResetButton;
    private Label resultsTitleLabel;
    private Label resultsDetailsLabel;
    private Button rematchButton;
    private Panel failOverlayPanel;
    private Label failOverlayLabel;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        titleLabel = new Label();
        statusLabel = new Label();
        navPanel = new Panel();
        menuButton = new Button();
        loadoutButton = new Button();
        progressButton = new Button();
        battleButton = new Button();
        examButton = new Button();
        resultsButton = new Button();
        contentPanel = new Panel();
        menuPanel = new Panel();
        menuIntroLabel = new Label();
        loadoutPanel = new Panel();
        loadoutHintLabel = new Label();
        learnedSpellsList = new ListBox();
        selectedSpellLabel = new Label();
        loadoutStartButton = new Button();
        progressPanel = new Panel();
        progressTitleLabel = new Label();
        progressTierLabel = new Label();
        progressStatsLabel = new Label();
        progressNextLabel = new Label();
        progressXpBar = new ProgressBar();
        resetProgressButton = new Button();
        battlePanel = new Panel();
        battleEventPanel = new Panel();
        battleEventTypeLabel = new Label();
        battleEventLabel = new Label();
        battleEventAccentPanel = new Panel();
        playerGroup = new GroupBox();
        playerHpBar = new ProgressBar();
        playerManaBar = new ProgressBar();
        playerHpLabel = new Label();
        playerManaLabel = new Label();
        playerShieldLabel = new Label();
        enemyGroup = new GroupBox();
        enemyHpBar = new ProgressBar();
        enemyManaBar = new ProgressBar();
        enemyHpLabel = new Label();
        enemyManaLabel = new Label();
        enemySlowLabel = new Label();
        enemyAttackLabel = new Label();
        enemyModelPanel = new Panel();
        castGroup = new GroupBox();
        patternBox = new RichTextBox();
        hintLabel = new Label();
        timerLabel = new Label();
        resultLabel = new Label();
        castStateLabel = new Label();
        startCastButton = new Button();
        resetCastButton = new Button();
        combatLogList = new ListBox();
        examPanel = new Panel();
        examHintLabel = new Label();
        examSpellList = new ListBox();
        examPatternBox = new RichTextBox();
        examTimerLabel = new Label();
        examResultLabel = new Label();
        examStatsLabel = new Label();
        examModeLabel = new Label();
        examModeCombo = new ComboBox();
        examStartButton = new Button();
        examResetButton = new Button();
        resultsPanel = new Panel();
        resultsTitleLabel = new Label();
        resultsDetailsLabel = new Label();
        rematchButton = new Button();
        failOverlayPanel = new Panel();
        failOverlayLabel = new Label();
        navPanel.SuspendLayout();
        contentPanel.SuspendLayout();
        menuPanel.SuspendLayout();
        loadoutPanel.SuspendLayout();
        progressPanel.SuspendLayout();
        battlePanel.SuspendLayout();
        battleEventPanel.SuspendLayout();
        playerGroup.SuspendLayout();
        enemyGroup.SuspendLayout();
        castGroup.SuspendLayout();
        examPanel.SuspendLayout();
        resultsPanel.SuspendLayout();
        failOverlayPanel.SuspendLayout();
        SuspendLayout();
        // 
        // titleLabel
        // 
        titleLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        titleLabel.Location = new Point(16, 10);
        titleLabel.Name = "titleLabel";
        titleLabel.Size = new Size(530, 40);
        titleLabel.TabIndex = 3;
        titleLabel.Text = "SpellCaster Arena";
        // 
        // statusLabel
        // 
        statusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        statusLabel.Location = new Point(16, 50);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(1150, 24);
        statusLabel.TabIndex = 2;
        statusLabel.Text = "Экран: главное меню";
        // 
        // navPanel
        // 
        navPanel.BorderStyle = BorderStyle.FixedSingle;
        navPanel.Controls.Add(menuButton);
        navPanel.Controls.Add(loadoutButton);
        navPanel.Controls.Add(progressButton);
        navPanel.Controls.Add(battleButton);
        navPanel.Controls.Add(examButton);
        navPanel.Controls.Add(resultsButton);
        navPanel.Location = new Point(16, 82);
        navPanel.Name = "navPanel";
        navPanel.Size = new Size(180, 586);
        navPanel.TabIndex = 1;
        // 
        // menuButton
        // 
        menuButton.Location = new Point(14, 16);
        menuButton.Name = "menuButton";
        menuButton.Size = new Size(148, 42);
        menuButton.TabIndex = 0;
        menuButton.Text = "Главная";
        menuButton.Click += MenuButton_Click;
        // 
        // loadoutButton
        // 
        loadoutButton.Location = new Point(14, 66);
        loadoutButton.Name = "loadoutButton";
        loadoutButton.Size = new Size(148, 42);
        loadoutButton.TabIndex = 1;
        loadoutButton.Text = "Набор";
        loadoutButton.Click += LoadoutButton_Click;
        // 
        // progressButton
        // 
        progressButton.Location = new Point(14, 118);
        progressButton.Name = "progressButton";
        progressButton.Size = new Size(148, 42);
        progressButton.TabIndex = 2;
        progressButton.Text = "Прогресс";
        progressButton.Click += ProgressButton_Click;
        // 
        // battleButton
        // 
        battleButton.Location = new Point(14, 166);
        battleButton.Name = "battleButton";
        battleButton.Size = new Size(148, 42);
        battleButton.TabIndex = 3;
        battleButton.Text = "Арена";
        battleButton.Click += BattleButton_Click;
        // 
        // examButton
        // 
        examButton.Location = new Point(14, 216);
        examButton.Name = "examButton";
        examButton.Size = new Size(148, 42);
        examButton.TabIndex = 4;
        examButton.Text = "Экзамен";
        examButton.Click += ExamButton_Click;
        // 
        // resultsButton
        // 
        resultsButton.Location = new Point(14, 266);
        resultsButton.Name = "resultsButton";
        resultsButton.Size = new Size(148, 42);
        resultsButton.TabIndex = 5;
        resultsButton.Text = "Итоги";
        resultsButton.Click += ResultsButton_Click;
        // 
        // contentPanel
        // 
        contentPanel.BorderStyle = BorderStyle.FixedSingle;
        contentPanel.Controls.Add(menuPanel);
        contentPanel.Controls.Add(loadoutPanel);
        contentPanel.Controls.Add(progressPanel);
        contentPanel.Controls.Add(battlePanel);
        contentPanel.Controls.Add(examPanel);
        contentPanel.Controls.Add(resultsPanel);
        contentPanel.Controls.Add(failOverlayPanel);
        contentPanel.Location = new Point(206, 82);
        contentPanel.Name = "contentPanel";
        contentPanel.Size = new Size(960, 586);
        contentPanel.TabIndex = 0;
        // 
        // menuPanel
        // 
        menuPanel.Controls.Add(menuIntroLabel);
        menuPanel.Dock = DockStyle.Fill;
        menuPanel.Location = new Point(0, 0);
        menuPanel.Name = "menuPanel";
        menuPanel.Size = new Size(958, 584);
        menuPanel.TabIndex = 0;
        // 
        // menuIntroLabel
        // 
        menuIntroLabel.Font = new Font("Segoe UI", 11F);
        menuIntroLabel.Location = new Point(20, 20);
        menuIntroLabel.Name = "menuIntroLabel";
        menuIntroLabel.Size = new Size(900, 220);
        menuIntroLabel.TabIndex = 0;
        menuIntroLabel.Text = resources.GetString("menuIntroLabel.Text");
        // 
        // loadoutPanel
        // 
        loadoutPanel.Controls.Add(loadoutHintLabel);
        loadoutPanel.Controls.Add(learnedSpellsList);
        loadoutPanel.Controls.Add(selectedSpellLabel);
        loadoutPanel.Controls.Add(loadoutStartButton);
        loadoutPanel.Dock = DockStyle.Fill;
        loadoutPanel.Location = new Point(0, 0);
        loadoutPanel.Name = "loadoutPanel";
        loadoutPanel.Size = new Size(958, 584);
        loadoutPanel.TabIndex = 1;
        // 
        // loadoutHintLabel
        // 
        loadoutHintLabel.Location = new Point(20, 20);
        loadoutHintLabel.Name = "loadoutHintLabel";
        loadoutHintLabel.Size = new Size(900, 24);
        loadoutHintLabel.TabIndex = 0;
        loadoutHintLabel.Text = "Выбери до 4 заклинаний для loadout. В бою они будут выпадать случайно.";
        // 
        // learnedSpellsList
        // 
        learnedSpellsList.FormattingEnabled = true;
        learnedSpellsList.Location = new Point(20, 52);
        learnedSpellsList.Name = "learnedSpellsList";
        learnedSpellsList.SelectionMode = SelectionMode.MultiExtended;
        learnedSpellsList.Size = new Size(900, 444);
        learnedSpellsList.TabIndex = 1;
        learnedSpellsList.SelectedIndexChanged += LearnedSpellsList_SelectedIndexChanged;
        learnedSpellsList.DoubleClick += LearnedSpellsList_DoubleClick;
        // 
        // selectedSpellLabel
        // 
        selectedSpellLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        selectedSpellLabel.Location = new Point(20, 522);
        selectedSpellLabel.Name = "selectedSpellLabel";
        selectedSpellLabel.Size = new Size(900, 24);
        selectedSpellLabel.TabIndex = 2;
        selectedSpellLabel.Text = "Selected spell: -";
        // 
        // loadoutStartButton
        // 
        loadoutStartButton.Location = new Point(740, 514);
        loadoutStartButton.Name = "loadoutStartButton";
        loadoutStartButton.Size = new Size(180, 42);
        loadoutStartButton.TabIndex = 3;
        loadoutStartButton.Text = "Start Battle";
        loadoutStartButton.Click += StartBattleButton_Click;
        // 
        // progressPanel
        // 
        progressPanel.Controls.Add(progressTitleLabel);
        progressPanel.Controls.Add(progressTierLabel);
        progressPanel.Controls.Add(progressStatsLabel);
        progressPanel.Controls.Add(progressNextLabel);
        progressPanel.Controls.Add(progressXpBar);
        progressPanel.Controls.Add(resetProgressButton);
        progressPanel.Dock = DockStyle.Fill;
        progressPanel.Location = new Point(0, 0);
        progressPanel.Name = "progressPanel";
        progressPanel.Size = new Size(958, 584);
        progressPanel.TabIndex = 2;
        // 
        // progressTitleLabel
        // 
        progressTitleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        progressTitleLabel.Location = new Point(28, 28);
        progressTitleLabel.Name = "progressTitleLabel";
        progressTitleLabel.Size = new Size(880, 52);
        progressTitleLabel.TabIndex = 0;
        progressTitleLabel.Text = "Прогресс мага";
        // 
        // progressTierLabel
        // 
        progressTierLabel.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        progressTierLabel.Location = new Point(32, 96);
        progressTierLabel.Name = "progressTierLabel";
        progressTierLabel.Size = new Size(860, 34);
        progressTierLabel.TabIndex = 1;
        progressTierLabel.Text = "Уровень доступа: Tier 1";
        // 
        // progressStatsLabel
        // 
        progressStatsLabel.Font = new Font("Segoe UI", 12F);
        progressStatsLabel.Location = new Point(34, 194);
        progressStatsLabel.Name = "progressStatsLabel";
        progressStatsLabel.Size = new Size(860, 120);
        progressStatsLabel.TabIndex = 2;
        progressStatsLabel.Text = "Опыт: 0 XP\r\nПобеды: 0\r\nОткрыто заклинаний: 0/0";
        // 
        // progressNextLabel
        // 
        progressNextLabel.Font = new Font("Segoe UI", 10F);
        progressNextLabel.Location = new Point(34, 334);
        progressNextLabel.Name = "progressNextLabel";
        progressNextLabel.Size = new Size(860, 80);
        progressNextLabel.TabIndex = 3;
        progressNextLabel.Text = "До следующего tier-а: -";
        // 
        // progressXpBar
        // 
        progressXpBar.Location = new Point(34, 146);
        progressXpBar.Name = "progressXpBar";
        progressXpBar.Size = new Size(860, 22);
        progressXpBar.Style = ProgressBarStyle.Continuous;
        progressXpBar.TabIndex = 4;
        // 
        // resetProgressButton
        // 
        resetProgressButton.Location = new Point(34, 438);
        resetProgressButton.Name = "resetProgressButton";
        resetProgressButton.Size = new Size(220, 42);
        resetProgressButton.TabIndex = 5;
        resetProgressButton.Text = "Сбросить прогресс";
        resetProgressButton.Click += ResetProgressButton_Click;
        // 
        // battlePanel
        // 
        battlePanel.Controls.Add(battleEventPanel);
        battlePanel.Controls.Add(playerGroup);
        battlePanel.Controls.Add(enemyGroup);
        battlePanel.Controls.Add(enemyModelPanel);
        battlePanel.Controls.Add(castGroup);
        battlePanel.Controls.Add(combatLogList);
        battlePanel.Dock = DockStyle.Fill;
        battlePanel.Location = new Point(0, 0);
        battlePanel.Name = "battlePanel";
        battlePanel.Size = new Size(958, 584);
        battlePanel.TabIndex = 3;
        // 
        // battleEventPanel
        // 
        battleEventPanel.BackColor = Color.FromArgb(210, 35, 56, 92);
        battleEventPanel.BorderStyle = BorderStyle.FixedSingle;
        battleEventPanel.Controls.Add(battleEventTypeLabel);
        battleEventPanel.Controls.Add(battleEventLabel);
        battleEventPanel.Controls.Add(battleEventAccentPanel);
        battleEventPanel.Location = new Point(20, 152);
        battleEventPanel.Name = "battleEventPanel";
        battleEventPanel.Size = new Size(920, 48);
        battleEventPanel.TabIndex = 0;
        battleEventPanel.Visible = false;
        // 
        // battleEventTypeLabel
        // 
        battleEventTypeLabel.BackColor = Color.Transparent;
        battleEventTypeLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        battleEventTypeLabel.ForeColor = Color.White;
        battleEventTypeLabel.Location = new Point(26, 5);
        battleEventTypeLabel.Name = "battleEventTypeLabel";
        battleEventTypeLabel.Size = new Size(140, 16);
        battleEventTypeLabel.TabIndex = 0;
        battleEventTypeLabel.Text = "INFO";
        // 
        // battleEventLabel
        // 
        battleEventLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        battleEventLabel.ForeColor = Color.White;
        battleEventLabel.Location = new Point(26, 18);
        battleEventLabel.Name = "battleEventLabel";
        battleEventLabel.Size = new Size(880, 22);
        battleEventLabel.TabIndex = 1;
        battleEventLabel.Text = "Battle event";
        battleEventLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // battleEventAccentPanel
        // 
        battleEventAccentPanel.BackColor = Color.FromArgb(92, 155, 255);
        battleEventAccentPanel.Dock = DockStyle.Left;
        battleEventAccentPanel.Location = new Point(0, 0);
        battleEventAccentPanel.Name = "battleEventAccentPanel";
        battleEventAccentPanel.Size = new Size(10, 46);
        battleEventAccentPanel.TabIndex = 2;
        // 
        // playerGroup
        // 
        playerGroup.Controls.Add(playerHpBar);
        playerGroup.Controls.Add(playerManaBar);
        playerGroup.Controls.Add(playerHpLabel);
        playerGroup.Controls.Add(playerManaLabel);
        playerGroup.Controls.Add(playerShieldLabel);
        playerGroup.Location = new Point(20, 16);
        playerGroup.Name = "playerGroup";
        playerGroup.Size = new Size(440, 124);
        playerGroup.TabIndex = 1;
        playerGroup.TabStop = false;
        playerGroup.Text = "Player";
        // 
        // playerHpBar
        // 
        playerHpBar.Location = new Point(14, 46);
        playerHpBar.Name = "playerHpBar";
        playerHpBar.Size = new Size(410, 14);
        playerHpBar.TabIndex = 0;
        // 
        // playerManaBar
        // 
        playerManaBar.Location = new Point(14, 92);
        playerManaBar.Name = "playerManaBar";
        playerManaBar.Size = new Size(410, 14);
        playerManaBar.TabIndex = 1;
        // 
        // playerHpLabel
        // 
        playerHpLabel.Location = new Point(12, 24);
        playerHpLabel.Name = "playerHpLabel";
        playerHpLabel.Size = new Size(260, 20);
        playerHpLabel.TabIndex = 2;
        playerHpLabel.Text = "HP: -";
        // 
        // playerManaLabel
        // 
        playerManaLabel.Location = new Point(12, 70);
        playerManaLabel.Name = "playerManaLabel";
        playerManaLabel.Size = new Size(260, 20);
        playerManaLabel.TabIndex = 3;
        playerManaLabel.Text = "Mana: -";
        // 
        // playerShieldLabel
        // 
        playerShieldLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        playerShieldLabel.Location = new Point(280, 24);
        playerShieldLabel.Name = "playerShieldLabel";
        playerShieldLabel.Size = new Size(144, 20);
        playerShieldLabel.TabIndex = 4;
        playerShieldLabel.Text = "Shield: OFF";
        // 
        // enemyGroup
        // 
        enemyGroup.Controls.Add(enemyHpBar);
        enemyGroup.Controls.Add(enemyManaBar);
        enemyGroup.Controls.Add(enemyHpLabel);
        enemyGroup.Controls.Add(enemyManaLabel);
        enemyGroup.Controls.Add(enemySlowLabel);
        enemyGroup.Controls.Add(enemyAttackLabel);
        enemyGroup.Location = new Point(480, 16);
        enemyGroup.Name = "enemyGroup";
        enemyGroup.Size = new Size(460, 124);
        enemyGroup.TabIndex = 2;
        enemyGroup.TabStop = false;
        enemyGroup.Text = "Enemy";
        // 
        // enemyHpBar
        // 
        enemyHpBar.Location = new Point(14, 46);
        enemyHpBar.Name = "enemyHpBar";
        enemyHpBar.Size = new Size(430, 14);
        enemyHpBar.TabIndex = 0;
        // 
        // enemyManaBar
        // 
        enemyManaBar.Location = new Point(14, 92);
        enemyManaBar.Name = "enemyManaBar";
        enemyManaBar.Size = new Size(430, 14);
        enemyManaBar.TabIndex = 1;
        // 
        // enemyHpLabel
        // 
        enemyHpLabel.Location = new Point(12, 24);
        enemyHpLabel.Name = "enemyHpLabel";
        enemyHpLabel.Size = new Size(200, 20);
        enemyHpLabel.TabIndex = 2;
        enemyHpLabel.Text = "HP: -";
        // 
        // enemyManaLabel
        // 
        enemyManaLabel.Location = new Point(12, 70);
        enemyManaLabel.Name = "enemyManaLabel";
        enemyManaLabel.Size = new Size(200, 20);
        enemyManaLabel.TabIndex = 3;
        enemyManaLabel.Text = "Mana: -";
        // 
        // enemySlowLabel
        // 
        enemySlowLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        enemySlowLabel.Location = new Point(220, 24);
        enemySlowLabel.Name = "enemySlowLabel";
        enemySlowLabel.Size = new Size(90, 20);
        enemySlowLabel.TabIndex = 4;
        enemySlowLabel.Text = "Slow: x0";
        // 
        // enemyAttackLabel
        // 
        enemyAttackLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        enemyAttackLabel.Location = new Point(318, 24);
        enemyAttackLabel.Name = "enemyAttackLabel";
        enemyAttackLabel.Size = new Size(130, 20);
        enemyAttackLabel.TabIndex = 5;
        enemyAttackLabel.Text = "Attack in: -";
        // 
        // enemyModelPanel
        // 
        enemyModelPanel.BackColor = Color.FromArgb(34, 39, 46);
        enemyModelPanel.BorderStyle = BorderStyle.FixedSingle;
        enemyModelPanel.Location = new Point(20, 210);
        enemyModelPanel.Name = "enemyModelPanel";
        enemyModelPanel.Size = new Size(180, 200);
        enemyModelPanel.TabIndex = 3;
        enemyModelPanel.Paint += EnemyModelPanel_Paint;
        // 
        // castGroup
        // 
        castGroup.Controls.Add(patternBox);
        castGroup.Controls.Add(hintLabel);
        castGroup.Controls.Add(timerLabel);
        castGroup.Controls.Add(resultLabel);
        castGroup.Controls.Add(castStateLabel);
        castGroup.Controls.Add(startCastButton);
        castGroup.Controls.Add(resetCastButton);
        castGroup.Location = new Point(214, 210);
        castGroup.Name = "castGroup";
        castGroup.Size = new Size(726, 192);
        castGroup.TabIndex = 4;
        castGroup.TabStop = false;
        castGroup.Text = "Spell Casting";
        // 
        // patternBox
        // 
        patternBox.Font = new Font("Consolas", 16F, FontStyle.Bold);
        patternBox.Location = new Point(14, 46);
        patternBox.Name = "patternBox";
        patternBox.ReadOnly = true;
        patternBox.ScrollBars = RichTextBoxScrollBars.None;
        patternBox.Size = new Size(350, 58);
        patternBox.TabIndex = 0;
        patternBox.Text = "";
        // 
        // hintLabel
        // 
        hintLabel.Location = new Point(14, 24);
        hintLabel.Name = "hintLabel";
        hintLabel.Size = new Size(500, 20);
        hintLabel.TabIndex = 1;
        hintLabel.Text = "Auto-cast mode: type current pattern as soon as it appears.";
        // 
        // timerLabel
        // 
        timerLabel.Location = new Point(14, 114);
        timerLabel.Name = "timerLabel";
        timerLabel.Size = new Size(280, 24);
        timerLabel.TabIndex = 2;
        timerLabel.Text = "Timer: -";
        // 
        // resultLabel
        // 
        resultLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        resultLabel.Location = new Point(14, 138);
        resultLabel.Name = "resultLabel";
        resultLabel.Size = new Size(500, 24);
        resultLabel.TabIndex = 3;
        resultLabel.Text = "Result: -";
        // 
        // castStateLabel
        // 
        castStateLabel.Location = new Point(210, 114);
        castStateLabel.Name = "castStateLabel";
        castStateLabel.Size = new Size(290, 24);
        castStateLabel.TabIndex = 4;
        castStateLabel.Text = "State: idle";
        // 
        // startCastButton
        // 
        startCastButton.Location = new Point(520, 46);
        startCastButton.Name = "startCastButton";
        startCastButton.Size = new Size(190, 46);
        startCastButton.TabIndex = 5;
        startCastButton.Text = "Auto Cast ON";
        startCastButton.Click += StartCastButton_Click;
        // 
        // resetCastButton
        // 
        resetCastButton.Location = new Point(520, 98);
        resetCastButton.Name = "resetCastButton";
        resetCastButton.Size = new Size(190, 46);
        resetCastButton.TabIndex = 6;
        resetCastButton.Text = "Reset";
        resetCastButton.Click += ResetCastButton_Click;
        // 
        // combatLogList
        // 
        combatLogList.FormattingEnabled = true;
        combatLogList.Location = new Point(20, 424);
        combatLogList.Name = "combatLogList";
        combatLogList.Size = new Size(920, 104);
        combatLogList.TabIndex = 5;
        // 
        // examPanel
        // 
        examPanel.Controls.Add(examHintLabel);
        examPanel.Controls.Add(examSpellList);
        examPanel.Controls.Add(examPatternBox);
        examPanel.Controls.Add(examTimerLabel);
        examPanel.Controls.Add(examResultLabel);
        examPanel.Controls.Add(examStatsLabel);
        examPanel.Controls.Add(examModeLabel);
        examPanel.Controls.Add(examModeCombo);
        examPanel.Controls.Add(examStartButton);
        examPanel.Controls.Add(examResetButton);
        examPanel.Dock = DockStyle.Fill;
        examPanel.Location = new Point(0, 0);
        examPanel.Name = "examPanel";
        examPanel.Size = new Size(958, 584);
        examPanel.TabIndex = 4;
        // 
        // examHintLabel
        // 
        examHintLabel.Location = new Point(20, 20);
        examHintLabel.Name = "examHintLabel";
        examHintLabel.Size = new Size(900, 24);
        examHintLabel.TabIndex = 0;
        examHintLabel.Text = "Экзамен: тренировка скорости и точности. Чем лучше серия, тем сложнее и ценнее следующие паттерны.";
        // 
        // examSpellList
        // 
        examSpellList.FormattingEnabled = true;
        examSpellList.Location = new Point(20, 52);
        examSpellList.Name = "examSpellList";
        examSpellList.Size = new Size(390, 404);
        examSpellList.TabIndex = 1;
        examSpellList.DoubleClick += ExamSpellList_DoubleClick;
        // 
        // examPatternBox
        // 
        examPatternBox.Font = new Font("Consolas", 16F, FontStyle.Bold);
        examPatternBox.Location = new Point(430, 52);
        examPatternBox.Name = "examPatternBox";
        examPatternBox.ReadOnly = true;
        examPatternBox.ScrollBars = RichTextBoxScrollBars.None;
        examPatternBox.Size = new Size(500, 74);
        examPatternBox.TabIndex = 2;
        examPatternBox.Text = "";
        // 
        // examTimerLabel
        // 
        examTimerLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        examTimerLabel.Location = new Point(430, 140);
        examTimerLabel.Name = "examTimerLabel";
        examTimerLabel.Size = new Size(500, 28);
        examTimerLabel.TabIndex = 3;
        examTimerLabel.Text = "Time Left: -";
        // 
        // examResultLabel
        // 
        examResultLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        examResultLabel.Location = new Point(430, 172);
        examResultLabel.Name = "examResultLabel";
        examResultLabel.Size = new Size(500, 26);
        examResultLabel.TabIndex = 4;
        examResultLabel.Text = "Result: -";
        // 
        // examStatsLabel
        // 
        examStatsLabel.Location = new Point(430, 204);
        examStatsLabel.Name = "examStatsLabel";
        examStatsLabel.Size = new Size(500, 190);
        examStatsLabel.TabIndex = 5;
        examStatsLabel.Text = "Score: 0\r\nCombo: 0\r\nAccuracy: 0%\r\nCPM: 0";
        // 
        // examModeLabel
        // 
        examModeLabel.Location = new Point(430, 404);
        examModeLabel.Name = "examModeLabel";
        examModeLabel.Size = new Size(80, 28);
        examModeLabel.TabIndex = 6;
        examModeLabel.Text = "Режим:";
        // 
        // examModeCombo
        // 
        examModeCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        examModeCombo.Items.AddRange(new object[] { "Обычный - 60 сек", "Длинная тренировка - 90 сек", "Строгий - 60 сек" });
        examModeCombo.Location = new Point(510, 400);
        examModeCombo.Name = "examModeCombo";
        examModeCombo.Size = new Size(180, 28);
        examModeCombo.TabIndex = 7;
        // 
        // examStartButton
        // 
        examStartButton.Location = new Point(430, 450);
        examStartButton.Name = "examStartButton";
        examStartButton.Size = new Size(240, 42);
        examStartButton.TabIndex = 8;
        examStartButton.Text = "Начать экзамен";
        examStartButton.Click += ExamStartButton_Click;
        // 
        // examResetButton
        // 
        examResetButton.Location = new Point(690, 450);
        examResetButton.Name = "examResetButton";
        examResetButton.Size = new Size(240, 42);
        examResetButton.TabIndex = 9;
        examResetButton.Text = "Сбросить";
        examResetButton.Click += ExamResetButton_Click;
        // 
        // resultsPanel
        // 
        resultsPanel.Controls.Add(resultsTitleLabel);
        resultsPanel.Controls.Add(resultsDetailsLabel);
        resultsPanel.Controls.Add(rematchButton);
        resultsPanel.Dock = DockStyle.Fill;
        resultsPanel.Location = new Point(0, 0);
        resultsPanel.Name = "resultsPanel";
        resultsPanel.Size = new Size(958, 584);
        resultsPanel.TabIndex = 5;
        // 
        // resultsTitleLabel
        // 
        resultsTitleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        resultsTitleLabel.Location = new Point(20, 20);
        resultsTitleLabel.Name = "resultsTitleLabel";
        resultsTitleLabel.Size = new Size(900, 52);
        resultsTitleLabel.TabIndex = 0;
        resultsTitleLabel.Text = "Results";
        // 
        // resultsDetailsLabel
        // 
        resultsDetailsLabel.Font = new Font("Segoe UI", 10F);
        resultsDetailsLabel.Location = new Point(20, 82);
        resultsDetailsLabel.Name = "resultsDetailsLabel";
        resultsDetailsLabel.Size = new Size(900, 220);
        resultsDetailsLabel.TabIndex = 1;
        resultsDetailsLabel.Text = "Battle details will appear here.";
        // 
        // rematchButton
        // 
        rematchButton.Location = new Point(20, 314);
        rematchButton.Name = "rematchButton";
        rematchButton.Size = new Size(180, 42);
        rematchButton.TabIndex = 2;
        rematchButton.Text = "Rematch";
        rematchButton.Click += RematchButton_Click;
        // 
        // failOverlayPanel
        // 
        failOverlayPanel.BackColor = Color.FromArgb(220, 20, 20, 20);
        failOverlayPanel.Controls.Add(failOverlayLabel);
        failOverlayPanel.Dock = DockStyle.Fill;
        failOverlayPanel.Location = new Point(0, 0);
        failOverlayPanel.Name = "failOverlayPanel";
        failOverlayPanel.Size = new Size(958, 584);
        failOverlayPanel.TabIndex = 6;
        failOverlayPanel.Visible = false;
        // 
        // failOverlayLabel
        // 
        failOverlayLabel.BackColor = Color.Transparent;
        failOverlayLabel.Dock = DockStyle.Fill;
        failOverlayLabel.Font = new Font("Segoe UI", 72F, FontStyle.Bold);
        failOverlayLabel.ForeColor = Color.Red;
        failOverlayLabel.Location = new Point(0, 0);
        failOverlayLabel.Name = "failOverlayLabel";
        failOverlayLabel.Size = new Size(958, 584);
        failOverlayLabel.TabIndex = 0;
        failOverlayLabel.Text = "ОШИБКА";
        failOverlayLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1184, 681);
        Controls.Add(contentPanel);
        Controls.Add(navPanel);
        Controls.Add(statusLabel);
        Controls.Add(titleLabel);
        KeyPreview = true;
        MinimumSize = new Size(1200, 720);
        Name = "MainForm";
        Text = "SpellCaster Arena - WinForms";
        KeyPress += MainForm_KeyPress;
        navPanel.ResumeLayout(false);
        contentPanel.ResumeLayout(false);
        menuPanel.ResumeLayout(false);
        loadoutPanel.ResumeLayout(false);
        progressPanel.ResumeLayout(false);
        battlePanel.ResumeLayout(false);
        battleEventPanel.ResumeLayout(false);
        playerGroup.ResumeLayout(false);
        enemyGroup.ResumeLayout(false);
        castGroup.ResumeLayout(false);
        examPanel.ResumeLayout(false);
        resultsPanel.ResumeLayout(false);
        failOverlayPanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
