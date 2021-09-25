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

    public int? GetLevel(string songName, string difficulty)
    {
        foreach (SongName song in _level.songs)
        {
            if (song.title == songName)
            {
                return difficulty switch
                {
                    "Easy" => song.Easy,
                    "Hard" => song.Hard,
                    "Extreme" => song.Extreme,
                    _ => null
                };
            }
        }

        return null;
    }
}
