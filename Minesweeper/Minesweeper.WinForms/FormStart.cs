using System;
using System.Drawing;
using System.Windows.Forms;
using Minesweeper.BLL;
using Minesweeper.Models;

namespace Minesweeper.WinForms
{
    // Start screen for setting up the game
    public partial class FormStart : Form
    {
        // Sliders for board size and difficulty
        private TrackBar trkSize, trkDifficulty;

        // Labels that show current slider values
        private Label lblSizeValue, lblDiffValue;

        public FormStart()
        {
            // Basic window settings
            Text = "Minesweeper - Setup";
            StartPosition = FormStartPosition.CenterScreen;
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(560, 380);
            MinimumSize = new Size(560, 380);
            Font = new Font("Segoe UI", 11f);

            // Main layout grid
            var tlp = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 4,
                Padding = new Padding(30, 20, 30, 20)
            };

            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));

            // Label for board size
            var lblSize = new Label
            {
                Text = "Board Size",
                AutoSize = true,
                Anchor = AnchorStyles.Right,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold)
            };

            // Trackbar for board size
            trkSize = new TrackBar
            {
                Minimum = 6,
                Maximum = 24,
                Value = 10,
                TickFrequency = 2,
                LargeChange = 1,
                Dock = DockStyle.Fill
            };

            // Shows current size value
            lblSizeValue = new Label
            {
                Text = "10 × 10",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold)
            };

            // Label for difficulty
            var lblDiff = new Label
            {
                Text = "Difficulty",
                AutoSize = true,
                Anchor = AnchorStyles.Right,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold)
            };

            // Trackbar for difficulty level
            trkDifficulty = new TrackBar
            {
                Minimum = 1,
                Maximum = 3,
                Value = 1,
                TickFrequency = 1,
                LargeChange = 1,
                Dock = DockStyle.Fill
            };

            // Shows current difficulty value
            lblDiffValue = new Label
            {
                Text = "1",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold)
            };

            // Button to start a new game
            var btnStart = new Button
            {
                Text = "Start Game",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                AutoSize = true,
                MinimumSize = new Size(220, 60),
                BackColor = Color.RoyalBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            // Button to view high scores
            var btnHighScores = new Button
            {
                Text = "High Scores",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                AutoSize = true,
                MinimumSize = new Size(220, 60),
                BackColor = Color.DarkOrange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            btnStart.FlatAppearance.BorderSize = 0;
            btnHighScores.FlatAppearance.BorderSize = 0;

            // Open high scores form
            btnHighScores.Click += (s, e) => new Form4().ShowDialog();

            // Update board size label when slider changes
            trkSize.ValueChanged += (s, e) =>
            {
                lblSizeValue.Text = $"{trkSize.Value} × {trkSize.Value}";
                tlp.PerformLayout();
            };

            // Update difficulty label when slider changes
            trkDifficulty.ValueChanged += (s, e) =>
            {
                lblDiffValue.Text = trkDifficulty.Value.ToString();
                tlp.PerformLayout();
            };

            // Start game button handler
            btnStart.Click += (s, e) =>
            {
                int size = trkSize.Value;

                // Map difficulty slider to bomb percentage
                float diffPct = trkDifficulty.Value switch
                {
                    1 => 0.12f,
                    2 => 0.18f,
                    _ => 0.24f
                };

                // Build board and start game form
                var board = new BoardModel(size) { DifficultyPercentage = diffPct };
                var ops = new BoardService();
                ops.SetupBombs(board);
                new FormGame(ops, board).Show();
            };

            // Add controls to layout grid
            tlp.Controls.Add(lblSize, 0, 0);
            tlp.Controls.Add(trkSize, 1, 0);
            tlp.Controls.Add(lblSizeValue, 2, 0);

            tlp.Controls.Add(lblDiff, 0, 1);
            tlp.Controls.Add(trkDifficulty, 1, 1);
            tlp.Controls.Add(lblDiffValue, 2, 1);

            // Panel that holds the two main buttons
            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new Padding(40, 20, 40, 0)
            };
            buttonPanel.Controls.Add(btnStart);
            buttonPanel.Controls.Add(btnHighScores);

            tlp.Controls.Add(buttonPanel, 0, 3);
            tlp.SetColumnSpan(buttonPanel, 3);

            Controls.Add(tlp);
            PerformLayout();
        }
    }
}
