using UnityEngine;

public class LevelConverter : MonoBehaviour
{
    private TextAsset _jsonFile;
    
    public static SongDataBase songData;
    
    void Start()
    {
        _jsonFile = Resources.Load<TextAsset>("Level/SongDataBase");
        songData = JsonUtility.FromJson<SongDataBase>(_jsonFile.text);
    }

    public int? GetLevel(string songName, string difficulty)
    {
        foreach (SongName song in songData.songs)
        {
            if (song.title == songName)
            {
                Level lev = song.level;
                return difficulty switch
                {
                    "Easy" => lev.Easy,
                    "Hard" => lev.Hard,
                    "Extreme" => lev.Extreme,
                    _ => null
                };
            }
        }

        return null;
    }
}
