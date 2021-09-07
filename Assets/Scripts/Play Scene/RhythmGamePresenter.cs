#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Rhythmium;
using UnityEngine;
using Reilas;


public sealed class RhythmGamePresenter : MonoBehaviour
{
    [SerializeField] private TapNote _tapNotePrefab = null!;
    [SerializeField] private HoldNote _holdNotePrefab = null!;
    [SerializeField] private AboveTapNote _aboveTapNotePrefab = null!;
    [SerializeField] private AboveSlideNote _aboveSlideNotePrefab = null!;

    [SerializeField] private AudioSource _audioSource = null!;

    private readonly List<TapNote> _tapNotes = new List<TapNote>();
    private readonly List<AboveTapNote> _aboveTapNotes = new List<AboveTapNote>();
    private readonly List<HoldNote> _holdNoteLines = new List<HoldNote>();
    private readonly List<AboveSlideNote> _aboveSlideNotes = new List<AboveSlideNote>();

    private ReilasChartEntity _chartEntity = null!;

    public int CurrentCombo;
    public static string musicname = null!;

    private string dif = MusicNumManage.difficulty;

    /// <summary>
    /// 判定結果を処理する
    /// </summary>
    public void HandleJudgeResult(JudgeResultType judgeResultType)
    {

    }

    public void OnAddCombo()
    {
    }

    private void Awake()
    {
        AwakeAsync().Forget();
    }

    private async UniTask AwakeAsync()
    {
        FindObjectOfType<Variable>().enabled = false;

        var chartTextAsset = dif != null ? await Resources.LoadAsync<TextAsset>("Charts/" + musicname + "." + dif) as TextAsset : await Resources.LoadAsync<TextAsset>("Charts/" + musicname + ".Hard") as TextAsset;

        if (chartTextAsset == null)
        {
            Debug.LogError("譜面データが見つかりませんでした");
            return;
        }

        var chartJsonData = JsonUtility.FromJson<ChartJsonData>(chartTextAsset.text);
        var chartEntity = new ReilasChartConverter().Convert(chartJsonData);
        var noteJsonDeta = JsonUtility.FromJson<NoteJsonData>(chartTextAsset.text);
        var timeLineJsonData = JsonUtility.FromJson<TimelineJsonData>(chartTextAsset.text);


        notJudgedNotes = chartEntity.Notes;
        notes = chartEntity.Notes;

        foreach(var note in chartEntity.Notes)
        {
            Debug.Log(note.Type + "," + note.LanePosition + " " + note.Size);
        }

        //Debug.Log();

        Debug.Log("最大コンボ数: " + chartEntity.Notes.Count);

        /*
        foreach (var noteJsonData in timeLineJsonData.notes)
        {

        }
        */

        var audioClipPath = "Songs/Songs/" + Path.GetFileNameWithoutExtension(chartJsonData.audioSource);
        var audioClip = await Resources.LoadAsync<AudioClip>(audioClipPath) as AudioClip;

        _audioSource.clip = audioClip;

        _audioSource.Play();

        // chartEntity
        _chartEntity = chartEntity;

        SpawnTapNotes(chartEntity.Notes.Where(note => note.Type == NoteType.Tap));
        SpawnHoldNotes(chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.Hold));
        SpawnAboveTapNotes(chartEntity.Notes.Where(note => note.Type == NoteType.AboveTap));
        SpawnAboveSlideNotes(chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveSlide));
    }

    private void SpawnTapNotes(IEnumerable<ReilasNoteEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(_tapNotePrefab);
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
        

        var touches = Input.touches;


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



        var orderedNotes = notes.OrderBy(note => note.JudgeTime);


        //var judgeService = new JudgeService();


        JudgeService.Judge(notJudgedNotes, currentTime,InputService.aboveLaneTapStates);


        foreach (var tapNote in _tapNotes)
        {
            tapNote.Render(currentTime);
        }

        foreach (var note in _aboveTapNotes)
        {
            note.Render(currentTime);
        }

        foreach (var note in _holdNoteLines)
        {
            note.Render(currentTime);
        }

        foreach (var note in _aboveSlideNotes)
        {
            note.Render(currentTime);
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