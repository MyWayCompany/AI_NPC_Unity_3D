using System;

public static class TavernAI
{
    private static readonly Random _random = new Random();

    public static string GenerateReply(string npcName, string message)
    {
        string m = message.ToLower();

        if (m.Contains("привет") || m.Contains("здравств"))
            return $"Приветствую, путник! Я {npcName}, хозяин этой таверны. Присаживайся, эль сегодня отменный!";
        if (m.Contains("как дела") || m.Contains("как ты"))
            return "Хм, дела идут... как идут. Торговля вялая, а по ночам из подвала опять скрипы. Но это ерунда, наверное.";
        if (m.Contains("подвал") || m.Contains("секрет") || m.Contains("что там"))
            return "Подвал? Обычный подвал! Бочки, пыль... Не стоит тебе туда соваться, поверь старому трактирщику.";
        if (m.Contains("эль") || m.Contains("пиво") || m.Contains("выпить"))
            return "Эль — гордость моей таверны! Один глоток — и забудешь все тревоги. Хотя... после вчерашнего я бы на твоём месте не пил.";
        if (m.Contains("имя") || m.Contains("кто ты"))
            return $"Я {npcName}, держу эту таверну уже двадцать лет. Видал тут всякое... и лучше тебе не знать, что именно.";

        string[] defaults = {
            "Хм, интересный вопрос... Налей-ка мне ещё эля, и я подумаю.",
            "Не знаю, дружище. Но если узнаю — ты первый услышишь.",
            "Эх, молодёжь... В моё время такие вопросы не задавали.",
            "Садись, отдохни. Разговоры разговоры, а эль стынет!"
        };
        return defaults[_random.Next(0, defaults.Length)];
    }
}
