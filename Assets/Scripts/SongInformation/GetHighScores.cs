using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GetHighScores : MonoBehaviour
{
    private string _jsonFilePath;
    private HighScores _highScore;

    private static HighScores SongInfo(string songPath)
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
            "Easy" => songName.easy,
            "Hard" => songName.hard,
            "Extreme" => songName.extreme,
            "KUJO" => songName.kujo,
            _ => null
        };
        return diff;
    }

    private string RankCalculator(int score)
    {
        return score switch
        {
            var n when n >= 995000 => "SSS",
            var n when n >= 990000 => "SS",
            var n when n >= 980000 => "S",
            var n when n >= 950000 => "A",
            var n when n >= 900000 => "B",
            var n when n >= 800000 => "C",
            var n when n > 0 => "D",
            _ => ""
        };
    }

    public bool GetLock(string songName)
    {
        var song = GetSong(songName);
        return song != null && song.extremeLock;
    }

    public string GetClear(string songName, string difficulty)
    {
        var diff = GetDiff(songName, difficulty);
        if (diff.allPerfect) return "All Perfect";
        if (diff.fullCombo) return "Full Combo";
        return diff.clear ? "Clear" : "Fail";
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

    public void SetHighScore(string songName, string difficulty, int score, bool allPerfect, bool fullCombo, bool clear)
    {
        var diff = GetDiff(songName, difficulty) ?? new Difficulty();
        diff.highScore = score;
        diff.rank = RankCalculator(score);
        if (allPerfect)
        {
            diff.allPerfect = true;
            diff.fullCombo = true;
            diff.clear = true;
        }
        else if (fullCombo)
        {
            diff.fullCombo = true;
            diff.clear = true;
        }
        if (difficulty == "Hard" && diff.highScore >= 980000)
        {
            GetSong(songName).extremeLock = true;
        }
        if (clear) diff.clear = true;
        StreamWrite();
    }
}
