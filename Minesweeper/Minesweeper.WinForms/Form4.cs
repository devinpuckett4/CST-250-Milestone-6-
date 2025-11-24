using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Minesweeper.Models;

namespace Minesweeper.WinForms
{
    // High scores window
    public partial class Form4 : Form
    {
        // Optional new score passed in from the game
        private readonly GameStat? _newScore;

        // Binding source for the grid
        private readonly BindingSource bindingSource = new();

        // Grid that shows high scores
        private readonly DataGridView dgvHighScores = new();

        // Labels that show average score and time
        private readonly Label lblAverageScore = new();
        private readonly Label lblAverageTime = new();

        public Form4(GameStat? newScore = null)
        {
            _newScore = newScore;
            InitializeComponents();
        }

        // Set up all controls on the form
        private void InitializeComponents()
        {
            Text = "High Scores";
            Width = 720;
            Height = 640; // taller so the bottom panel is visible
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;

            // Top menu bar
            var menu = new MenuStrip();

            // File menu with save, load and exit
            var fileMenu = new ToolStripMenuItem("File");
            fileMenu.DropDownItems.Add("Save", null, (s, e) => HighScoreManager.Save());
            fileMenu.DropDownItems.Add("Load", null, (s, e) =>
            {
                HighScoreManager.Load();
                BindScoresAndRefresh();
            });
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("Exit", null, (s, e) => Close());

            // Sort menu for different sort orders
            var sortMenu = new ToolStripMenuItem("Sort");

            // Sort by player name
            sortMenu.DropDownItems.Add("By Name", null, (s, e) =>
            {
                var sorted = HighScoreManager.Scores
                    .OrderBy(gs => gs.Name)
                    .ThenByDescending(gs => gs.Score)
                    .ToList();

                bindingSource.DataSource = sorted;
                dgvHighScores.DataSource = bindingSource;
                bindingSource.ResetBindings(false);
            });

            // Sort by score
            sortMenu.DropDownItems.Add("By Score", null, (s, e) =>
            {
                var sorted = HighScoreManager.Scores
                    .OrderByDescending(gs => gs.Score)
                    .ThenBy(gs => gs.GameTime)
                    .ThenByDescending(gs => gs.Date)
                    .ToList();

                bindingSource.DataSource = sorted;
                dgvHighScores.DataSource = bindingSource;
                bindingSource.ResetBindings(false);
            });

            // Sort by date
            sortMenu.DropDownItems.Add("By Date", null, (s, e) =>
            {
                var sorted = HighScoreManager.Scores
                    .OrderByDescending(gs => gs.Date)
                    .ThenByDescending(gs => gs.Score)
                    .ToList();

                bindingSource.DataSource = sorted;
                dgvHighScores.DataSource = bindingSource;
                bindingSource.ResetBindings(false);
            });

            menu.Items.Add(fileMenu);
            menu.Items.Add(sortMenu);

            Controls.Add(menu);
            MainMenuStrip = menu;

            // Data grid for listing scores
            dgvHighScores.Dock = DockStyle.Fill;
            dgvHighScores.ReadOnly = true;
            dgvHighScores.AllowUserToAddRows = false;
            dgvHighScores.BackgroundColor = Color.White;
            dgvHighScores.RowHeadersVisible = false;
            dgvHighScores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Panel at bottom that shows averages
            var statsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                Padding = new Padding(10)
            };

            // Average score label
            lblAverageScore.AutoSize = true;
            lblAverageScore.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            lblAverageScore.Location = new Point(10, 10);

            // Average time label
            lblAverageTime.AutoSize = true;
            lblAverageTime.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            lblAverageTime.Location = new Point(10, 35);

            statsPanel.Controls.Add(lblAverageScore);
            statsPanel.Controls.Add(lblAverageTime);

            // Add grid and stats panel to form
            Controls.Add(dgvHighScores);
            Controls.Add(statsPanel);
            statsPanel.BringToFront();

            // Load event handler
            Load += Form4_Load;
        }

        // Called when the form finishes loading
        private void Form4_Load(object? sender, EventArgs e)
        {
            // Load scores from file
            HighScoreManager.Load();

            // Add a new score if one was passed in
            if (_newScore != null)
                HighScoreManager.AddAndSave(_newScore!);

            // Bind scores and set column formatting
            BindScoresAndRefresh();
        }

        // Bind scores to the grid and refresh labels and formatting
        private void BindScoresAndRefresh()
        {
            bindingSource.DataSource = HighScoreManager.Scores;
            dgvHighScores.DataSource = bindingSource;

            // Hide the Id column
            var idColumn = dgvHighScores.Columns["Id"];
            if (idColumn != null)
                idColumn.Visible = false;

            // Player name column
            var nameCol = dgvHighScores.Columns["Name"];
            if (nameCol != null)
            {
                nameCol.HeaderText = "Player Name";
                nameCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            // Score column
            var scoreCol = dgvHighScores.Columns["Score"];
            if (scoreCol != null)
                scoreCol.HeaderText = "Score";

            // Game time column
            var timeCol = dgvHighScores.Columns["GameTime"];
            if (timeCol != null)
            {
                timeCol.HeaderText = "Time";
                timeCol.DefaultCellStyle.Format = @"mm\:ss";
            }

            // Date column
            var dateCol = dgvHighScores.Columns["Date"];
            if (dateCol != null)
            {
                dateCol.HeaderText = "Date";
                dateCol.DefaultCellStyle.Format = "MM/dd/yy";
            }

            // By default show top scores first
            var sorted = HighScoreManager.Scores
                .OrderByDescending(gs => gs.Score)
                .ThenBy(gs => gs.GameTime)
                .ThenByDescending(gs => gs.Date)
                .ToList();

            bindingSource.DataSource = sorted;
            dgvHighScores.DataSource = bindingSource;
            bindingSource.ResetBindings(false);

            // Update the average labels
            UpdateSummaryLabels();
        }

        // Update the average score and time labels
        private void UpdateSummaryLabels()
        {
            var avgScore = HighScoreManager.AverageScore;
            var avgTime = HighScoreManager.AverageGameTime;

            lblAverageScore.Text = $"Average Score: {avgScore:F0}";
            lblAverageTime.Text = "Average Time: " + avgTime.ToString(@"mm\:ss");
        }
    }
}
