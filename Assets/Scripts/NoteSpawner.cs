using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour {
  public GameObject tapPrefab = null;
  public GameObject holdPrefab = null;
  public TextAsset songFile;

  private Song song;

  private static float channelWidth = 10f / 4f;
  private static float channelOffset = channelWidth * 3f / 2f;

  public float spd;

  private static float getChannelX(int channel) {
    return -channel * channelWidth + channelOffset;
  }


  void Start() {
    spd = GameObject.Find("Run Programs").GetComponent<Variable>().speed;
    song = JsonUtility.FromJson<Song>(songFile.text);
    float zScale = -10f;
    foreach (Tap tap in song.taps) {
      Instantiate(tapPrefab, new Vector3(getChannelX(tap.channel), -0.5f, tap.start * zScale - 2), Quaternion.identity);
    }
    foreach (Hold hold in song.holds) {
      float zLength = (hold.end - hold.start) * -zScale;
      GameObject obj = Instantiate(holdPrefab, new Vector3(getChannelX(hold.channel), -0.5f, hold.start * zScale - zLength / 2), Quaternion.identity);
      obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, zLength);
    }
  }

}
