using UnityEngine;
using UnityEngine.UI;

public class LevelConverter : MonoBehaviour
{
    private Level _level;
    private TextAsset _jsonFile;
    
    public Text easyLevel;
    public Text hardLevel;
    public Text extremeLevel;

    

    void Start()
    {
        _jsonFile = Resources.Load<TextAsset>("Level/level");
        _level = JsonUtility.FromJson<Level>(_jsonFile.text);
    }

    public void GetLevel(string songName)
    {
        foreach (SongName song in _level.songs)
        {
            if (song.title == songName)
            {
                easyLevel.text = song.Easy.ToString();
                hardLevel.text = song.Hard.ToString();
                extremeLevel.text = song.Extreme.ToString();
            }
        }
    }
}
