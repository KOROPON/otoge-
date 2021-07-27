using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class NoteSpawner : MonoBehaviour
{
    public GameObject tapPrefab = null;
    public GameObject holdPrefab = null;
    public float far;
    private Song song;
    public bool wait;

    [Tooltip("trueならばPlayNoteを起動")]
    public bool togglePlayNote = true;

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

    private string songPath
    {
        get
        {
            if (songFileName == "")
            {
                return "";
            }
            return Application.persistentDataPath + "/" + songFileName;
        }
    }

    private string jsonString
    {
        get
        {
            if (loadFromUserFolder)
            {
                using (StreamReader reader = new StreamReader(songPath))
                {
                    return reader.ReadToEnd();
                }
            }
            return songFile.text;
        }
    }

    public float zScale
    {
        get
        {
            return -60f * spd;
        }
    }

    public static float getChannelX(int channel)
    {
        return -channel * channelWidth + channelOffset;
    }

    public static int getChannelFromX(float x)
    {
        return (int)Mathf.Round(Mathf.Clamp(-(x - channelOffset) / channelWidth, 0f, 3f));
    }

    void Awake()
    {
        spd = Variable.speed;
        localbpm = Variable.bpm;
        while(transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
                child.SetParent(null);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }

        song = JsonUtility.FromJson<Song>(jsonString);
        foreach (Tap tap in song.taps)
        {
            GameObject obj = Instantiate(tapPrefab, transform, false);
            obj.transform.localPosition = new Vector3(getChannelX(tap.channel), -0.5f, tap.start * zScale);
            obj.transform.localScale = new Vector3(2.4f, obj.transform.localScale.y, obj.transform.localScale.z);
            obj.transform.GetChild(0).localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, obj.transform.localScale.z + 10 * spd);
            obj.GetComponent<PlayNote>().togglePlayNote = togglePlayNote;
        }
        foreach (Hold hold in song.holds)
        {
            float zLength = (hold.end - hold.start) * -zScale;
            GameObject obj = Instantiate(holdPrefab, transform, false);
            obj.transform.localPosition = new Vector3(getChannelX(hold.channel), -0.5f, hold.start * zScale);
            obj.transform.localScale = new Vector3(2.4f, obj.transform.localScale.y, zLength);
            obj.transform.GetChild(0).localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, 1 + 1 / zLength * 10 * spd);
            obj.GetComponent<PlayNote>().togglePlayNote = togglePlayNote;
        }
    }

    [ContextMenu("Save File")]
    void SaveFile()
    {
        Song song = new Song();
        TapComponent[] tcs = GetComponentsInChildren<TapComponent>();
        song.taps = new Tap[tcs.Length];
        for (int i = 0; i < tcs.Length; i++)
        {
            Tap tap = new Tap();
            tap.channel = tcs[i].channel;
            tap.start = tcs[i].start;
            song.taps[i] = tap;
        }
        HoldComponents[] hcs = GetComponentsInChildren<HoldComponents>();
        song.holds = new Hold[hcs.Length];
        for (int i = 0; i < hcs.Length; i++)
        {
            Hold hold = new Hold();
            hold.channel = hcs[i].channel;
            hold.start = hcs[i].start;
            song.holds[i] = hold;
        }
        using (StreamWriter writer = new StreamWriter(songPath))
        {
            writer.WriteLine(JsonUtility.ToJson(song, true));
        }
    }

    public void TapSpawn()
    {
    }

    public void Holdspawn()
    {
    }

    private void Update()
    {
        if (wait)
        {
            far += spd * Time.deltaTime;
        }
    }
}
