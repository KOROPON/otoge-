using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongButtonSpawner : MonoBehaviour
{
    [SerializeField] private GameObject songPrefab;
    
    private SongDataBase _level;
    private Transform _content;
    
    public void SpawnSongs(bool kujo, List<TitleAndLevel> titleAndLevels)
    {
        var o = GameObject.Find("Reilas");
        
        if (o != null) foreach (Transform songOb in GameObject.Find("Content").transform) Destroy(songOb.gameObject);
        
        _content = GameObject.Find("Content").transform;
        
        for (var i = 0; i < titleAndLevels.Count; i++)
        {
            var song = titleAndLevels[i];

            var songButton = Instantiate(songPrefab, _content);

            if (kujo) songButton.transform.localPosition = new Vector3(400, -175 - 250 * 2, 0);
            else songButton.transform.localPosition = new Vector3(400, -175 - 250 * (i - 1), 0);
            
            songButton.name = song.title;
            
            for (var j = 0; j < songButton.transform.childCount; j++)
            {
                var songName = songButton.transform.GetChild(j);
                if (songName.name == "SongName") songName.gameObject.GetComponent<Text>().text = song.title;
            }
        }
    }
}
