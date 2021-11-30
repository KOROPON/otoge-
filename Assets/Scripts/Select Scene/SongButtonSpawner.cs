using UnityEngine;
using UnityEngine.UI;

public class SongButtonSpawner : MonoBehaviour
{
    [SerializeField]private GameObject songPrefab;
    
    private SongDataBase _level;
    private Transform _content;
    
    public void SpawnSongs(bool kujo)
    {
        var gameObject = GameObject.Find("Reilas");
        if (gameObject != null)
        {
            foreach (Transform songOb in GameObject.Find("Content").transform)
            {
                GameObject.Destroy(songOb.gameObject);
            }
        }
        _level = LevelConverter.songData;
        _content = GameObject.Find("Content").transform;
        for (int i = 0; i < _level.songs.Length; i++)
        {
            SongName song = _level.songs[i];

            if (kujo && song.title != "Reilas") continue;

            GameObject songButton = Instantiate(songPrefab, _content);
            songButton.transform.localPosition = new Vector3(400, -175 - 250 * (i - 1), 0);
            songButton.name = song.title;
            for (int j = 0; j < songButton.transform.childCount; j++)
            {
                Transform songName = songButton.transform.GetChild(j);
                if (songName.name == "SongName")
                {
                    songName.gameObject.GetComponent<Text>().text = song.title;
                }
            }
        }
    }
}
