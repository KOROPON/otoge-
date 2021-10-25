#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Rhythmium;
using UnityEngine;
using Reilas;
using System;

public abstract class HeadGuide
{
    public int indexNum;
    public float time;
}

public class ReilasNoteEntityToGameObject
{
    public ReilasNoteEntity note;
    public bool hasBeenTapped;
}

public sealed class RhythmGamePresenter : MonoBehaviour
{
    public AudioSource songAudio = null!;

    private ScoreComboCalculator _scoreComboCalculator;
    
    [SerializeField] private TapNote tapNotePrefab = null!;
    [SerializeField] private HoldNote holdNotePrefab = null!;
    [SerializeField] private AboveTapNote aboveTapNotePrefab = null!;
    [SerializeField] private AboveChainNote aboveChainNotePrefab = null!;
    [SerializeField] private AboveHoldNote aboveHoldNotePrefab = null!;
    [SerializeField] private AboveSlideNote aboveSlideNotePrefab = null!;
    
    [SerializeField] private BarLine barLinePrefab = null!;
    [SerializeField] private GameObject keyBeamPrefab = null!;
    
    private readonly List<GameObject> _allKeyBeam = new List<GameObject>();
        
    private static AudioSource _audioSource = null!;
    private static readonly AudioSource LongPerfect = null!;

    public static readonly List<TapNote> TapNotes = new List<TapNote>();
    public static readonly List<AboveTapNote> AboveTapNotes = new List<AboveTapNote>();
    public static readonly List<AboveChainNote> AboveChainNotes = new List<AboveChainNote>();
    public static readonly List<HoldNote> HoldNotes = new List<HoldNote>();
    public static readonly List<AboveHoldNote> AboveHoldNotes = new List<AboveHoldNote>();
    public static readonly List<AboveSlideNote> AboveSlideNotes = new List<AboveSlideNote>();

    private List<ReilasNoteLineEntity> _reilasAboveSlide = new List<ReilasNoteLineEntity>();
    private List<ReilasNoteLineEntity> _reilasAboveHold = new List<ReilasNoteLineEntity>();
    private List<ReilasNoteLineEntity> _reilasHold = new List<ReilasNoteLineEntity>();
    private List<ReilasNoteEntity> _reilasChain = new List<ReilasNoteEntity>();

    public static readonly List<BarLine> BarLines = new List<BarLine>();

    private List<ReilasNoteEntity> _tapNotes = new List<ReilasNoteEntity>();

    public static List<ReilasNoteEntity> internalNotes = new List<ReilasNoteEntity>();
    public static List<ReilasNoteEntity> chainNotes = new List<ReilasNoteEntity>();
    
    public static readonly List<ReilasNoteEntityToGameObject>[] TapNoteLanes = new List<ReilasNoteEntityToGameObject>[36];
    
    public static int countNotes;
    
    //Effect用
    public static readonly List<HoldEffector> HoldEffectors = new List<HoldEffector>();
    public static readonly List<AboveHoldEffector> AboveHoldEffectors = new List<AboveHoldEffector>();
    public static readonly List<AboveSlideEffector> AboveSlideEffectors = new List<AboveSlideEffector>();

    //Judge用
    public static bool[]? internalNoteJudge;
    public static bool[]? chainNoteJudge;
    
    private ReilasChartEntity _chartEntity = null!;

    public static string musicName = null!;
    public static string dif = null!;

    //Reilas移行判定
    public bool jumpToKujo;
    private bool _throughPoint;

    public static readonly bool[] LaneTapStates = new bool[36];

    public float gameSpeed;

    private List<HeadGuide> _aboveHoldHead = new List<HeadGuide>();
    private List<HeadGuide> _aboveSlideHead = new List<HeadGuide>();
    private List<HeadGuide> _holdHead = new List<HeadGuide>();
    
    private JudgeService? _judgeService;

    private float _judgeTime;
    private float _audioTime;

    private static readonly System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();

    /// <summary>
    /// 判定結果を処理する
    /// </summary>

    private readonly List<float> _barLineTimes = BarLine.BarLines;

