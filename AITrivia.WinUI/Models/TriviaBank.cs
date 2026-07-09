using System.Text.Json;

namespace AITrivia.Models;

public class TriviaBank
{
    public IReadOnlyList<TriviaQuestion> Questions { get; }

    private TriviaBank(List<TriviaQuestion> questions)
    {
        Questions = questions;
    }

    public static TriviaBank Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Assets", "trivia_questions.json");
        AppLog.Info($"TriviaBank.Load from {path}");
        var json = File.ReadAllText(path);
        var questions = JsonSerializer.Deserialize<List<TriviaQuestion>>(json)
            ?? throw new InvalidOperationException("Failed to load trivia questions.");
        AppLog.Info($"TriviaBank.Load loaded {questions.Count} questions");
        return new TriviaBank(questions);
    }

    public TriviaQuestion? RandomQuestion(HashSet<int> excludingIds, HashSet<string> categories)
    {
        var rng = Random.Shared;
        var filtered = Questions.Where(q => categories.Contains(q.Category)).ToList();
        if (filtered.Count == 0) filtered = Questions.ToList();

        var available = filtered.Where(q => !excludingIds.Contains(q.Id)).ToList();
        if (available.Count == 0) available = filtered;

        if (available.Count == 0) return null;
        var selected = available[rng.Next(available.Count)];
        AppLog.Info($"TriviaBank.RandomQuestion selected id={selected.Id} category={selected.Category} difficulty={selected.Difficulty}");
        return selected;
    }
}
