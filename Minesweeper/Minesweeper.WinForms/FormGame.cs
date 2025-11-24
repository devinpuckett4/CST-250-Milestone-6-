using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Minesweeper.BLL;
using Minesweeper.Models;

namespace Minesweeper.WinForms
{
    // Game form for playing Minesweeper
    public partial class FormGame : Form
    {
        // Board logic and data
        private readonly IBoardOperations _ops;
        private readonly BoardModel _board;

        // Main UI controls
        private Panel pnlGrid;
        private Label lblStatus;
        private Label lblTimer;
        private Label lblRewards;
        private Button btnPeek;
        private Button btnClose;
        private System.Windows.Forms.Timer _gameTimer;

        // Cell size in pixels
        private int _cellSize;

        public FormGame(IBoardOperations ops, BoardModel board)
        {
            _ops = ops ?? throw new ArgumentNullException(nameof(ops));
            _board = board ?? throw new ArgumentNullException(nameof(board));

            // Pick cell size based on board size
            int n = _board.Size;
            _cellSize = n <= 9 ? 44 : n <= 14 ? 38 : n <= 19 ? 32 : 28;

            Text = $"Minesweeper - {n}x{n}";
            StartPosition = FormStartPosition.CenterScreen;

            int gridW = _cellSize * n;
            int gridH = _cellSize * n;

            // Size the form around the grid
            ClientSize = new Size(Math.Max(560, gridW + 40), Math.Max(620, gridH + 180));

            // Panel that holds the grid of buttons
            pnlGrid = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(gridW + 2, gridH + 2),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Gainsboro
            };

            // Status label at top left
            lblStatus = new Label
            {
                Text = "Good luck! Left-click reveal • Right-click flag",
                AutoSize = true,
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 11f, FontStyle.Bold)
            };

            // Timer label that will sit next to the status text
            lblTimer = new Label
            {
                Text = "Time: 0s",
                AutoSize = true,
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            // Shows how many peeks are left
            lblRewards = new Label
            {
                Text = "Peeks: 0",
                AutoSize = true,
                Location = new Point(20, pnlGrid.Bottom + 20),
                Font = new Font("Segoe UI", 11f)
            };

            // Button to use a peek reward
            btnPeek = new Button
            {
                Text = "Use Peek",
                Size = new Size(120, 34),
                Location = new Point(20, lblRewards.Bottom + 10)
            };
            btnPeek.Click += BtnPeek_Click;

            // Button to close the game form
            btnClose = new Button
            {
                Text = "Close Game",
                Size = new Size(120, 34),
                Location = new Point(btnPeek.Right + 20, btnPeek.Top)
            };
            btnClose.Click += (s, e) => Close();

            // Game timer updates seconds played
            _gameTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _gameTimer.Tick += GameTimer_Tick;
            _gameTimer.Start();

            // Add main controls to the form
            Controls.AddRange(new Control[] { pnlGrid, lblStatus, lblTimer, lblRewards, btnPeek, btnClose });

            // Build the grid of buttons
            BuildButtons(n);

            // Use the same font for all grid buttons
            pnlGrid.Font = CellFont;

            // Position timer label next to status label
            PositionTimerNextToStatus();

            // Draw the initial board
            RefreshBoard();
        }

        // Monospaced font so symbols and numbers appear centered
        private Font CellFont =>
            new Font("Consolas", Math.Max(10f, _cellSize * 0.55f), FontStyle.Bold, GraphicsUnit.Pixel);

        // Place the timer label just to the right of the status label
        private void PositionTimerNextToStatus()
        {
            lblTimer.Location = new Point(lblStatus.Right + 10, lblStatus.Top);
        }

