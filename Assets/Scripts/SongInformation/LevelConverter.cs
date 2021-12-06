#nullable enable
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;

public class TitleAndLevel
{
    public string title;
    public int? level;
}
public class LevelConverter : MonoBehaviour
{
    private TextAsset _jsonFile;
    
    public static SongDataBase songData;

    private void Start()
    {
        _jsonFile = Resources.Load<TextAsset>("Level/SongDataBase");
        songData = JsonUtility.FromJson<SongDataBase>(_jsonFile.text);
    }

    public static List<TitleAndLevel> GetGameObject(string difficulty)
    {
        return (from song in songData.songs
            select song.title
            into title
            let level = GetLevel(title, difficulty)
            where level != 0
            select new TitleAndLevel {title = title, level = level}).OrderBy(level => level.level).ToList();
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
                "Kujo" => lev.kujo,
                _ => null
            };
        }
        return null;
    }

    public static string? GetNoteEditor(string songName, string difficulty)
    {
        return (from song in songData.songs
            where song.title == songName
            select song.noteDesiner
            into noteDesigner
            select difficulty switch
            {
                "Easy" => noteDesigner.easyDesiner,
                "Hard" => noteDesigner.hardDesiner,
                "Extreme" => noteDesigner.extremeDesiner,
                "Kujo" => noteDesigner.kujoDesiner,
                _ => null
            }).FirstOrDefault();
    }

    public static string? GetComposer(string songName)
    {
        return (from song in songData.songs where song.title == songName select song.composer).FirstOrDefault();
    }
}
