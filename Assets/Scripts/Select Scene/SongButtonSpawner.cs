using UnityEngine;
using UnityEngine.UI;

public class SongButtonSpawner : MonoBehaviour
{
    [SerializeField] private GameObject songPrefab;
    
    private SongDataBase _level;
    private Transform _content;
    
    public void SpawnSongs()
    {
        _level = LevelConverter.songData;
        _content = GameObject.Find("Content").transform;
        for (var i = 0; i < _level.songs.Length; i++)
        {
            var song = _level.songs[i];
            var songButton = Instantiate(songPrefab, _content);
            songButton.transform.localPosition = new Vector3(400, -175 - 250 * (i - 1), 0);
            songButton.name = song.title;
            for (var j = 0; j < songButton.transform.childCount; j++)
            {
                var songName = songButton.transform.GetChild(j);
                if (songName.name == "SongName")
                {
                    songName.gameObject.GetComponent<Text>().text = song.title;
                }
            }
        }
    }
}
