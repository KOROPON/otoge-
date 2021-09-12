using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GetHighScores : MonoBehaviour
{
    private string jsonFilePath;
    private HighScores highScore;

    private HighScores SongInfo(string songPath)
    {
        using (StreamReader reader = new StreamReader(songPath))
        {
            string jsonString = reader.ReadToEnd();
            return JsonUtility.FromJson<HighScores>(jsonString);
        }
    }

    private void StreamWrite()
    {
        using (StreamWriter writer = new StreamWriter(jsonFilePath))
        {
            writer.WriteLine(JsonUtility.ToJson(highScore, true));
        }
    }

    void Awake()
    {
        jsonFilePath = Application.persistentDataPath + "/SongInformation.json";
        if (!File.Exists(jsonFilePath))
        {
            highScore = new HighScores();
            StreamWrite();
        }
        highScore = SongInfo(jsonFilePath);
    }

    private Song GetSong(string title)
    {
        for (int i = 0; i < highScore.songs.Length; i++)
        {
            Debug.Log(title);
            Debug.Log(highScore.songs[i].title);
            if (title == highScore.songs[i].title)
            {
                return highScore.songs[i];
            }
        }
        Song emptySong = new Song();
        Array.Resize(ref highScore.songs, highScore.songs.Length + 1);
        highScore.songs[highScore.songs.Length - 1] = emptySong;
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
