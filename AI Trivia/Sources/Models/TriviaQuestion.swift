import Foundation

struct TriviaQuestion: Codable, Identifiable {
    let id: Int
    let category: String
    let question: String
    let answers: [String]
    let correctIndex: Int
    let difficulty: String
    let funFact: String
}

struct TriviaBank {
    let questions: [TriviaQuestion]
    
    static func load() -> TriviaBank {
        guard let url = Bundle.module.url(forResource: "trivia_questions", withExtension: "json") else {
            fatalError("trivia_questions.json not found in bundle")
        }
        do {
            let data = try Data(contentsOf: url)
            let questions = try JSONDecoder().decode([TriviaQuestion].self, from: data)
            return TriviaBank(questions: questions)
        } catch {
            fatalError("Failed to decode trivia_questions.json: \(error)")
        }
    }
    
    func randomQuestion(excluding ids: Set<Int>) -> TriviaQuestion? {
        let available = questions.filter { !ids.contains($0.id) }
        if available.isEmpty {
            // Reset - all questions used, start over
            return questions.randomElement()
        }
        return available.randomElement()
    }
    
    func randomQuestion(excluding ids: Set<Int>, categories: Set<String>) -> TriviaQuestion? {
        let filtered = questions.filter { categories.contains($0.category) }
        let available = filtered.filter { !ids.contains($0.id) }
        if available.isEmpty {
            return filtered.randomElement()
        }
        return available.randomElement()
    }
}
