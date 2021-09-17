using System.IO;
using UnityEngine;

public class LevelConverter : MonoBehaviour
{
    private Level _level;
    private Level JsonFileReader()
    {
        using StreamReader reader = new StreamReader("Scripts/SongInformation/JsonToC#/Level");
        string jsonString = reader.ReadToEnd();
        return JsonUtility.FromJson<Level>(jsonString);
    }

    void Start()
    {
        _level = JsonFileReader();
    }

    public int GetLevel(string songName, string diff)
    {
        foreach (SongName song in _level.songs)
        {
            if (song.title == songName)
            {
                return diff switch
                {
                    "Easy" => song.Easy,
                    "Hard" => song.Hard,
                    "Extreme" => song.Extreme,
                    "KUJO" => song.KUJO,
                    _ => 0
                };
            }
        }
        return 0;
    }
}
