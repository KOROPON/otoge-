#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Rhythmium;
using UnityEngine;
using Reilas;
using System;
using UnityEngine.UI;

public sealed class RhythmGamePresenter : MonoBehaviour
{
    public Text text1;
    public Text text2;

    [SerializeField] private TapNote _tapNotePrefab = null!;
    [SerializeField] private HoldNote _holdNotePrefab = null!;
    [SerializeField] private AboveTapNote _aboveTapNotePrefab = null!;
    [SerializeField] private AboveChainNote _aboveChainNotePrefab = null!;
    [SerializeField] private AboveSlideNote _aboveSlideNotePrefab = null!;

    [SerializeField] private AudioSource _audioSource = null!;

    public static List<TapNote> _tapNotes = new List<TapNote>();
    public static List<AboveTapNote> _aboveTapNotes = new List<AboveTapNote>();
    public static List<AboveChainNote> _aboveChainNotes = new List<AboveChainNote>();
    public static List<HoldNote> _holdNoteLines = new List<HoldNote>();
    public static List<AboveSlideNote> _aboveSlideNotes = new List<AboveSlideNote>();

    private ReilasChartEntity _chartEntity = null!;

    public static string musicname = null!;
    public static string dif = null!;


    /// <summary>
    /// 判定結果を処理する
    /// </summary>

    private void Awake()
    {
        AwakeAsync().Forget();
    }