    private static IEnumerable<ReilasNoteEntity> GetNoteTypes(ReilasChartEntity chart, string type)
    {
        return type switch
        {
            "Tap" => chart.Notes.Where(note => note.Type == NoteType.Tap || note.Type == NoteType.Hold || note.Type == NoteType.AboveTap || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveSlide),
            "Internal" => chart.Notes.Where(note => note.Type == NoteType.HoldInternal || note.Type == NoteType.AboveHoldInternal || note.Type == NoteType.AboveSlideInternal),
            "Chain" => chart.Notes.Where(note => note.Type == NoteType.AboveChain),
            "GroundTap" => chart.Notes.Where(note => note.Type == NoteType.Tap || note.Type == NoteType.Hold),
            "AboveTap" => chart.Notes.Where(note => note.Type == NoteType.AboveTap || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveSlide),
            _=> chart.Notes.Where(note => note.Type == NoteType.None)
        };
    }
    
    public static bool CheckType(ReilasNoteEntity note, string noteType)
    {
        return noteType switch
        {
            "GroundTap" => note.Type == NoteType.Tap || note.Type == NoteType.Hold,
            "AboveTap" => note.Type == NoteType.AboveTap || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveSlide,
            "GroundInternal" => note.Type == NoteType.HoldInternal,
            "AboveInternal" => note.Type == NoteType.AboveHoldInternal || note.Type == NoteType.AboveSlideInternal,
            "Chain" => note.Type == NoteType.AboveChain,
            _ => note.Type == NoteType.None
        };
    }
    
    public static int GetLane(ReilasNoteEntity note)
    {
        if (CheckType(note, "GroundTap") || CheckType(note, "GroundInternal")) return note.LanePosition;
        return note.LanePosition + 4;
    }

    private static void GetLanes(ReilasNoteEntityToGameObject note)
    {
        var noteLanePosition = GetLane(note.note);
        if (noteLanePosition < 4) TapNoteLanes[noteLanePosition].Add(note);
        else
        {
            switch (noteLanePosition)
            { 
                case 4: 
                { 
                    for (var i = noteLanePosition; i < noteLanePosition + note.note.Size; i++)
                    {
                        TapNoteLanes[i].Add(note);
                    }

                    break;
                } 
                case 35: 
                { 
                    for (var i = noteLanePosition - 1; i < noteLanePosition + note.note.Size - 1; i++)
                    {
                        TapNoteLanes[i].Add(note);
                    }

                    break;
                }
                default: 
                { 
                    for (var i = noteLanePosition - 1; i < noteLanePosition + note.note.Size; i++)
                    {
                        TapNoteLanes[i].Add(note);
                    }

                    break;
                }
            }
        }
    }
    
    private void Awake()
    {
        _judgeService = gameObject.AddComponent<JudgeService>();
        _scoreComboCalculator = GameObject.Find("Main").GetComponent<ScoreComboCalculator>();
        NotePositionCalculatorService.CalculateGameSpeed();
        AwakeAsync().Forget();
        for (var i = 0; i < _judgeService.tapJudgeStartIndex.Length; i++) _judgeService.tapJudgeStartIndex[i] = 0;
        _judgeService.internalJudgeStartIndex = 0;
        _judgeService.chainJudgeStartIndex = 0;
    }
    
