using System;
using System.Linq;
using System.Collections.Generic;
using Minesweeper.Models;

namespace Minesweeper.BLL
{
    // Handles all board logic for the Minesweeper game
    public class BoardService : IBoardOperations
    {
        // Random number generator for bomb and reward placement
        private readonly Random _rng;

        public BoardService() : this(new Random()) { }

        public BoardService(Random rng)
        {
            _rng = rng;
        }

        // Set up a new board with bombs, rewards and counts
        public void SetupBombs(BoardModel board)
        {
            // Reset game timing and state
            board.StartTime = DateTime.UtcNow;
            board.EndTime = null;
            board.GameState = GameState.StillPlaying;
            board.RewardsRemaining = 0;

            // Initialize each cell and randomly place bombs
            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    var cell = board.Cells[r, c];
                    cell.IsVisited = false;
                    cell.IsFlagged = false;
                    cell.HasSpecialReward = false;
                    cell.NumberOfBombNeighbors = 0;
                    cell.IsBomb = _rng.NextDouble() < board.DifficultyPercentage;
                }
            }

            // Place up to 5 special reward cells on safe tiles
            PlaceRewards(board, 5);

            // Count bombs around each cell
            CountBombsNearby(board);
        }

        // Place a number of special reward cells on safe tiles
        private void PlaceRewards(BoardModel board, int rewardCount)
        {
            int n = board.Size;

            // Collect all non bomb cells
            var safeCells = new List<(int r, int c)>();
            for (int r = 0; r < n; r++)
                for (int c = 0; c < n; c++)
                    if (!board.Cells[r, c].IsBomb)
                        safeCells.Add((r, c));

            if (safeCells.Count == 0)
                return;

            int toPlace = Math.Min(rewardCount, safeCells.Count);

            // Randomly pick safe cells for rewards
            for (int i = 0; i < toPlace; i++)
            {
                int idx = _rng.Next(safeCells.Count);
                var (rr, cc) = safeCells[idx];
                safeCells.RemoveAt(idx);

                board.Cells[rr, cc].HasSpecialReward = true;
            }

            // Start the player with that many peeks
            board.RewardsRemaining = toPlace;
        }

        // Compute number of bombs near each cell
        public void CountBombsNearby(BoardModel board)
        {
            int n = board.Size;
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    var cell = board.Cells[r, c];

                    // For bombs, use a sentinel value like 9
                    if (cell.IsBomb)
                    {
                        cell.NumberOfBombNeighbors = 9;
                        continue;
                    }

                    int count = 0;
                    for (int dr = -1; dr <= 1; dr++)
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            if (dr == 0 && dc == 0) continue;
                            int rr = r + dr;
                            int cc = c + dc;
                            if (rr >= 0 && rr < n && cc >= 0 && cc < n && board.Cells[rr, cc].IsBomb)
                                count++;
                        }

                    cell.NumberOfBombNeighbors = count;
                }
            }
        }

        // Reveal a cell and apply rules for bombs, numbers, rewards and flood fill
        public void RevealCell(BoardModel board, int r, int c)
        {
            if (!InBounds(board, r, c)) return;
            var cell = board.Cells[r, c];
            if (cell.IsVisited || cell.IsFlagged) return;

            // Make sure first reveal is safe
            bool isFirstReveal = board.Cells.Cast<CellModel>().All(cm => !cm.IsVisited);
            if (isFirstReveal && cell.IsBomb)
                EnsureFirstClickSafe(board, r, c);

            // Give a reward if this cell has one
            if (cell.HasSpecialReward)
            {
                cell.HasSpecialReward = false;
                board.RewardsRemaining += 1;
            }

            // If this is a bomb, mark visited and stop
            if (cell.IsBomb)
            {
                cell.IsVisited = true;
                return;
            }

            // If this cell has a number, only reveal this one
            if (cell.NumberOfBombNeighbors > 0)
            {
                cell.IsVisited = true;
                return;
            }

            // Empty cell with no neighbors, open surrounding area
            FloodFillOpening(board, r, c);
        }

        // Move a bomb away from the first clicked cell
        private void EnsureFirstClickSafe(BoardModel board, int r, int c)
        {
            var cell = board.Cells[r, c];
            var safeCells = new List<(int rr, int cc)>();

            for (int rr = 0; rr < board.Size; rr++)
                for (int cc = 0; cc < board.Size; cc++)
                    if (!board.Cells[rr, cc].IsBomb &&
                        !(rr == r && cc == c) &&
                        !board.Cells[rr, cc].HasSpecialReward)
                        safeCells.Add((rr, cc));

            if (safeCells.Count > 0)
            {
                var (nr, nc) = safeCells[_rng.Next(safeCells.Count)];
                board.Cells[nr, nc].IsBomb = true;
                cell.IsBomb = false;
                CountBombsNearby(board);
            }
        }

        // Recursively reveal empty neighbors
        private void FloodFillOpening(BoardModel board, int r, int c)
        {
            if (!InBounds(board, r, c)) return;
            var cell = board.Cells[r, c];
            if (cell.IsVisited || cell.IsFlagged || cell.IsBomb) return;

            cell.IsVisited = true;
            if (cell.NumberOfBombNeighbors > 0) return;

            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    FloodFillOpening(board, r + dr, c + dc);
                }
        }

        // Simple reveal used by tests and older code
        public bool VisitCell(BoardModel board, int r, int c)
        {
            if (!InBounds(board, r, c)) return false;
            var cell = board.Cells[r, c];
            if (cell.IsVisited || cell.IsFlagged) return false;

            cell.IsVisited = true;

            // Handle reward if present
            if (cell.HasSpecialReward)
            {
                cell.HasSpecialReward = false;
                board.RewardsRemaining += 1;
            }

            return cell.IsBomb;
        }

        // Toggle a flag on a cell
        public void ToggleFlag(BoardModel board, int r, int c)
        {
            if (!InBounds(board, r, c)) return;
            var cell = board.Cells[r, c];
            if (cell.IsVisited) return;
            cell.IsFlagged = !cell.IsFlagged;
        }

        // Use one reward to peek at a cell without revealing it
        public string UseRewardPeek(BoardModel board, int r, int c)
        {
            if (board.RewardsRemaining <= 0)
                return "No reward available.";

            if (!InBounds(board, r, c))
                return "That position is out of bounds.";

            board.RewardsRemaining -= 1;
            var cell = board.Cells[r, c];

            return cell.IsBomb
                ? "Peek result: This cell IS a bomb."
                : "Peek result: This cell is safe.";
        }

        // Set game state to won, lost or still playing
        public GameState DetermineGameState(BoardModel board)
        {
            int n = board.Size;

            // Check for loss any visited bomb
            for (int r = 0; r < n; r++)
                for (int c = 0; c < n; c++)
                    if (board.Cells[r, c].IsBomb && board.Cells[r, c].IsVisited)
                    {
                        board.GameState = GameState.Lost;
                        board.EndTime = DateTime.UtcNow;
                        return board.GameState;
                    }

            // Check for win all safe cells visited
            bool allSafeVisited = true;
            for (int r = 0; r < n; r++)
                for (int c = 0; c < n; c++)
                    if (!board.Cells[r, c].IsBomb && !board.Cells[r, c].IsVisited)
                    {
                        allSafeVisited = false;
                        break;
                    }

            if (allSafeVisited)
            {
                board.GameState = GameState.Won;
                board.EndTime = DateTime.UtcNow;
                return board.GameState;
            }

            // Otherwise still playing
            board.GameState = GameState.StillPlaying;
            return board.GameState;
        }

        // Compute a final score based on size, difficulty and speed
        public int DetermineFinalScore(BoardModel board)
        {
            if (board.GameState != GameState.Won || board.EndTime == null)
                return 0;

            double elapsed = (board.EndTime.Value - board.StartTime).TotalSeconds;
            double baseScore = board.Size * board.Size * 20;
            double difficultyBonus = board.DifficultyPercentage * 5000;
            double speedBonus = Math.Max(0, 3000 - elapsed * 10);

            return (int)(baseScore + difficultyBonus + speedBonus);
        }

        // Create a game stat record after a win
        public GameStat CreateGameStat(BoardModel board, string playerName)
        {
            if (board.GameState != GameState.Won || board.EndTime == null)
                throw new InvalidOperationException("Game must be won to create stat.");

            var elapsed = board.EndTime.Value - board.StartTime;

            return new GameStat
            {
                Name = playerName,
                Score = DetermineFinalScore(board),
                GameTime = elapsed,
                Date = board.EndTime.Value
            };
        }

        // Check whether row and column are inside the board
        private bool InBounds(BoardModel board, int r, int c)
            => r >= 0 && r < board.Size && c >= 0 && c < board.Size;
    }
}
