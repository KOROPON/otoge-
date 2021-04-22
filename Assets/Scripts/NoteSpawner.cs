using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour {
  public GameObject tapPrefab = null;
  public GameObject holdPrefab = null;
  public TextAsset songFile;

  private Song song;

  void Start() {
    song = JsonUtility.FromJson<Song>(songFile.text);
    foreach (Tap tap in song.taps) {
      Instantiate(tapPrefab, new Vector3(tap.channel + 4, -0.5f, tap.start), Quaternion.identity);
    }
    foreach (Hold hold in song.holds) {
      GameObject obj = Instantiate(holdPrefab, new Vector3(hold.channel + 4, -0.5f, hold.start), Quaternion.identity);
      obj.transform.localScale = new Vector3(1, 1, hold.end - hold.start);
    }
  }
}
