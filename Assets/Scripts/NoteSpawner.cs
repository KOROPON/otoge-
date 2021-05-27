using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NoteSpawner : MonoBehaviour {
  public GameObject tapPrefab = null;
  public GameObject holdPrefab = null;
  public TextAsset songFile;

  private Song song;

  private static float channelWidth = 10f / 4f;
  private static float channelOffset = channelWidth * 3f / 2f;

  public float spd = 1f;

  public float zScale {
    get {
      return spd * -10f;
    }
  }

  private static float getChannelX(int channel) {
    return -channel * channelWidth + channelOffset;
  }


  void Start() {
    while(transform.childCount > 0) {
      Transform child = transform.GetChild(0);
      if (Application.isPlaying) {
        Destroy(child.gameObject);
        child.SetParent(null);
      } else {
        DestroyImmediate(child.gameObject);
      }
    }

    song = JsonUtility.FromJson<Song>(songFile.text);
    foreach (Tap tap in song.taps) {
      Debug.Log(tap);
      Instantiate(tapPrefab, new Vector3(getChannelX(tap.channel), -0.5f, tap.start * zScale), Quaternion.identity, transform);
    }
    foreach (Hold hold in song.holds) {
      float zLength = (hold.end - hold.start) * -zScale;
      GameObject obj = Instantiate(holdPrefab, new Vector3(getChannelX(hold.channel), -0.5f, hold.start * zScale - zLength / 2), Quaternion.identity, transform);
      obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, zLength);
    }
  }

  [ContextMenu("Save File")]
  void SaveFile() {

  }
}
