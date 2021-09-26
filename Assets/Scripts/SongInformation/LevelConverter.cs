using UnityEngine;

public class LevelConverter : MonoBehaviour
{
    private TextAsset _jsonFile;
    
    public static Level level;
    
    void Start()
    {
        _jsonFile = Resources.Load<TextAsset>("Level/level");
        level = JsonUtility.FromJson<Level>(_jsonFile.text);
    }

    public int? GetLevel(string songName, string difficulty)
    {
        foreach (SongName song in level.songs)
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
