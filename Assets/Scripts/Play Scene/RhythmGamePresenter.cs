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
    public AudioSource songAudio;

    [SerializeField] private TapNote _tapNotePrefab = null!;
    [SerializeField] private HoldNote _holdNotePrefab = null!;
    [SerializeField] private AboveTapNote _aboveTapNotePrefab = null!;
    [SerializeField] private AboveChainNote _aboveChainNotePrefab = null!;
    [SerializeField] private AboveHoldNote _aboveHoldNotePrefab = null!;
    [SerializeField] private AboveSlideNote _aboveSlideNotePrefab = null!;


    [SerializeField] private static AudioSource _audioSource = null!;

    public static List<TapNote> _tapNotes = new List<TapNote>();
    public static List<AboveTapNote> _aboveTapNotes = new List<AboveTapNote>();
    public static List<AboveChainNote> _aboveChainNotes = new List<AboveChainNote>();
    public static List<HoldNote> _holdNotes = new List<HoldNote>();
    public static List<AboveHoldNote> _aboveHoldNotes = new List<AboveHoldNote>();
    public static List<AboveSlideNote> _aboveSlideNotes = new List<AboveSlideNote>();

    //Judge用
    public static List<List<float>> notJudgedTapNotes = new List<List<float>>();
    public static List<List<float>> notJudgedAboveTapNotes = new List<List<float>>();
    public static List<List<float>> notJudgedHoldNotes = new List<List<float>>();
    public static List<List<float>> notJudgedAboveHoldNotes = new List<List<float>>();
    public static List<List<float>> notJudgedAboveSlideNotes = new List<List<float>>();
    public static List<List<float>> notJudgedAboveChainNotes = new List<List<float>>();
    public static List<List<float>> notJudgedInternalNotes = new List<List<float>>();
    public static List<List<float>> notJudgedAboveInternalNotes = new List<List<float>>();


    private ReilasChartEntity _chartEntity = null!;

    public static string musicname = null!;
    public static string dif = null!;

    JudgeService judgeService;

    float judgeTime;
    float audioTime;
    /// <summary>
    /// 判定結果を処理する
    /// </summary>

    private void Awake()
    {
        judgeService = new JudgeService();
        AwakeAsync().Forget();
    }

    private async UniTask AwakeAsync()
    {
        FindObjectOfType<Variable>().enabled = false;

        var chartTextAsset = await Resources.LoadAsync<TextAsset>("Charts/" + musicname + "." + dif) as TextAsset;

        if (chartTextAsset == null)
        {
            Debug.LogError("譜面データが見つかりませんでした");
            return;
        }


        var chartJsonData = JsonUtility.FromJson<ChartJsonData>(chartTextAsset.text);
        var chartEntity = new ReilasChartConverter().Convert(chartJsonData);

        notJudgedNotes = chartEntity.Notes;
        notJudgedNotes.OrderBy(notes => notes.JudgeTime);
        


        Debug.Log("最大コンボ数: " + notJudgedNotes.Count);

        NoteLineJsonData[] noteJsonData = chartJsonData.timeline.noteLines;

        var audioClipPath = "Songs/Songs/" + Path.GetFileNameWithoutExtension(chartJsonData.audioSource);
        var audioClip = await Resources.LoadAsync<AudioClip>(audioClipPath) as AudioClip;
        _audioSource = songAudio;
        _audioSource.clip = audioClip;

        if (PlayerPrefs.HasKey("volume"))
        {
        // tap音調整
        }



        // chartEntity
        _chartEntity = chartEntity;

        SpawnTapNotes(chartEntity.Notes.Where(note => note.Type == NoteType.Tap));
        SpawnChainNotes(chartEntity.Notes.Where(note => note.Type == NoteType.AboveChain));
        SpawnHoldNotes(chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.Hold));
        SpawnAboveTapNotes(chartEntity.Notes.Where(note => note.Type == NoteType.AboveTap));
        SpawnAboveHoldNotes(chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveHold));
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
        foreach (ReilasNoteEntity reilasNoteEntity in notJudgedNotes)
        {
            switch (reilasNoteEntity)
            {
                case ReilasNoteEntity notes when notes.Type == NoteType.Tap: notJudgedTapNotes.Add(new List<float>() { reilasNoteEntity.JudgeTime, reilasNoteEntity.LanePosition, reilasNoteEntity.Size }); break;
                case ReilasNoteEntity notes when notes.Type == NoteType.AboveTap: notJudgedAboveTapNotes.Add(new List<float>() { reilasNoteEntity.JudgeTime, reilasNoteEntity.LanePosition, reilasNoteEntity.Size }); break;
                case ReilasNoteEntity notes when notes.Type == NoteType.Hold: notJudgedHoldNotes.Add(new List<float>() { reilasNoteEntity.JudgeTime, reilasNoteEntity.LanePosition, reilasNoteEntity.Size }); break;
                case ReilasNoteEntity notes when notes.Type == NoteType.AboveHold: notJudgedAboveHoldNotes.Add(new List<float>() { reilasNoteEntity.JudgeTime, reilasNoteEntity.LanePosition, reilasNoteEntity.Size }); break;
                case ReilasNoteEntity notes when notes.Type == NoteType.AboveSlide: notJudgedAboveSlideNotes.Add(new List<float>() { reilasNoteEntity.JudgeTime, reilasNoteEntity.LanePosition, reilasNoteEntity.Size }); break;
                case ReilasNoteEntity notes when notes.Type == NoteType.AboveChain: notJudgedAboveChainNotes.Add(new List<float>() { reilasNoteEntity.JudgeTime, reilasNoteEntity.LanePosition, reilasNoteEntity.Size }); break;
                case ReilasNoteEntity notes when notes.Type == NoteType.AboveHoldInternal || notes.Type == NoteType.AboveSlideInternal: notJudgedAboveInternalNotes.Add(new List<float>() { reilasNoteEntity.JudgeTime, reilasNoteEntity.LanePosition, reilasNoteEntity.Size }); break;
                case ReilasNoteEntity notes when notes.Type == NoteType.HoldInternal: notJudgedInternalNotes.Add(new List<float>() { reilasNoteEntity.JudgeTime, reilasNoteEntity.LanePosition, reilasNoteEntity.Size }); break;
            }
        }

        Shutter.blChange = "Open";//シーンを開く
    }

    public static void PlaySongs()
    {
      _audioSource.Play();
    }

    private void SpawnTapNotes(IEnumerable<ReilasNoteEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(_tapNotePrefab);
            tapNote.Initialize(note);
            tapNote.transform.position = new Vector3(transform.position.x, transform.position.y, 999);
            _tapNotes.Add(tapNote);
            tapNote.gameObject.SetActive(false);
        }
    }

    private void SpawnAboveTapNotes(IEnumerable<ReilasNoteEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(_aboveTapNotePrefab);
            tapNote.Initialize(note);
            //tapNote.transform.position = new Vector3(transform.position.x, transform.position.y, 999);
            _aboveTapNotes.Add(tapNote);
            tapNote.gameObject.SetActive(false);
        }
    }

    private void SpawnChainNotes(IEnumerable<ReilasNoteEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(_aboveChainNotePrefab);
            tapNote.Initialize(note);
            //tapNote.transform.position = new Vector3(transform.position.x, transform.position.y, 999);
            _aboveChainNotes.Add(tapNote);
            tapNote.gameObject.SetActive(false);
        }
    }

    private void SpawnHoldNotes(IEnumerable<ReilasNoteLineEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(_holdNotePrefab);
            tapNote.Initialize(note);
            _holdNotes.Add(tapNote);
        }
    }

    private void SpawnAboveHoldNotes(IEnumerable<ReilasNoteLineEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(_aboveHoldNotePrefab);
            tapNote.Initialize(note);
            _aboveHoldNotes.Add(tapNote);
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

    // 譜面情報に存在してるまだ判定されていないノーツ
    public static List<ReilasNoteEntity> notJudgedNotes = new List<ReilasNoteEntity>();


    static Vector3[] lanePositions = new Vector3[]
    {
        //上のレーン
        new Vector3(3f, 0, 0),
        new Vector3(1.25f, 0, 0),
        new Vector3(-1.25f, 0, 0),
        new Vector3(-3f, 0, 0),

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
        if (_audioSource == null)
        {
          return;
        }

        InputService.aboveLaneTapStates.Clear();

        var alltouch = Input.touches;
        Array.Resize(ref alltouch,0);
        var touches = Input.touches;
        text2.text = touches.Count().ToString();

        foreach (var touch in touches)
        {
            var nearestLaneIndex = screenPoints.Select((screenPoint, index) => (screenPoint, index)).OrderBy(screenPoint => Vector2.Distance(screenPoint.screenPoint, touch.position)).First().index;//押した場所に一番近いレーンの番号
            //Debug.Log(nearestLaneIndex);
            bool start = false;
            // touch.position
            // このフレームで押されたよん
            if (touch.phase == TouchPhase.Began)
            {
                start = true;
            }

            InputService.aboveLaneTapStates.Add(new LaneTapState
            {
                laneNumber = nearestLaneIndex, tapStarting = start
            });
        }


        var currentTime = _audioSource.time;
        judgeTime = currentTime;
        audioTime = currentTime;
        if (PlayerPrefs.HasKey("judgegap"))
        {
          judgeTime += PlayerPrefs.GetFloat("judgegap") / 1000;
        }
        if (PlayerPrefs.HasKey("audiogap"))
        {
          audioTime += PlayerPrefs.GetFloat("audiogap") / 1000;
        }



       

        //var judgeService = new JudgeService();
        judgeService.Judge(judgeTime, InputService.aboveLaneTapStates);

    }

    void LateUpdate()
    {
        var _aboveNearestTap = _aboveTapNotes.Where(note => note.aboveTapTime - audioTime < 5f);
        //Debug.Log(_tapNotes.Count());
        var _tapNote = _tapNotes.Where(note => note._tapTime - audioTime < 5f);
        var _chainNote = _aboveChainNotes.Where(note => note.aboveChainTime - audioTime < 5f);

        foreach (var tapNote in _tapNote)
        {
            tapNote.Render(audioTime);
        }

        foreach (var note in _aboveNearestTap)
        {
            note.Render(audioTime);
        }

        foreach (var note in _chainNote)
        {
            note.Render(audioTime);
        }

        foreach (var note in _holdNotes)
        {
            note.Render(audioTime);
        }

        foreach (var note in _aboveHoldNotes)
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

    // このフレームにタップしたか
    public bool tapStarting;
}

public class InputService
{
    // 上レーン 最大 36 個
    public static List<LaneTapState> aboveLaneTapStates = new List<LaneTapState>();

    // 下レーン 最大 4 つ
    //public static List<LaneTapState> LaneTapStates = new List<LaneTapState>();
}