    private async UniTask AwakeAsync()
    {
        //FindObjectOfType<Variable>().enabled = false;

        var chartTextAsset = await Resources.LoadAsync<TextAsset>("Charts/" + musicName + "." + dif) as TextAsset;

        if (chartTextAsset == null)
        {
            Debug.LogError("譜面データが見つかりませんでした");
            return;
        }


        var chartJsonData = JsonUtility.FromJson<ChartJsonData>(chartTextAsset.text);
        var chartEntity = new ReilasChartConverter().Convert(chartJsonData);
        
        foreach (var bpm in chartEntity.BpmChanges)
        {
           // Debug.Log(bpm.Duration);
        }

        NoteLineJsonData[] noteJsonData = chartJsonData.timeline.noteLines;

        var audioClipPath = "Songs/Songs/" + Path.GetFileNameWithoutExtension(chartJsonData.audioSource);
        var audioClip = await Resources.LoadAsync<AudioClip>(audioClipPath) as AudioClip;
        _audioSource = songAudio;
        _audioSource.clip = audioClip;

        if (_audioSource.clip != null) BarLine.GetBarLines(musicName, _audioSource.clip.length);


        if (PlayerPrefs.HasKey("volume"))
        {
            // tap音調整
        }

        // chartEntity
        _chartEntity = chartEntity;

        _tapNotes = new List<ReilasNoteEntity>(GetNoteTypes(_chartEntity, "Tap"));
        internalNotes = new List<ReilasNoteEntity>(GetNoteTypes(_chartEntity, "Internal"));
        chainNotes = new List<ReilasNoteEntity>(GetNoteTypes(_chartEntity, "Chain"));
        
        _reilasAboveSlide = _chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveSlide).ToList();
        _reilasAboveHold = _chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveHold).ToList();
        _reilasHold = _chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.Hold).ToList();
        _reilasChain = _chartEntity.Notes.Where(note => note.Type == NoteType.AboveChain).ToList();
        
        List<int> removeInt = new List<int>();
        
        foreach (var note in _tapNotes)
        {
            switch (note.Type)
            {
                case NoteType.AboveSlide:
                {
                    if (noteJsonData.Any(jsonData => note.JsonData.guid == jsonData.tail))
                    {
                        note.Type = NoteType.AboveSlideInternal;
                        internalNotes.Add(note);
                        removeInt.Add(_tapNotes.IndexOf(note));
                    }

                    break;
                }
                case NoteType.AboveHold:
                {
                    if (noteJsonData.Any(jsonData => note.JsonData.guid == jsonData.tail))
                    {
                        note.Type = NoteType.AboveSlideInternal;
                        internalNotes.Add(note);
                        removeInt.Add(_tapNotes.IndexOf(note));
                    }

                    break;
                }
                case NoteType.Hold:
                {
                    if (noteJsonData.Any(jsonData => note.JsonData.guid == jsonData.tail))
                    {
                        note.Type = NoteType.AboveSlideInternal;
                        internalNotes.Add(note);
                        removeInt.Add(_tapNotes.IndexOf(note));
                    }

                    break;
                }
                case NoteType.Tap:
                    break;
                case NoteType.HoldInternal:
                    break;
                case NoteType.AboveTap:
                    break;
                case NoteType.AboveHoldInternal:
                    break;
                case NoteType.AboveSlideInternal:
                    break;
                case NoteType.AboveChain:
                    break;
                case NoteType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        for(var num = removeInt.Count() - 1; num >= 0; num--)
        {
            _tapNotes.RemoveAt(removeInt[num]);
        }
        
        _tapNotes.OrderBy(note => note.JudgeTime);
        internalNotes.OrderBy(note => note.JudgeTime);
        chainNotes.OrderBy(note => note.JudgeTime);
        
        chainNoteJudge = new bool[chainNotes.Count];
        internalNoteJudge = new bool[internalNotes.Count];

        countNotes = _tapNotes.Count + internalNotes.Count + chainNotes.Count;
        
        for (var i = 0; i < TapNoteLanes.Length; i++)
        {
            TapNoteLanes[i] = new List<ReilasNoteEntityToGameObject>();
        }
        
        SpawnTapNotes(GetNoteTypes(_chartEntity, "GroundTap"));
        SpawnAboveTapNotes(GetNoteTypes(_chartEntity, "AboveTap"));
        SpawnChainNotes(_reilasChain);
        SpawnHoldNotes(_reilasHold);
        SpawnAboveHoldNotes(_reilasAboveHold);
        SpawnAboveSlideNotes(_reilasAboveSlide);
        SpawnBarLines(_barLineTimes);
        
        //シーンを開く
        Shutter.bltoPlay = true;
        Shutter.blShutterChange = "Open";
    }

    public static void PlaySongs()
    {
        _audioSource.Play();
        Stopwatch.Start();
    }


    private void SpawnTapNotes(IEnumerable<ReilasNoteEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(tapNotePrefab);
            tapNote.Initialize(note);
            GetLanes(new ReilasNoteEntityToGameObject
            {
                note = note,
                hasBeenTapped = false
            });
            var transform1 = tapNote.transform;
            var position = transform1.position;
            transform1.position = new Vector3(position.x, position.y, -10);
            TapNotes.Add(tapNote);
            tapNote.gameObject.SetActive(false);
        }
    }

    private void SpawnAboveTapNotes(IEnumerable<ReilasNoteEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(aboveTapNotePrefab);
            tapNote.Initialize(note);
            GetLanes(new ReilasNoteEntityToGameObject
            {
                note = note,
                hasBeenTapped = true
            });
            AboveTapNotes.Add(tapNote);
            tapNote.gameObject.SetActive(false);
        }
    }

    private void SpawnChainNotes(IEnumerable<ReilasNoteEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(aboveChainNotePrefab);
            tapNote.Initialize(note);
            AboveChainNotes.Add(tapNote);
            tapNote.gameObject.SetActive(false);
        }
    }

    private void SpawnHoldNotes(IEnumerable<ReilasNoteLineEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(holdNotePrefab);
            var holdEffector = tapNote.transform.Find("HoldEffector").gameObject.GetComponent<HoldEffector>();
            tapNote.Initialize(note);
            holdEffector.EffectorInitialize(note);
            tapNote.gameObject.SetActive(false);
            HoldEffectors.Add(holdEffector);
            HoldNotes.Add(tapNote);
        }
    }

    private void SpawnAboveHoldNotes(IEnumerable<ReilasNoteLineEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(aboveHoldNotePrefab);
            var aboveHoldEffector = tapNote.transform.Find("AboveHoldEffector").gameObject.GetComponent<AboveHoldEffector>();
            tapNote.Initialize(note);
            aboveHoldEffector.EffectorInitialize(note);
            tapNote.gameObject.SetActive(false);
            AboveHoldEffectors.Add(aboveHoldEffector);
            AboveHoldNotes.Add(tapNote);
        }
    }
    private void SpawnAboveSlideNotes(IEnumerable<ReilasNoteLineEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(aboveSlideNotePrefab);
            var aboveSlideEffector = tapNote.transform.Find("AboveSlideEffector").gameObject.GetComponent<AboveSlideEffector>();
            tapNote.Initialize(note);
            aboveSlideEffector.EffectorInitialize(note);
            tapNote.gameObject.SetActive(false);
            AboveSlideEffectors.Add(aboveSlideEffector);
            AboveSlideNotes.Add(tapNote);
        }
    }

    private void SpawnBarLines(IEnumerable<float> lines)
    {
        foreach (var line in lines)
        {
            var barLine = Instantiate(barLinePrefab);
            barLine.Initialize();
            barLine.gameObject.SetActive(false);
            BarLines.Add(barLine);
        }
    }

    private const float Z = -0.9f;

    public static readonly Vector3[] LanePositions = new Vector3[]
    {
        //下のレーン
        new Vector3(-3f, 0, Z),
        new Vector3(-1f, 0, Z),
        new Vector3(1f, 0,Z),
        new Vector3(3f, 0, Z),

        //上のレーン
        new Vector3(-4.5f,0.22f,Z),
        new Vector3(-4.45f,0.66f,Z),
        new Vector3(-4.36f,1.09f,Z),
        new Vector3(-4.23f,1.51f,Z),
        new Vector3(-4.06f,1.92f,Z),
        new Vector3(-3.86f,2.31f,Z),
        new Vector3(-3.61f,2.68f,Z),
        new Vector3(-3.33f,3.02f,Z),
        new Vector3(-3.02f,3.33f,Z),
        new Vector3(-2.68f,3.61f,Z),
        new Vector3(-2.31f,3.86f,Z),
        new Vector3(-1.92f,4.06f,Z),
        new Vector3(-1.51f,4.23f,Z),
        new Vector3(-1.09f,4.36f,Z),
        new Vector3(-0.66f,4.45f,Z),
        new Vector3(-0.22f,4.5f,Z),
        new Vector3(0.22f,4.5f,Z),
        new Vector3(0.66f,4.45f,Z),
        new Vector3(1.09f,4.36f,Z),
        new Vector3(1.51f,4.23f,Z),
        new Vector3(1.92f,4.06f,Z),
        new Vector3(2.31f,3.86f,Z),
        new Vector3(2.68f,3.61f,Z),
        new Vector3(3.02f,3.33f,Z),
        new Vector3(3.33f,3.02f,Z),
        new Vector3(3.61f,2.68f,Z),
        new Vector3(3.86f,2.31f,Z),
        new Vector3(4.06f,1.92f,Z),
        new Vector3(4.23f,1.51f,Z),
        new Vector3(4.36f,1.09f,Z),
        new Vector3(4.45f,0.66f,Z),
        new Vector3(4.5f,0.22f,Z),
    };

    // Camera.main.WorldToScreenPoint(lanePosition3D))  "レーンの位置を"2D変換  //
    private readonly IEnumerable<Vector3> _screenPoints = LanePositions.Select(lanePosition3D => Camera.main != null ? Camera.main.WorldToScreenPoint(lanePosition3D) : default);

    private void Update()
    {
        if (_audioSource == null)
        {
          return;
        }
    
        InputService.AboveLaneTapStates.Clear();
        for (var i = 0; i < LaneTapStates.Length; i++)
        {
            LaneTapStates[i] = false;
        }
        
        var allTouch = Input.touches;
        Array.Resize(ref allTouch,0);
        var touches = Input.touches;


        foreach (var touch in touches)
        {
            //gameObject.transform.position = new Vector3()
            var (vector3, laneIndex) = _screenPoints.Select((screenPoint, index) => (screenPoint, index))
                .OrderBy(screenPoint => Vector2.Distance(screenPoint.screenPoint, touch.position)).First();
            var distance = Vector2.Distance(vector3, touch.position);
            var nearestLaneIndex = distance < 150 ? laneIndex : 40;//押した場所に一番近いレーンの番号
            //Debug.Log(nearestLaneIndex);
            bool start = touch.phase == TouchPhase.Began;
            // touch.position
            // このフレームで押されたよん
            
            if (nearestLaneIndex < 36)
            {
                LaneTapStates[nearestLaneIndex] = true;
            }
            else
            {
                continue;
            }
            
            InputService.AboveLaneTapStates.Add(new LaneTapState
            {
                laneNumber = nearestLaneIndex,
                tapStarting = start
            });
        }


        var currentTime = _audioSource.time;
        _judgeTime = currentTime;
        _audioTime = currentTime;
        
        if (PlayerPrefs.HasKey("judgeGap"))
        {
          _judgeTime += PlayerPrefs.GetFloat("judgeGap") / 1000;
        }
        
        if (PlayerPrefs.HasKey("audioGap"))
        {
          _audioTime += PlayerPrefs.GetFloat("audioGap") / 1000;
        }

        if (musicName == "Reilas" && _throughPoint)
        {
            if (currentTime >= 78)
            {
                if (_scoreComboCalculator.slider.value >= 0.75f) 
                {
                    jumpToKujo = true;
                }
                _throughPoint = true;
            }
        }
        

        for(var keyIndex = _allKeyBeam.Count() - 1; keyIndex >= 0; keyIndex--)
        {
            Destroy(_allKeyBeam[keyIndex].gameObject);
            _allKeyBeam.RemoveAt(keyIndex);
        }

        List<int> dupLane = new List<int>();
        //Debug.Log(InputService.aboveLaneTapStates.Count());
        foreach (var laneNum in InputService.AboveLaneTapStates.Select(tap => tap.laneNumber))
        {
            for (var i = 1; i <= dupLane.Count() - 1; i++)
            {
                if (laneNum == dupLane[i])
                {
                    continue;
                }
            }

            // キービームの表示
            var keyBeam = Instantiate(keyBeamPrefab);
            switch (laneNum)
            {
                case var n when n > 3 && n < 20:
                    keyBeam.transform.position = new Vector3(LanePositions[laneNum].x + 0.2f, LanePositions[laneNum].y, 9.7f);
                    keyBeam.transform.localScale = new Vector3(0.2f, 1, 1);
                    keyBeam.transform.Rotate(new Vector3(0, 90 - (180 / 33 * (laneNum - 2) + 180 / 33 * (laneNum - 1)) / 2, 0));
                    break;
                case var n when n > 3:
                    keyBeam.transform.position = new Vector3(LanePositions[laneNum].x - 0.2f, LanePositions[laneNum].y, 9.7f);
                    keyBeam.transform.localScale = new Vector3(0.2f, 1, 1);
                    keyBeam.transform.Rotate(new Vector3(0, 90 - (180 / 33 * (laneNum - 2) + 180 / 33 * (laneNum - 1)) / 2, 0));
                    break;
                default:
                    keyBeam.transform.position = new Vector3(LanePositions[laneNum].x, LanePositions[laneNum].y, 9.55f);
                    keyBeam.transform.localScale = new Vector3(1, 1f, 1);
                    break;
            }
            _allKeyBeam.Add(keyBeam);
            dupLane.Add(laneNum);
        }

      //  Debug.Log(judgeTime + "   " + stopwatch.Elapsed);

        _judgeService.Judge(_judgeTime);

    }

    private void LateUpdate()
    {
        var aboveTapMove = AboveTapNotes.Where(note => note.aboveTapTime - _audioTime < 5f);
        var tapNoteMove = TapNotes.Where(note => note.tapTime - _audioTime < 5f);
        var chainNoteMove = AboveChainNotes.Where(note => note.aboveChainTime - _audioTime < 5f);

        List<AboveHoldNote> aboveHoldNoteMove = new List<AboveHoldNote>();
        List<AboveSlideNote> aboveSlideNoteMove = new List<AboveSlideNote>();
        List<HoldNote> holdNoteMove = new List<HoldNote>();
        var indexNum = 0;

        var noteAddCutOff = _audioTime + 5;
        
        foreach(var aboveHold in _reilasAboveHold)
        {
            if (aboveHold.Head.JudgeTime < noteAddCutOff) // tail の時間 - currentTime < 5s の時 setActive => true & Render()
            {
                aboveHoldNoteMove.Add(AboveHoldNotes[indexNum]);
            }
            else
            {
                break;
            }
            indexNum++;
        }
        indexNum = 0;
        
        
        
        foreach(var aboveSlide in _reilasAboveSlide)
        {
            if (aboveSlide.Head.JudgeTime < noteAddCutOff) // tail の時間 - currentTime < 5s の時 setActive => true & Render()
            {
                aboveSlideNoteMove.Add(AboveSlideNotes[indexNum]);
            }
            else
            {
                break;
            }
            indexNum++;
        }
        indexNum = 0;
        foreach(var hold in _reilasHold)
        {
            if (hold.Head.JudgeTime < noteAddCutOff) // tail の時間 - currentTime < 5s の時 setActive => true & Render()
            {
                holdNoteMove.Add(HoldNotes[indexNum]);
            }
            else
            {
                break;
            }
            indexNum++;
        }

        foreach (var tapNote in tapNoteMove)
        {
            tapNote.Render(_audioTime);
        }

        foreach (var note in aboveTapMove)
        {
            note.Render(_audioTime);
        }

        foreach (var note in chainNoteMove)
        {
            note.Render(_audioTime);
        }

        for (var num = holdNoteMove.Count - 1; num >= 0; num--)
        {
            holdNoteMove[num].Render(_audioTime, num, _reilasHold);
        }

        for (var num = aboveHoldNoteMove.Count - 1; num >= 0; num--)
        {
            aboveHoldNoteMove[num].Render(_audioTime, num, _reilasAboveHold);
        }

        for (var num = aboveSlideNoteMove.Count - 1; num >= 0; num--)
        {
            aboveSlideNoteMove[num].Render(_audioTime, num, _reilasAboveSlide);
        }

        foreach (var note in HoldEffectors)
        {
            if (_audioTime - note.holdEffectTime >= 0)
            {
                note.Render(_audioTime, LongPerfect);
            }
            else break;
        }

        foreach (var note in AboveHoldEffectors)
        {
            if (_audioTime - note.aboveHoldEffectTime >= 0)
            {
                note.Render(_audioTime, LongPerfect);
            }
            else break;
        }
        foreach (var note in AboveSlideEffectors)
        {
            if (_audioTime - note.aboveSlideEffectTime >= 0)
            {
                note.Render(_audioTime, LongPerfect);
            }
            else break;
        }
        for (var i = 0; i < BarLines.Count; i++)
        {
            BarLines[i].Render(_barLineTimes[i], _audioTime);
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

public static class InputService
{
    // 上レーン 最大 36 個
    public static readonly List<LaneTapState> AboveLaneTapStates = new List<LaneTapState>();

    // 下レーン 最大 4 つ
    //public static List<LaneTapState> LaneTapStates = new List<LaneTapState>();
}
