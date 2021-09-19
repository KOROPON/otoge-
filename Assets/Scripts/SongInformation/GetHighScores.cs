using System;
using System.IO;
using UnityEngine;

public class GetHighScores : MonoBehaviour
{
    private string _jsonFilePath;
    private HighScores _highScore;

    private HighScores SongInfo(string songPath)
    {
        using StreamReader reader = new StreamReader(songPath);
        string jsonString = reader.ReadToEnd();
        return JsonUtility.FromJson<HighScores>(jsonString);
    }

    private void StreamWrite()
    {
        using StreamWriter writer = new StreamWriter(_jsonFilePath);
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
        foreach (Song t in _highScore.songs)
        {
            if (title == t.title)
            {
                return t;
            }
        }
        Song emptySong = new Song();
        Array.Resize(ref _highScore.songs, _highScore.songs.Length + 1);
        _highScore.songs[_highScore.songs.Length - 1] = emptySong;
        emptySong.title = title;
        return emptySong;
    }

    private Difficulty GetDiff(string title, string difficulty)
    {
        Song songName = GetSong(title);

        Difficulty diff = difficulty switch
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
        switch (score)
        {
            case int n when n >= 995000:
                return "SSS";
            case int n when n >= 990000:
                return "SS";
            case int n when n >= 980000:
                return "S";
            case int n when n >= 950000:
                return "A";
            case int n when n >= 900000:
                return "B";
            case int n when n >= 800000:
                return "C";
            case int n when n > 0:
                return "D";
            default: return "";
        }
    }

    public int GetHighScore(string songName, string difficulty)
    {
        Difficulty diff = GetDiff(songName, difficulty);
        if (diff != null)
        {
            return diff.highScore;
        }
        else
        {
            return 0;
        }
    }

    public string GetRank(string songName, string difficulty)
    {
        Difficulty diff = GetDiff(songName, difficulty);
        if (diff != null)
        {
            return diff.rank;
        }
        else
        {
            return "";
        }
    }

    public void SetHighScore(string songName, string difficulty, int score)
    {
        Difficulty diff = GetDiff(songName, difficulty) ?? new Difficulty();
        diff.highScore = score;
        diff.rank = RankCalculator(score);
        StreamWrite();
    }
}
