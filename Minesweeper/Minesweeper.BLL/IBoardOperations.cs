using System;
using Minesweeper.Models;

namespace Minesweeper.BLL
{
    // Defines operations that can be performed on a Minesweeper board
    public interface IBoardOperations
    {
        // Set up bombs and initial state for a new game
        void SetupBombs(BoardModel board);

        // Count how many bombs are near each cell
        void CountBombsNearby(BoardModel board);

        // Reveal a cell and apply game rules
        void RevealCell(BoardModel board, int r, int c);

        // Older reveal method kept for tests or backward use
        [Obsolete("Use RevealCell(board, r, c) instead for proper flood-fill behavior.")]
        bool VisitCell(BoardModel board, int r, int c);

        // Toggle a flag on a cell
        void ToggleFlag(BoardModel board, int r, int c);

        // Use a reward to peek at a cell without revealing it
        string UseRewardPeek(BoardModel board, int r, int c);

        // Check if the game is won, lost or still playing
        GameState DetermineGameState(BoardModel board);

        // Calculate the final score for a finished game
        int DetermineFinalScore(BoardModel board);

        // Create a game stat entry for a finished game
        GameStat CreateGameStat(BoardModel board, string playerName);
    }
}
