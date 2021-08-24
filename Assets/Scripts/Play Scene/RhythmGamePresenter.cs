#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Rhythmium;
using UnityEngine;
using Reilas;

/*

public enum JudgeResultType
{
    Perfect,
    Good,
    Bad,
    Miss
}

/// <summary>
/// 判定結果
/// </summary>
public class JudgeResult
{
    public JudgeResultType ResultType;
}

/// <summary>
/// 判定を行うサービス
/// </summary>
public class JudgeService
{
    public static List<JudgeResultType> allJudgeType = new List<JudgeResultType>();

    private readonly InputService _inputService = new InputService();

    private List<JudgeResult> _result = new List<JudgeResult>(10);

    public List<JudgeResult> Judge(List<ReilasNoteEntity> notJudgedNotes, float currentTime)
    {
        var lanePositions = new Vector3[]
        {
                new Vector3(-5f, 0, 0),
                new Vector3(-2.5f, 0, 0),
                new Vector3(2.5f, 0, 0),
                new Vector3(2.5f, 0, 0),
        };

        var screenPoints = lanePositions.Select(lanePosition3D => Camera.main.WorldToScreenPoint(lanePosition3D));

        var touches = Input.touches;
        foreach (var touch in touches)
        {
            var 一番近いレーンのインデックス = screenPoints
                .Select((screenPoint, index) => (screenPoint, index))
                .OrderBy(screenPoint => Vector2.Distance(screenPoint.screenPoint, touch.position))
                .First().index;

            // touch.position
            // このフレームで押されたよん
            if (touch.phase == TouchPhase.Began)
            {
            }
        }

        _result.Clear();

        const float 判定外秒数 = 1f;

        var inputService = _inputService;

        foreach (var note in notJudgedNotes)
        {
            // 判定ラインを超えているか
            // 判定ラインを過ぎて 0.2 秒経ったらミスにする
            if (note.JudgeTime - currentTime < 0.2f)
            {
                _result.Add(new JudgeResult
                {
                    ResultType = JudgeResultType.Miss
                });

                continue;
            }


            if (Mathf.Abs(note.JudgeTime - currentTime) >= 判定外秒数)
            {
                break;
            }

            if (note.Type == NoteType.Tap)
            {
                for (var i = 0; i < note.Size; i++)
                {
                    // 現在チェックするレーン番号
                    var laneIndex = note.LanePosition + i;

                    // 今押された瞬間だよ
                    if (inputService.LaneTapStates[laneIndex].TapStating)
                    {
                        if (Mathf.Abs(note.JudgeTime - currentTime) < 0.2f)
                        {
                            // パーフェクト
                            notJudgedNotes.RemoveAt(0);

                            _result.Add(new JudgeResult
                            {
                                ResultType = JudgeResultType.Perfect
                            });

                            // メインのクラスに判定結果を伝えます
                            GameObject.FindObjectOfType<RhythmGamePresenter>().HandleJudgeResult(JudgeResultType.Perfect);
                        }

                        if (Mathf.Abs(note.JudgeTime - currentTime) < 0.4f)
                        {
                            // GOOD
                            notJudgedNotes.RemoveAt(0);

                            _result.Add(new JudgeResult
                            {
                                ResultType = JudgeResultType.Good
                            });
                        }
                    }
                }
            }
        }

        return _result;
    }
}
*/

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

    private ReilasChartEntity _chartEntity;

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
        new Vector3(2.5f, 0, 0),
        new Vector3(1.25f, 0, 0),
        new Vector3(-1.25f, 0, 0),
        new Vector3(-2.5f, 0, 0),
    };

     IEnumerable<Vector3> screenPoints = lanePositions.Select(lanePosition3D => Camera.main.WorldToScreenPoint(lanePosition3D));// Camera.main.WorldToScreenPoint(lanePosition3D))  "レーンの位置を"2D変換  //

    private void Update()
    {
        

        var touches = Input.touches;


        Debug.Log(touches);
        foreach (var touch in touches)
        {
            var nearestLaneIndex = screenPoints.Select((screenPoint, index) => (screenPoint, index)).OrderBy(screenPoint => Vector2.Distance(screenPoint.screenPoint, touch.position)).First().index;//押した場所に一番近いレーンの番号
            Debug.Log(nearestLaneIndex);

            // touch.position
            // このフレームで押されたよん
            if (touch.phase == TouchPhase.Began)
            {
                InputService.aboveLaneTapStates.Add(new LaneTapState
                {
                    laneNumber = nearestLaneIndex,
                    IsHold = true,
                    TapStating = true
                });
            }
            else
            {
                InputService.aboveLaneTapStates.Add(new LaneTapState
                {
                    laneNumber = nearestLaneIndex,
                    IsHold = true,
                    TapStating = false
                });
            }
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
}

public class InputService
{
    // 上レーン 最大 36 個
    public static List<LaneTapState> aboveLaneTapStates = new List<LaneTapState>();

    // 下レーン 最大 4 つ
    //public static List<LaneTapState> LaneTapStates = new List<LaneTapState>();
}