        // Build all grid buttons based on board size
        private void BuildButtons(int n)
        {
            pnlGrid.Controls.Clear();
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    var btn = new Button
                    {
                        Name = $"btn_{r}_{c}",
                        Size = new Size(_cellSize, _cellSize),
                        Location = new Point(c * _cellSize, r * _cellSize),
                        Tag = (r, c),
                        Margin = Padding.Empty,
                        FlatStyle = FlatStyle.Flat,
                        Font = CellFont,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Padding = Padding.Empty,
                        BackColor = Color.LightGray,
                        Text = ""
                    };

                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.Silver;
                    btn.UseVisualStyleBackColor = false;

                    // Left click and right click handlers
                    btn.Click += OnCellClick;
                    btn.MouseUp += OnCellMouseUp;

                    pnlGrid.Controls.Add(btn);
                }
            }
        }

        // Update timer text every second
        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            var elapsed = DateTime.UtcNow - _board.StartTime;
            lblTimer.Text = $"Time: {(int)elapsed.TotalSeconds}s";
            PositionTimerNextToStatus();
        }

        // Handle right click to toggle a flag
        private void OnCellMouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (sender is not Button btn || btn.Tag is not ValueTuple<int, int> tag) return;
            if (!_board.Cells[tag.Item1, tag.Item2].IsVisited)
            {
                _ops.ToggleFlag(_board, tag.Item1, tag.Item2);
                RefreshBoard();
            }
        }

        // Handle left click to reveal a cell
        private void OnCellClick(object? sender, EventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not ValueTuple<int, int> tag) return;

            // Start timer on first click
            if (_board.StartTime == default)
                _board.StartTime = DateTime.UtcNow;

            int prevRewards = _board.RewardsRemaining;
            _ops.RevealCell(_board, tag.Item1, tag.Item2);

            // Show message if a new reward was earned
            if (_board.RewardsRemaining > prevRewards)
                MessageBox.Show("You found a special Hint reward! You can now use 'Use Peek' safely.", "Reward Found!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshBoard();

            var state = _ops.DetermineGameState(_board);

            if (state == GameState.Lost)
            {
                lblStatus.Text = "Boom! You lost";
                lblStatus.ForeColor = Color.Red;
                PositionTimerNextToStatus();
                RevealAllBombs();
                DisableAllCells();
                _gameTimer.Stop();
            }
            else if (state == GameState.Won)
            {
                _gameTimer.Stop();
                _board.EndTime = DateTime.UtcNow;

                int score = _ops.DetermineFinalScore(_board);

                lblStatus.Text = $"You won! Score: {score}";
                lblStatus.ForeColor = Color.DarkGreen;
                PositionTimerNextToStatus();

                DisableAllCells();

                // Prompt for player name and show high scores
                using var nameForm = new Form3();
                if (nameForm.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(nameForm.PlayerName))
                {
                    var stat = new GameStat
                    {
                        Name = nameForm.PlayerName,
                        Score = score,
                        GameTime = _board.EndTime.Value - _board.StartTime,
                        Date = DateTime.UtcNow
                    };
                    new Form4(stat).ShowDialog();
                }
            }
        }

        // Click handler for the peek button
        private void BtnPeek_Click(object? sender, EventArgs e)
        {
            if (_board.RewardsRemaining <= 0)
            {
                MessageBox.Show("No peeks available!", "Peek", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Small input dialog for row and column
            using var dlg = new Form
            {
                Text = "Use Peek Reward",
                Width = 340,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lbl = new Label { Text = "Enter row,col (e.g. 5,8):", Left = 20, Top = 20, AutoSize = true };
            var txt = new TextBox { Left = 20, Top = 50, Width = 280 };
            var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 140, Top = 90 };
            var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Left = 220, Top = 90 };

            dlg.Controls.AddRange(new Control[] { lbl, txt, ok, cancel });
            dlg.AcceptButton = ok;
            dlg.CancelButton = cancel;

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var parts = txt.Text.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2 || !int.TryParse(parts[0], out int row) || !int.TryParse(parts[1], out int col))
            {
                MessageBox.Show("Invalid format. Use row,col", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (row < 0 || row >= _board.Size || col < 0 || col >= _board.Size)
            {
                MessageBox.Show("Out of bounds!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ask the board logic to peek at this cell
            string result = _ops.UseRewardPeek(_board, row, col);
            MessageBox.Show(result, "Peek Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshBoard();
        }

        // Redraw all cells from the board state
        private void RefreshBoard()
        {
            for (int r = 0; r < _board.Size; r++)
                for (int c = 0; c < _board.Size; c++)
                    RefreshCell(r, c);

            lblRewards.Text = $"Peeks: {_board.RewardsRemaining}";
            btnPeek.Enabled = _board.GameState == GameState.StillPlaying && _board.RewardsRemaining > 0;
        }

        // Choose text color based on number of neighboring bombs
        private static Color ColorForNumber(int num) => num switch
        {
            1 => Color.RoyalBlue,
            2 => Color.ForestGreen,
            3 => Color.Firebrick,
            4 => Color.MidnightBlue,
            5 => Color.Maroon,
            6 => Color.Teal,
            7 => Color.Black,
            8 => Color.DimGray,
            _ => Color.Black
        };

        // Update a single cell button based on its state
        private void RefreshCell(int r, int c)
        {
            if (pnlGrid.Controls[$"btn_{r}_{c}"] is not Button btn) return;

            var cell = _board.Cells[r, c];
            var state = _board.GameState;

            // Center text and emoji
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.Padding = Padding.Empty;
            btn.TextImageRelation = TextImageRelation.Overlay;

            btn.Enabled = state == GameState.StillPlaying && !cell.IsVisited;

            // Default state for unrevealed cell
            btn.Text = "";
            btn.BackColor = Color.LightGray;
            btn.ForeColor = Color.Black;
            btn.Font = CellFont;

            // Show flag on flagged cells
            if (cell.IsFlagged)
            {
                btn.Text = "🚩";
                btn.BackColor = Color.Khaki;
                btn.ForeColor = Color.Red;
                btn.Font = new Font(btn.Font, FontStyle.Bold);
            }

            // Show content for visited cells
            if (cell.IsVisited)
            {
                btn.BackColor = cell.IsBomb ? Color.Red : Color.White;

                if (cell.IsBomb)
                {
                    btn.Text = "💣";
                    btn.ForeColor = Color.White;
                    btn.Font = new Font(btn.Font, FontStyle.Bold);
                }
                else if (cell.NumberOfBombNeighbors > 0)
                {
                    btn.Text = cell.NumberOfBombNeighbors.ToString();
                    btn.ForeColor = ColorForNumber(cell.NumberOfBombNeighbors);
                    btn.Font = new Font(btn.Font, FontStyle.Bold);
                }
            }

            // Extra visuals when the game is lost
            if (state == GameState.Lost)
            {
                if (cell.IsBomb)
                {
                    btn.Text = "💣";
                    btn.BackColor = Color.Red;
                    btn.ForeColor = Color.White;
                    btn.Font = new Font(btn.Font, FontStyle.Bold);
                }
                else if (cell.IsFlagged && !cell.IsBomb)
                {
                    btn.Text = "X";
                    btn.BackColor = Color.Orange;
                    btn.ForeColor = Color.Black;
                }
            }
        }

        // Reveal all bombs after the player loses
        private void RevealAllBombs()
        {
            for (int r = 0; r < _board.Size; r++)
                for (int c = 0; c < _board.Size; c++)
                    if (_board.Cells[r, c].IsBomb)
                        _board.Cells[r, c].IsVisited = true;
            RefreshBoard();
        }

        // Disable all grid buttons when the game is over
        private void DisableAllCells()
        {
            foreach (Button b in pnlGrid.Controls)
                b.Enabled = false;
        }
    }
}
