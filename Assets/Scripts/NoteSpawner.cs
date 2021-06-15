using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//[ExecuteInEditMode]
public class NoteSpawner : MonoBehaviour {
  public GameObject tapPrefab = null;
  public GameObject holdPrefab = null;
  public float far;
  private Song song;
  public bool wait;

  public float spd;// = Variable.speed;
  public float localbpm;// = Variable.bpm;

  private static float channelWidth = 2.4f;
  private static float channelOffset = channelWidth * 3f / 2f;

  [Tooltip("譜面作成の時はユーザーのフォルダーからロードする")]
  public bool loadFromUserFolder = false;
  [Tooltip("ゲーム自体のプレイ画面用のテキストアセット")]
  public TextAsset songFile;
  [Tooltip("譜面作成のユーザーがつけたファイル名")]
  public string songFileName = "";

  private string songPath {
    get {
      if (songFileName == "") {
        return "";
      }
      return Application.persistentDataPath + "/" + songFileName;
    }
  }

  private string jsonString {
    get {
      if (loadFromUserFolder) {
        using (StreamReader reader = new StreamReader(songPath)) {
          return reader.ReadToEnd();
        }
      }
      return songFile.text;
    }
  }

  public float zScale {
    get {
      //return spd * -10f;
      return -60f * spd;    //1000 * spd / bpm * Json�̐��l = z
    }
  }

  private static float getChannelX(int channel) {
    return -channel * channelWidth + channelOffset;
  }



  void Awake() {
    spd = Variable.speed;
    localbpm = Variable.bpm;
    while(transform.childCount > 0) {
      Transform child = transform.GetChild(0);
      if (Application.isPlaying) {
        Destroy(child.gameObject);
        child.SetParent(null);
      } else {
        DestroyImmediate(child.gameObject);
      }
    }

    song = JsonUtility.FromJson<Song>(jsonString);
    foreach (Tap tap in song.taps) {
      GameObject obj = Instantiate(tapPrefab, new Vector3(getChannelX(tap.channel), -0.5f, tap.start * zScale), Quaternion.identity, transform);
      obj.transform.localScale = new Vector3(2.4f,obj.transform.localScale.y,transform.localScale.z);
      obj.transform.GetChild(0).localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, obj.transform.localScale.z + 10 * spd);
    }
    foreach (Hold hold in song.holds) {
      float zLength = (hold.end - hold.start) * -zScale;
      GameObject obj = Instantiate(holdPrefab, new Vector3(getChannelX(hold.channel), -0.5f, hold.start * zScale), Quaternion.identity, transform);
      obj.transform.localScale = new Vector3(2.4f, obj.transform.localScale.y, zLength);
      obj.transform.GetChild(0).localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, 1 + 1 / zLength * 10 * spd);
    }
  }

  //[ContextMenu("Save File")]
  //void SaveFile() {

  //}

  public void TapSpawn() {

  }
  public void Holdspawn() {

  }

  private void Update() {
    if(wait) {
      far += spd * Time.deltaTime;
    }
  }
}
