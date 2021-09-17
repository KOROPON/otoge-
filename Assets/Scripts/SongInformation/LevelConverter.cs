using System.IO;
using UnityEngine;

public class LevelConverter : MonoBehaviour
{
    private Level _level;
    private TextAsset _jsonFile;

    void Start()
    {
        _jsonFile = Resources.Load<TextAsset>("Level/level");
        _level = JsonUtility.FromJson<Level>(_jsonFile.text);
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
