using UnityEngine;

public class LevelConverter : MonoBehaviour
{
    private TextAsset _jsonFile;
    
    public static SongDataBase songData;

    private void Start()
    {
        _jsonFile = Resources.Load<TextAsset>("Level/SongDataBase"); //テキストは取得可能
        songData = JsonUtility.FromJson<SongDataBase>(_jsonFile.text); // SongDataBase にすると破損
    }

    public static int? GetLevel(string songName, string difficulty)
    {
        foreach (var song in songData.songs)
        {
            if (song.title != songName) continue;
            var lev = song.level;
            return difficulty switch
            {
                "Easy" => lev.easy,
                "Hard" => lev.hard,
                "Extreme" => lev.extreme,
                _ => null
            };
        }
        return null;
    }
}
