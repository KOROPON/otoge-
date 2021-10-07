using System;
using System.IO;
using UnityEngine;

public class GetHighScores : MonoBehaviour
{
    private string _jsonFilePath;
    private HighScores _highScore;

    private HighScores SongInfo(string songPath)
    {
        using var reader = new StreamReader(songPath);
        var jsonString = reader.ReadToEnd();
        return JsonUtility.FromJson<HighScores>(jsonString);
    }

    private void StreamWrite()
    {
        using var writer = new StreamWriter(_jsonFilePath);
        writer.WriteLine(JsonUtility.ToJson(_highScore, true));
    }

    public void Awake()
    {
        _jsonFilePath = Application.persistentDataPath + "/SongInformation.json";
        if (!File.Exists(_jsonFilePath))
        {
            _highScore = new HighScores();
            StreamWrite();
        }
        _highScore = SongInfo(_jsonFilePath);
    }

    private Song GetSong(string title)
    {
        foreach (var t in _highScore.songs)
        {
            if (title == t.title)
            {
                return t;
            }
        }
        var emptySong = new Song();
        Array.Resize(ref _highScore.songs, _highScore.songs.Length + 1);
        _highScore.songs[_highScore.songs.Length - 1] = emptySong;
        emptySong.title = title;
        return emptySong;
    }

    private Difficulty GetDiff(string title, string difficulty)
    {
        var songName = GetSong(title);

        var diff = difficulty switch
        {
            "Easy" => songName.Easy,
            "Hard" => songName.Hard,
            "Extreme" => songName.Extreme,
            "KUJO" => songName.KUJO,
            _ => null
        };
        return diff;
    }

    private string RankCalculator(int score)
    {
        return score switch
        {
            int n when n >= 995000 => "SSS",
            int n when n >= 990000 => "SS",
            int n when n >= 980000 => "S",
            int n when n >= 950000 => "A",
            int n when n >= 900000 => "B",
            int n when n >= 800000 => "C",
            int n when n > 0 => "D",
            _ => ""
        };
    }

    public bool GetLock(string songName)
    {
        var song = GetSong(songName);
        return song != null && song.extremeLock;
    }

    public int GetHighScore(string songName, string difficulty)
    {
        var diff = GetDiff(songName, difficulty);
        return diff?.highScore ?? 0;
    }

    public string GetRank(string songName, string difficulty)
    {
        var diff = GetDiff(songName, difficulty);
        return diff != null ? diff.rank : "";
    }

    public void SetHighScore(string songName, string difficulty, int score)
    {
        var diff = GetDiff(songName, difficulty) ?? new Difficulty();
        diff.highScore = score;
        diff.rank = RankCalculator(score);
        if (difficulty == "Hard" && diff.highScore >= 980000)
        {
            GetSong(songName).extremeLock = true;
        }
        StreamWrite();
    }
}
