using System;
using System.Collections;
using System.Collections.Generic;
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
        foreach (var t in _highScore.songs)
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

    public void SetHighScore(string songName, string difficulty, int score)
    {
        Difficulty diff = GetDiff(songName, difficulty);
        if (diff == null)
        {
            diff = new Difficulty();
        }
        diff.highScore = score;
        StreamWrite();
    }
}