    private async UniTask AwakeAsync()
    {
        FindObjectOfType<Variable>().enabled = false;

        var chartTextAsset = dif != null ? await Resources.LoadAsync<TextAsset>("Charts/" + musicname + "." + dif) as TextAsset : await Resources.LoadAsync<TextAsset>("Charts/" + musicname + ".Easy") as TextAsset;

        if (chartTextAsset == null)
        {
            Debug.LogError("譜面データが見つかりませんでした");
            return;
        }


        var chartJsonData = JsonUtility.FromJson<ChartJsonData>(chartTextAsset.text);
        var chartEntity = new ReilasChartConverter().Convert(chartJsonData);
        
        notJudgedNotes = chartEntity.Notes;
        notes = chartEntity.Notes;
        notJudgedNotes.OrderBy(notes => notes.JudgeTime);

        Debug.Log("最大コンボ数: " + chartEntity.Notes.Count);

        NoteLineJsonData[] noteJsonData = chartJsonData.timeline.noteLines;

        var audioClipPath = "Songs/Songs/" + Path.GetFileNameWithoutExtension(chartJsonData.audioSource);
        var audioClip = await Resources.LoadAsync<AudioClip>(audioClipPath) as AudioClip;

        _audioSource.clip = audioClip;

        if (PlayerPrefs.HasKey("volume"))
        {
        // tap音調整
        }

        _audioSource.Play();

        // chartEntity
        _chartEntity = chartEntity;

        SpawnTapNotes(chartEntity.Notes.Where(note => note.Type == NoteType.Tap));
        SpawnChainNotes(chartEntity.Notes.Where(note => note.Type == NoteType.AboveChain));
        SpawnHoldNotes(chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.Hold));
        SpawnAboveTapNotes(chartEntity.Notes.Where(note => note.Type == NoteType.AboveTap));
        SpawnAboveSlideNotes(chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveSlide));



        for (int i = 0; i < notJudgedNotes.Count; i++)
        {
            ReilasNoteEntity reilasNoteEntity = notJudgedNotes[i];

            if (reilasNoteEntity.Type == NoteType.AboveSlide)
            {
                for(int k = 0; k < noteJsonData.Length; k++)
                {
                    if (reilasNoteEntity.JsonData.guid == noteJsonData[k].tail)
                    {
                        notJudgedNotes[i].Type = NoteType.AboveSlideInternal;
                        break;
                    }
                    else
                    {

                    }
                }
            }
            else if(reilasNoteEntity.Type == NoteType.AboveHold || reilasNoteEntity.Type == NoteType.Hold)
            {
                for (int k = 0; k < noteJsonData.Length; k++)
                {

                    if (reilasNoteEntity.JsonData.guid == noteJsonData[k].tail)
                    {
                        notJudgedNotes[i].Type = NoteType.AboveSlideInternal;
                        break;
                    }
                    else
                    {

                    }
                }
            }
        }
    }

    private void SpawnTapNotes(IEnumerable<ReilasNoteEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(_tapNotePrefab);
            tapNote.transform.position = new Vector3(0, 0, 999);
            tapNote.Initialize(note);
            _tapNotes.Add(tapNote);
        }
    }

    private void SpawnAboveTapNotes(IEnumerable<ReilasNoteEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(_aboveTapNotePrefab);
            tapNote.Initialize(note);
            _aboveTapNotes.Add(tapNote);
        }
    }

    private void SpawnChainNotes(IEnumerable<ReilasNoteEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(_aboveChainNotePrefab);
            tapNote.Initialize(note);
            _aboveChainNotes.Add(tapNote);
        }
    }

    private void SpawnHoldNotes(IEnumerable<ReilasNoteLineEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(_holdNotePrefab);
            tapNote.Initialize(note);
            _holdNoteLines.Add(tapNote);
        }
    }

    private void SpawnAboveSlideNotes(IEnumerable<ReilasNoteLineEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(_aboveSlideNotePrefab);
            tapNote.Initialize(note);
            _aboveSlideNotes.Add(tapNote);
        }
    }

    // 譜面情報に存在してる
    List<ReilasNoteEntity> notes = new List<ReilasNoteEntity>();

    // まだ判定されていないノーツ
    List<ReilasNoteEntity> notJudgedNotes = new List<ReilasNoteEntity>();


    static Vector3[] lanePositions = new Vector3[]
    {
        //上のレーン
        new Vector3(-2.5f, 0, 0),
        new Vector3(-1.25f, 0, 0),
        new Vector3(1.25f, 0, 0),
        new Vector3(2.5f, 0, 0),

        //下のレーン
        new Vector3(4.5f,0.3f,0),
        new Vector3(4.3f,1.3f,0),
        new Vector3(4f,2.3f,0),
        new Vector3(3.5f,3.2f,0),
        new Vector3(2.9f,3.95f,0),
        new Vector3(2.1f,4.5f,0),
        new Vector3(1.3f,4.9f,0),
        new Vector3(0.5f,5.1f,0),
        new Vector3(-0.5f,5.1f,0),
        new Vector3(-1.3f,4.9f,0),
        new Vector3(-2.1f,4.5f,0),
        new Vector3(-2.9f,3.95f,0),
        new Vector3(-3.5f,3.2f,0),
        new Vector3(-4f,2.3f,0),
        new Vector3(-4.3f,1.3f,0),
        new Vector3(-4.5f,0.3f,0),
    };

     IEnumerable<Vector3> screenPoints = lanePositions.Select(lanePosition3D => Camera.main.WorldToScreenPoint(lanePosition3D));// Camera.main.WorldToScreenPoint(lanePosition3D))  "レーンの位置を"2D変換  //

    private void Update()
    {
        text1.text = JudgeService.aa;

        InputService.aboveLaneTapStates.Clear();

        var alltouch = Input.touches;
        Array.Resize(ref alltouch,0);
        var touches = Input.touches;
        text2.text = touches.Count().ToString();

        foreach (var touch in touches)
        {
            var nearestLaneIndex = screenPoints.Select((screenPoint, index) => (screenPoint, index)).OrderBy(screenPoint => Vector2.Distance(screenPoint.screenPoint, touch.position)).First().index;//押した場所に一番近いレーンの番号
            //Debug.Log(nearestLaneIndex);
            bool end = false;
            bool start = false;
            // touch.position
            // このフレームで押されたよん
            if (touch.phase == TouchPhase.Began)
            {
                start = true;
                //touch.phase = false;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                end = true;
            }

            InputService.aboveLaneTapStates.Add(new LaneTapState
            {
                laneNumber = nearestLaneIndex,
                IsHold = true,
                TapStating = start,
                tapEnding = end
            });
        }


        var currentTime = _audioSource.time - _chartEntity.StartTime;
        var judgeTime = currentTime;
        var audioTime = currentTime;
        if (PlayerPrefs.HasKey("judgegap"))
        {
          judgeTime += PlayerPrefs.GetFloat("judgegap") / 1000;
        }
        if (PlayerPrefs.HasKey("audiogap"))
        {
          audioTime += PlayerPrefs.GetFloat("audiogap") / 1000;
        }



        var orderedNotes = notes.OrderBy(note => note.JudgeTime);

        Debug.Log("");
        //var judgeService = new JudgeService();
        foreach(var a in InputService.aboveLaneTapStates)
        {
            Debug.Log(a.laneNumber + "," + a.IsHold) ;
        }
        JudgeService.Judge(notJudgedNotes, _audioSource.time,InputService.aboveLaneTapStates);


        var _aboveNearestTap = _aboveTapNotes.Where(note => note.aboveTapTime - currentTime < 5f);
        var _tapNote = _tapNotes.Where(note => note._tapTime - currentTime < 5f);

        foreach (var tapNote in _tapNote)
        {
            tapNote.Render(audioTime);
        }

        foreach (var note in _aboveNearestTap)
        {
            note.Render(audioTime);
        }

        foreach (var note in _holdNoteLines)
        {
            note.Render(audioTime);
        }

        foreach (var note in _aboveSlideNotes)
        {
            note.Render(audioTime);
        }
    }
}

// レーンの押されている状態
public class LaneTapState
{
    //レーンの番号
    public int laneNumber;
    // 今のフレーム押されているか
    public bool IsHold;

    // このフレームにタップしたか
    public bool TapStating;

    //このフレームで指を離したか
    public bool tapEnding;
}

public class InputService
{
    // 上レーン 最大 36 個
    public static List<LaneTapState> aboveLaneTapStates = new List<LaneTapState>();

    // 下レーン 最大 4 つ
    //public static List<LaneTapState> LaneTapStates = new List<LaneTapState>();
}
