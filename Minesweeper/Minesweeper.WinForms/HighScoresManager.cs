using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Minesweeper.Models;
using Newtonsoft.Json;

namespace Minesweeper.WinForms
{
    public static class HighScoreManager
    {
        // highscores.json will live next to your EXE 
        private static readonly string _filePath =
            Path.Combine(Application.StartupPath, "highscores.json");

        public static List<GameStat> Scores { get; private set; } = new();
        
        public static double AverageScore =>
            Scores.Count == 0 ? 0 : Scores.Average(s => s.Score);

        public static TimeSpan AverageGameTime =>
            Scores.Count == 0
                ? TimeSpan.Zero
                : TimeSpan.FromSeconds(Scores.Average(s => s.GameTime.TotalSeconds));

        // Load existing scores, or create empty file if missing/empty/broken
        public static void Load()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Scores = new List<GameStat>();
                    Save();
                    return;
                }

                var json = File.ReadAllText(_filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    Scores = new List<GameStat>();
                    Save();
                    return;
                }

                var loaded = JsonConvert.DeserializeObject<List<GameStat>>(json);
                Scores = loaded ?? new List<GameStat>();
            }
            catch
            {
                // If anything goes wrong, fall back to empty list
                Scores = new List<GameStat>();
            }

            // Always keep them sorted (highest score first)
            Scores = Scores
                .OrderByDescending(s => s.Score)
                .ThenBy(s => s.GameTime)
                .ThenByDescending(s => s.Date)
                .ToList();
        }

        public static void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(Scores, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch
            {
                // Optional: show a MessageBox on error
            }
        }

        // Called from Form4 when you win and enter your name
        public static void AddAndSave(GameStat stat)
        {
            if (stat == null) return;

            // Make sure we are up to date with what’s already on disk
            Load();

            // Give it an Id if your GameStat has that property
            if (stat.Id == Guid.Empty)
                stat.Id = Guid.NewGuid();

            Scores.Add(stat);

            // Re-sort and keep only the top 5
            Scores = Scores
                .OrderByDescending(s => s.Score)
                .ThenBy(s => s.GameTime)
                .ThenByDescending(s => s.Date)
                .Take(10)
                .ToList();

            Save();
        }
    }
}
