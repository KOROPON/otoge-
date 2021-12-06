#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Rhythmium;
using UnityEngine;
using UnityEngine.UI;
using Reilas;
using System;
using Boss;
using ShutterScene;
using Title;

public class ReilasNoteEntityToGameObject
{
    public ReilasNoteEntity note = null!;
    public bool hasBeenTapped;
}

public class RhythmGamePresenter : MonoBehaviour
{
    [SerializeField] private TapNote tapNotePrefab = null!;
    [SerializeField] private HoldNote holdNotePrefab = null!;
    [SerializeField] private AboveTapNote aboveTapNotePrefab = null!;
    [SerializeField] private AboveChainNote aboveChainNotePrefab = null!;
    [SerializeField] private AboveHoldNote aboveHoldNotePrefab = null!;
    [SerializeField] private AboveSlideNote aboveSlideNotePrefab = null!;
    [SerializeField] private BarLine barLinePrefab = null!;
    [SerializeField] private NoteConnector noteConnectorPrefab = null!;
    [SerializeField] private GameObject keyBeamPrefab = null!;


    private AudioSource _longPerfect = null!;
    private ScoreComboCalculator _scoreComboCalculator = null!;

    private List<ReilasNoteEntity> _tapNotes = new List<ReilasNoteEntity>();
    
    private int _speedChangesIndex;

    private bool _checkSpeedChangeEntity;

    private static AudioSource _audioSource = null!;

    private readonly List<GameObject> _allKeyBeam = new List<GameObject>();
    private readonly List<Connector> _noteConnectors = new List<Connector>();
    
    private readonly List<float> _barLines = new List<float>();

    public AudioSource songAudio = null!;

    public bool judgeForgive;
    
    public List<ReilasNoteLineEntity> reilasAboveSlide = new List<ReilasNoteLineEntity>();
    public List<ReilasNoteLineEntity> reilasAboveHold = new List<ReilasNoteLineEntity>();
    public List<ReilasNoteLineEntity> reilasHold = new List<ReilasNoteLineEntity>();
    public List<ReilasNoteEntity> reilasChain = new List<ReilasNoteEntity>();
    public List<ReilasNoteLineEntity> reilasKujoAboveSlide = new List<ReilasNoteLineEntity>();
    public List<ReilasNoteLineEntity> reilasKujoAboveHold = new List<ReilasNoteLineEntity>();
    public List<ReilasNoteLineEntity> reilasKujoHold = new List<ReilasNoteLineEntity>();
    public List<ReilasNoteEntity> reilasKujoChain = new List<ReilasNoteEntity>();

    public static bool isHolding;
    
    public static int countNotes;

    public static List<ReilasNoteEntity> internalNotes = new List<ReilasNoteEntity>();
    public static List<ReilasNoteEntity> chainNotes = new List<ReilasNoteEntity>();

    public static readonly List<TapNote> TapNotes = new List<TapNote>();
    public static readonly List<AboveTapNote> AboveTapNotes = new List<AboveTapNote>();
    public static readonly List<AboveChainNote> AboveChainNotes = new List<AboveChainNote>();
    public static readonly List<HoldNote> HoldNotes = new List<HoldNote>();
    public static readonly List<AboveHoldNote> AboveHoldNotes = new List<AboveHoldNote>();
    public static readonly List<AboveSlideNote> AboveSlideNotes = new List<AboveSlideNote>();
    public static readonly List<TapNote> TapKujoNotes = new List<TapNote>();
    public static readonly List<AboveTapNote> AboveKujoTapNotes = new List<AboveTapNote>();
    public static readonly List<AboveChainNote> AboveKujoChainNotes = new List<AboveChainNote>();
    public static readonly List<HoldNote> HoldKujoNotes = new List<HoldNote>();
    public static readonly List<AboveHoldNote> AboveKujoHoldNotes = new List<AboveHoldNote>();
    public static readonly List<AboveSlideNote> AboveKujoSlideNotes = new List<AboveSlideNote>();
    public static readonly List<BarLine> BarLines = new List<BarLine>();
    public static readonly List<NoteConnector> NoteConnectors = new List<NoteConnector>();
    public static readonly List<NoteConnector> NoteKujoConnectors = new List<NoteConnector>();
    public static readonly List<ReilasNoteEntityToGameObject>[] TapNoteLanes = new List<ReilasNoteEntityToGameObject>[36];
    public static readonly List<ReilasNoteEntityToGameObject>[] TapKujoNoteLanes = new List<ReilasNoteEntityToGameObject>[36];
    public static readonly List<ReilasNoteEntity> InternalKujoNotes = new List<ReilasNoteEntity>();
    public static readonly List<ReilasNoteEntity> ChainKujoNotes = new List<ReilasNoteEntity>();
    
    private static readonly List<SpeedChangeEntity> SpeedChanges = new List<SpeedChangeEntity>();

    //Effect用
    public static readonly List<HoldEffector> HoldEffectors = new List<HoldEffector>();
    public static readonly List<AboveHoldEffector> AboveHoldEffectors = new List<AboveHoldEffector>();
    public static readonly List<AboveSlideEffector> AboveSlideEffectors = new List<AboveSlideEffector>();

    //Judge用
    public static bool[]? internalNoteJudge;
    public static bool[]? chainNoteJudge;

    private ReilasChartEntity? _chartEntity;

    public static string musicName = null!;
    public static string dif = null!;

    //Reilas移行判定
    public static bool jumpToKujo;
    public bool alreadyChangeKujo;
    private bool _isAllowed;
    private BossGimmicks? _boss;

    public static readonly bool[,] LaneTapStates = new bool[36, 2];

    private AllJudgeService? _judgeService;

    public float judgeTime;
    public float audioTime;

    public static bool tutorial; // チュートリアル判定
    public GameObject _tutorial;
    private Text tutorialText;
    /// <summary>
    /// 判定結果を処理する
    /// </summary>

    public static IEnumerable<ReilasNoteEntity> GetNoteTypes(ReilasChartEntity? chart, string type)
    {
        return type switch
        {
            "Tap" => chart.Notes.Where(note => note.Type == NoteType.Tap || note.Type == NoteType.Hold || note.Type == NoteType.AboveTap || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveSlide),
            "Internal" => chart.Notes.Where(note => note.Type == NoteType.HoldInternal || note.Type == NoteType.AboveHoldInternal || note.Type == NoteType.AboveSlideInternal),
            "Chain" => chart.Notes.Where(note => note.Type == NoteType.AboveChain),
            "GroundTap" => chart.Notes.Where(note => note.Type == NoteType.Tap || note.Type == NoteType.Hold),
            "AboveTap" => chart.Notes.Where(note => note.Type == NoteType.AboveTap || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveSlide),
            _ => chart.Notes.Where(note => note.Type == NoteType.None)
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

    private static int GetMiddleLane(ReilasNoteEntity note)
    {
        if (CheckType(note, "GroundTap") || CheckType(note, "GroundInternal")) return note.LanePosition;
        // ReSharper disable once PossibleLossOfFraction
        return GetLane(note) + (int) Mathf.Floor(note.Size / 2);
    }

    private static void GetLanes(ReilasNoteEntityToGameObject note, bool boss)
    {
        var noteLanePosition = GetLane(note.note);
        if (noteLanePosition < 4)
        {
            if (TapKujoNoteLanes.Length <= noteLanePosition) Debug.LogWarning("Null");
            if(boss) TapKujoNoteLanes[noteLanePosition].Add(note);
            else TapNoteLanes[noteLanePosition].Add(note);
        }
        else
        {
            switch (noteLanePosition)
            {
                case 35:
                    {
                        if (boss)
                        {
                            TapKujoNoteLanes[34].Add(note);
                            TapKujoNoteLanes[35].Add(note);
                            break;
                        }
                        TapNoteLanes[34].Add(note);
                        TapNoteLanes[35].Add(note);
                        break;
                    }
                case 4:
                    {
                        if (boss)
                        {
                            for (var i = noteLanePosition; i < noteLanePosition + note.note.Size && i < 36; i++) TapKujoNoteLanes[i].Add(note);
                            break;
                        }
                        for (var i = noteLanePosition; i < noteLanePosition + note.note.Size && i < 36; i++) TapNoteLanes[i].Add(note);

                        break;
                    }
                default:
                    {
                        if (boss)
                        {
                            for (var i = noteLanePosition - 1; i < noteLanePosition + note.note.Size && i < 36; i++) TapKujoNoteLanes[i].Add(note);
                            break;
                        }
                        for (var i = noteLanePosition - 1; i < noteLanePosition + note.note.Size && i < 36; i++) TapNoteLanes[i].Add(note);

                        break;
                    }
            }
        }
    }

    private static void ConnectorAdder(ICollection<ConnectingKinds> addingList, IReadOnlyList<int> lanes, string flag)
    {
        for (var i = 1; i < lanes.Count; i++)
        {
            addingList.Add(new ConnectingKinds
            {
                connector = new []{lanes[0], lanes[i]},
                kind = flag
            });
        }
    }

    private void Awake()
    {
        
        jumpToKujo = false;
        _isAllowed = gameObject.AddComponent<GetHighScores>().GetKujoLock("Reilas");
        _longPerfect = GameObject.Find("LongPerfect").GetComponent<AudioSource>();
        
        for (var i = 0; i < TapNoteLanes.Length; i++) TapNoteLanes[i] = new List<ReilasNoteEntityToGameObject>();
        
        for (var i = 0; i < TapKujoNoteLanes.Length; i++) TapKujoNoteLanes[i] = new List<ReilasNoteEntityToGameObject>();
        
        _judgeService = gameObject.GetComponent<AllJudgeService>();
        _boss = GameObject.Find("BossGimmick").GetComponent<BossGimmicks>();
        _judgeService.JudgeStart();
        _scoreComboCalculator = GameObject.Find("Main").GetComponent<ScoreComboCalculator>();
        NotePositionCalculatorService.CalculateGameSpeed();
        gameObject.AddComponent<GetHighScores>();
        
        for (var i = 0; i < 36; i++)
        {
            LaneTapStates[i, 0] = false;
            LaneTapStates[i, 1] = false;
            _judgeService.tapJudgeStartIndex[i] = 0;
        }
        
        AwakeAsync().Forget();
        
        if (musicName == "Reilas" && dif == "Extreme") _boss.BossAwake();
        
        _judgeService.internalJudgeStartIndex = 0;
        _judgeService.chainJudgeStartIndex = 0;
        
    }

    private async UniTask AwakeAsync()
    {
        if (_boss == null) return;

        if (ChangeScene.aspect > 2 && Camera.main != null)
                Camera.main.transform.position = new Vector3(0, 2.2f, -3.3f);

        TextAsset? chartTextAsset;
        if (tutorial)
        {
            chartTextAsset = await Resources.LoadAsync<TextAsset>("Charts/tutorial") as TextAsset;
            tutorialText = _tutorial.GetComponent<Text>();
            GameObject.Find("Button").SetActive(false);
            StartCoroutine(Tutorial());
        }
        else
        {
            chartTextAsset = await Resources.LoadAsync<TextAsset>("Charts/" + musicName + "." + dif) as TextAsset;
            _tutorial.SetActive(false);
        }

        if (chartTextAsset == null)
        {
            Debug.LogError("譜面データが見つかりませんでした");
            return;
        }

        var chartJsonData = JsonUtility.FromJson<ChartJsonData>(chartTextAsset.text);
        var chartEntity = new ReilasChartConverter().Convert(chartJsonData);

        NoteLineJsonData[] noteJsonData = chartJsonData.timeline.noteLines;

        var audioClipPath =
            "Songs/Songs/" + Path.GetFileNameWithoutExtension(chartJsonData.audioSource); //AudioSource の取得
        var audioClip = await Resources.LoadAsync<AudioClip>(audioClipPath) as AudioClip;
        Debug.Log(audioClipPath);
        _audioSource = songAudio;
        _audioSource.clip = audioClip;

        if (PlayerPrefs.HasKey("volume"))
        {
            var volume = PlayerPrefs.GetFloat("volume") / 100;
            GameObject.Find("Perfect").GetComponent<AudioSource>().volume = volume;
            GameObject.Find("Good").GetComponent<AudioSource>().volume = volume;
            GameObject.Find("Bad").GetComponent<AudioSource>().volume = volume;
            GameObject.Find("LongPerfect").GetComponent<AudioSource>().volume = volume;
        }

        if(chartEntity == null)
        {
            Debug.LogError("chartEntityIsNull!");
        }
            // chartEntity
            _chartEntity = chartEntity;
        if(_chartEntity == null)
        {
            Debug.LogError("_chartEntityIsNull");
        }

        if (_chartEntity != null && _chartEntity.SpeedChanges != null)
        {
            foreach (var bpm in _chartEntity.SpeedChanges) SpeedChanges.Add(bpm);
            
            _checkSpeedChangeEntity = true;
        }

        NotePositionCalculatorService.firstChartSpeed = 60f;
        NotePositionCalculatorService.CalculateNoteSpeed(NotePositionCalculatorService.firstChartSpeed);

        _tapNotes = new List<ReilasNoteEntity>(GetNoteTypes(_chartEntity, "Tap")).OrderBy(note => note.JudgeTime).ToList();
        var subInternalNotes = new List<ReilasNoteEntity>(GetNoteTypes(_chartEntity, "Internal"));
        chainNotes = new List<ReilasNoteEntity>(GetNoteTypes(_chartEntity, "Chain")).OrderBy(note => note.JudgeTime).ToList();

        _tapNotes = _tapNotes.Where(note => note.LanePosition >= 0 && note.LanePosition + note.Size < 36)
            .OrderBy(note => note.JudgeTime).ToList();
        internalNotes = internalNotes.Where(note => note.LanePosition >= 0 && note.LanePosition + note.Size < 36)
            .OrderBy(note => note.JudgeTime).ToList();
        chainNotes = chainNotes.Where(note => note.LanePosition >= 0 && note.LanePosition + note.Size < 36)
            .OrderBy(note => note.JudgeTime).ToList();

        if (_chartEntity != null)
        {
            reilasAboveSlide = _chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveSlide)
                .OrderBy(note => note.Head.JudgeTime).ToList();
            reilasAboveHold = _chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveHold)
                .OrderBy(note => note.Head.JudgeTime).ToList();
            reilasHold = _chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.Hold)
                .OrderBy(note => note.Head.JudgeTime).ToList();
            reilasChain = _chartEntity.Notes.Where(note => note.Type == NoteType.AboveChain)
                .OrderBy(note => note.JudgeTime).ToList();


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
                            subInternalNotes.Add(note);
                            removeInt.Add(_tapNotes.IndexOf(note));
                        }

                        break;
                    }
                    case NoteType.AboveHold:
                    {
                        if (noteJsonData.Any(jsonData => note.JsonData.guid == jsonData.tail))
                        {
                            note.Type = NoteType.AboveHoldInternal;
                            subInternalNotes.Add(note);
                            removeInt.Add(_tapNotes.IndexOf(note));
                        }

                        break;
                    }
                    case NoteType.Hold:
                    {
                        if (noteJsonData.Any(jsonData => note.JsonData.guid == jsonData.tail))
                        {
                            note.Type = NoteType.HoldInternal;
                            subInternalNotes.Add(note);
                            removeInt.Add(_tapNotes.IndexOf(note));
                        }

                        break;
                    }
                    case NoteType.Tap:
                    case NoteType.HoldInternal:
                    case NoteType.AboveTap:
                    case NoteType.AboveHoldInternal:
                    case NoteType.AboveSlideInternal:
                    case NoteType.AboveChain:
                    case NoteType.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            for (var num = removeInt.Count - 1; num >= 0; num--) _tapNotes.RemoveAt(removeInt[num]);

            _tapNotes.OrderBy(note => note.JudgeTime);
            chainNotes.OrderBy(note => note.JudgeTime);

            var notInternals = new List<ReilasNoteEntity>(_chartEntity.Notes.Where(note =>
                CheckType(note, "GroundTap") || CheckType(note, "AboveTap") || CheckType(note, "Chain")));

            foreach (var measure in _chartEntity.Measures) _barLines.Add(measure.JudgeTime);

            var tapNoteIndex = 0;
            var tapNotesLength = notInternals.Count;
            for (var i = tapNoteIndex; i < tapNotesLength; i++)
            {
                var lanes = new List<int>();
                var currentTime = notInternals[i].JudgeTime;
                var nextNoteIndex = i + 1;
                if (nextNoteIndex == tapNotesLength) break;
                if (Math.Abs(currentTime - notInternals[nextNoteIndex].JudgeTime) > 0) continue;
                for (var j = i; j < tapNotesLength; j++)
                {
                    var tapNote = notInternals[j];
                    var nextIndex = j + 1;
                    lanes.Add(GetMiddleLane(tapNote));
                    if (nextIndex == tapNotesLength) break;
                    if (Math.Abs(tapNote.JudgeTime - notInternals[nextIndex].JudgeTime) == 0f) continue;
                    tapNoteIndex = nextIndex;
                    break;
                }

                var connectorKindList = new List<ConnectingKinds>();
                var orderedLanes = lanes.OrderBy(lane => lane);
                System.Diagnostics.Debug.Assert(orderedLanes != null, nameof(orderedLanes) + " != null");
                var groundTaps =
                    new List<int>((orderedLanes ?? throw new InvalidOperationException()).Where(note => note < 4));
                var aboveTaps = new List<int>(orderedLanes.Where(note => note >= 4));
                var groundTapsLength = groundTaps.Count;

                if (groundTapsLength > 0)
                {
                    if (groundTapsLength > 1) ConnectorAdder(connectorKindList, groundTaps, "Ground-Ground");

                    foreach (var lane in aboveTaps)
                    {
                        connectorKindList.Add(new ConnectingKinds
                        {
                            connector = new[] {NoteConnector.GetConnectorLane(lane, groundTaps), lane},
                            kind = "Ground-Above"
                        });
                    }
                }

                ConnectorAdder(connectorKindList, aboveTaps, "Above-Above");

                _noteConnectors.Add(new Connector
                {
                    currentTime = currentTime,
                    connectingList = connectorKindList
                });
            }


            reilasAboveSlide = reilasAboveSlide.OrderBy(note => note.Head.JudgeTime).ToList();
            reilasAboveHold = reilasAboveHold.OrderBy(note => note.Head.JudgeTime).ToList();
            reilasHold = reilasHold.OrderBy(note => note.Head.JudgeTime).ToList();
            reilasChain = reilasChain.OrderBy(note => note.JudgeTime).ToList();

            SpawnTapNotes(GetNoteTypes(_chartEntity, "GroundTap"), false);
            SpawnAboveTapNotes(GetNoteTypes(_chartEntity, "AboveTap"), false);
        }

        SpawnChainNotes(reilasChain, false);
        SpawnHoldNotes(reilasHold, false);
        SpawnAboveHoldNotes(reilasAboveHold, false);
        SpawnAboveSlideNotes(reilasAboveSlide, false);
        SpawnBarLines(_barLines);
        SpawnNoteConnectors(_noteConnectors, false);

        BarLines.OrderBy(note => note.judgeTime);

        internalNotes.Add(subInternalNotes[0]);
        for (var num = 1; num < subInternalNotes.Count; num++)
        {
            for (int index = 0; index <= internalNotes.Count; index++)
            {
                if (index == internalNotes.Count)
                {
                    internalNotes.Add(subInternalNotes[num]);
                }
                if (subInternalNotes[num].JudgeTime <= internalNotes[index].JudgeTime)
                {
                    internalNotes.Insert(index, subInternalNotes[num]);
                    break;
                }
            }
        }
        chainNoteJudge = new bool[chainNotes.Count];
        internalNoteJudge = new bool[internalNotes.Count];

        countNotes = _tapNotes.Count + internalNotes.Count + chainNotes.Count;

        if (_scoreComboCalculator != null) _scoreComboCalculator.ScoreComboStart();

        Shutter.blToPlay = true;
        Shutter.blShutterChange = "Open";
    }

    public static void PlaySongs()
    {
        _audioSource.Play();
    }


    public void SpawnTapNotes(IEnumerable<ReilasNoteEntity> notes, bool bosNotes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(tapNotePrefab);
            tapNote.Initialize(note);
            
            GetLanes(new ReilasNoteEntityToGameObject
            {
                note = note,
                hasBeenTapped = false
            }, bosNotes);
            
            var transform1 = tapNote.transform;
            var position = transform1.position;
            transform1.position = new Vector3(position.x, position.y, -10);
            
            if (!bosNotes) TapNotes.Add(tapNote);
            else TapKujoNotes.Add(tapNote);
            
            tapNote.transform.position = new Vector3(position.x, position.y, 999);
            tapNote.gameObject.SetActive(false);
        }
    }

    public void SpawnAboveTapNotes(IEnumerable<ReilasNoteEntity> notes, bool bosNotes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(aboveTapNotePrefab);
            tapNote.Initialize(note);
            
            GetLanes(new ReilasNoteEntityToGameObject
            {
                note = note,
                hasBeenTapped = false
            }, bosNotes);
            
            if (!bosNotes) AboveTapNotes.Add(tapNote);
            else AboveKujoTapNotes.Add(tapNote);
            
            tapNote.gameObject.SetActive(false);
        }
    }

    public void SpawnChainNotes(IEnumerable<ReilasNoteEntity> notes, bool bosNotes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(aboveChainNotePrefab);
            tapNote.Initialize(note);
            
            if (!bosNotes) AboveChainNotes.Add(tapNote);
            else AboveKujoChainNotes.Add(tapNote);
            
            tapNote.gameObject.SetActive(false);
        }
    }

    public void SpawnHoldNotes(IEnumerable<ReilasNoteLineEntity> notes, bool bosNotes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(holdNotePrefab);
            var holdEffector = tapNote.transform.Find("HoldEffector").gameObject.GetComponent<HoldEffector>();
            
            tapNote.Initialize(note, bosNotes);
            holdEffector.EffectorInitialize(note);
            tapNote.gameObject.SetActive(false);
            HoldEffectors.Add(holdEffector);
            
            if (!bosNotes) HoldNotes.Add(tapNote);
            else HoldKujoNotes.Add(tapNote);
        }
    }

    public void SpawnAboveHoldNotes(IEnumerable<ReilasNoteLineEntity> notes, bool bosNotes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(aboveHoldNotePrefab);
            var aboveHoldEffector =
                tapNote.transform.Find("AboveHoldEffector").gameObject.GetComponent<AboveHoldEffector>();
            
            tapNote.Initialize(note, bosNotes);
            aboveHoldEffector.EffectorInitialize(note);
            tapNote.gameObject.SetActive(false);
            AboveHoldEffectors.Add(aboveHoldEffector);
            
            if (!bosNotes) AboveHoldNotes.Add(tapNote);
            else AboveKujoHoldNotes.Add(tapNote);
        }
    }
    
    public void SpawnAboveSlideNotes(IEnumerable<ReilasNoteLineEntity> notes, bool bosNotes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(aboveSlideNotePrefab);
            var aboveSlideEffector = tapNote.transform.Find("AboveSlideEffector").gameObject
                .GetComponent<AboveSlideEffector>();
            
            tapNote.Initialize(note, bosNotes);
            aboveSlideEffector.EffectorInitialize(note);
            tapNote.gameObject.SetActive(false);
            AboveSlideEffectors.Add(aboveSlideEffector);
            if (!bosNotes) AboveSlideNotes.Add(tapNote);
            else AboveKujoSlideNotes.Add(tapNote);
        }
    }

    private void SpawnBarLines(IEnumerable<float> lines)
    {
        foreach (var line in lines)
        {
            var barLine = Instantiate(barLinePrefab);
            
            barLine.Initialize(line);
            BarLines.Add(barLine);
        }
    }

    private void SpawnNoteConnectors(IEnumerable<Connector> connectors, bool bosNotes)
    {
        foreach (var connector in connectors)
        {
            var connect = Instantiate(noteConnectorPrefab);
            
            connect.Initialize(connector, bosNotes);
            connect.gameObject.SetActive(false);
            NoteConnectors.Add(connect);
        }
    }

    private const float Z = -0.3f;
    private const float Z2 = -0.4f;
    
    public static readonly Vector3[] LanePositions = new Vector3[]
    {
        //下のレーン
        new Vector3(-3f, 0, Z2),
        new Vector3(-1f, 0, Z2),
        new Vector3(1f, 0,Z2),
        new Vector3(3f, 0, Z2),

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
    
    private const float Za = -0.35f;
    private const float Za2 = -0.33f;

    private static readonly Vector3[] UserLanePositions = new Vector3[]
    {
        //下のレーン
        new Vector3(-3.3f, -0.2f, Za),
        new Vector3(-1.1f, -0.2f,Za),
        new Vector3(1.1f, -0.2f, Za),
        new Vector3(3.3f, -0.2f, Za),

        //上のレーン
        new Vector3(-5.4f,0.262f,Za2),
        new Vector3(-5.34f,0.792f,Za2),
        new Vector3(-5.232f,1.308f,Za2),
        new Vector3(-5.076f,1.812f,Za2),
        new Vector3(-4.872f,2.304f,Za2),
        new Vector3(-4.632f,2.772f,Za2),
        new Vector3(-4.322f,3.216f,Za2),
        new Vector3(-3.966f,3.624f,Za2),
        new Vector3(-3.624f,3.996f,Za2),
        new Vector3(-3.214f,4.332f,Za2),
        new Vector3(-2.772f,4.632f,Za2),
        new Vector3(-2.304f,4.872f,Za2),
        new Vector3(-1.812f,5.076f,Za2),
        new Vector3(-1.308f,5.232f,Za2),
        new Vector3(-0.792f,5.34f,Za2),
        new Vector3(-0.264f,5.4f,Za2),
        new Vector3(0.264f,5.4f,Za2),
        new Vector3(0.792f,5.34f,Za2),
        new Vector3(1.308f,5.232f,Za2),
        new Vector3(1.812f,5.076f,Za2),
        new Vector3(2.304f,4.872f,Za2),
        new Vector3(2.772f,4.632f,Za2),
        new Vector3(3.214f,4.332f,Za2),
        new Vector3(3.624f,3.996f,Za2),
        new Vector3(3.966f,3.624f,Za2),
        new Vector3(4.322f,3.216f,Za2),
        new Vector3(4.632f,2.772f,Za2),
        new Vector3(4.872f,2.304f,Za2),
        new Vector3(5.076f,1.812f,Za2),
        new Vector3(5.232f,1.308f,Za2),
        new Vector3(5.34f,0.792f,Za2),
        new Vector3(5.4f,0.264f,Za2),
    };

    // Camera.main.WorldToScreenPoint(lanePosition3D))  "レーンの位置を"2D変換  //
    private readonly IEnumerable<Vector3> _screenPoints = UserLanePositions.Select(lanePosition3D =>
        Camera.main != null ? Camera.main.WorldToScreenPoint(lanePosition3D) : default);

    public static float CalculatePassedTime(List<SpeedChangeEntity> bpmChanges, int index)
    {
        var passedTime = 0f;
        for (var i = 0; i <= Math.Min(index, bpmChanges.Count - 1); i++)
        {
            if (i == 0) passedTime += 60 * 4 / NotePositionCalculatorService.firstChartSpeed * bpmChanges[0].Position;
            else passedTime += 60 * 4 / bpmChanges[i - 1].Speed * (bpmChanges[i].Position - bpmChanges[i - 1].Position);
        }

        return passedTime;
    }

    private void Update()
    {
        if (_audioSource == null) return;

        InputService.AboveLaneTapStates.Clear();
        for (var i = 0; i < 36; i++)
        {
            LaneTapStates[i, 1] = LaneTapStates[i, 0];
            LaneTapStates[i, 0] = false;
        }

        var allTouch = Input.touches;
        Array.Resize(ref allTouch, 0);
        var touches = Input.touches;


        foreach (var touch in touches)
        {
            var (vector3, laneIndex) = _screenPoints.Select((screenPoint, index) => (screenPoint, index))
                .OrderBy(screenPoint => Vector2.Distance(screenPoint.screenPoint, touch.position)).First();
            var distance = Vector2.Distance(vector3, touch.position);
            var nearestLaneIndex = distance < 500 ? laneIndex : 40;//押した場所に一番近いレーンの番号
            var start = touch.phase == TouchPhase.Began;
            
            // touch.position
            // このフレームで押されたよん

            if (nearestLaneIndex < 36)
            {
                LaneTapStates[nearestLaneIndex, 0] = true;
                LaneTapStates[nearestLaneIndex, 1] = start;
            }
            else continue;

            InputService.AboveLaneTapStates.Add(new LaneTapState
            {
                laneNumber = nearestLaneIndex,
                tapStarting = start
            });
        }


        var currentTime = _audioSource.time;
        judgeTime = currentTime;
        audioTime = currentTime;

        if (_checkSpeedChangeEntity)
        {
            for (var i = _speedChangesIndex; i < SpeedChanges.Count; i++)
            {
                if (currentTime < CalculatePassedTime(SpeedChanges, i)) break;
                
                NotePositionCalculatorService.CalculateNoteSpeed(SpeedChanges[i].Speed);
                _speedChangesIndex++;
            }
        }

        if (PlayerPrefs.HasKey("judgegap"))
        {
            judgeTime += PlayerPrefs.GetFloat("judgegap") / 1000;
        }
        if (PlayerPrefs.HasKey("audiogap"))
        {
            audioTime += PlayerPrefs.GetFloat("audiogap") / 1000;
            judgeTime += PlayerPrefs.GetFloat("audiogap") / 1000;
        }

        if (musicName == "Reilas" && dif == "Extreme" && !alreadyChangeKujo && _scoreComboCalculator != null &&
            !_isAllowed) jumpToKujo = _scoreComboCalculator.slider.fillAmount >= 0.7f;
        
        for (var keyIndex = _allKeyBeam.Count - 1; keyIndex >= 0; keyIndex--)
        {
            Destroy(_allKeyBeam[keyIndex].gameObject);
            _allKeyBeam.RemoveAt(keyIndex);
        }

        List<int> dupLane = new List<int>();
        if (dupLane == null) throw new ArgumentNullException(nameof(dupLane));

        foreach (var laneNum in InputService.AboveLaneTapStates.Select(tap => tap.laneNumber))
        {
            // キービームの表示
            var keyBeam = Instantiate(keyBeamPrefab);
            switch (laneNum)
            {
                case var n when n > 3 && n < 20:
                {
                    keyBeam.transform.position =
                        new Vector3(LanePositions[laneNum].x + 0.2f, LanePositions[laneNum].y, 9.7f);
                    keyBeam.transform.localScale = new Vector3(0.25f, 1, 1);
                    keyBeam.transform.Rotate(new Vector3(0,
                        90 - (180 / 33 * (laneNum - 2) + 180 / 33 * (laneNum - 1)) / 2, 0));
                    break;
                }
                case var n when n > 3:
                {
                    keyBeam.transform.position =
                        new Vector3(LanePositions[laneNum].x - 0.2f, LanePositions[laneNum].y, 9.7f);
                    keyBeam.transform.localScale = new Vector3(0.25f, 1, 1);
                    keyBeam.transform.Rotate(new Vector3(0,
                        90 - (180 / 33 * (laneNum - 2) + 180 / 33 * (laneNum - 1)) / 2, 0));
                    break;
                }
                default:
                {
                    keyBeam.transform.position = new Vector3(LanePositions[laneNum].x, LanePositions[laneNum].y, 9.55f);
                    keyBeam.transform.localScale = new Vector3(1, 1, 1);
                    break;
                }
            }

            _allKeyBeam.Add(keyBeam);
            dupLane.Add(laneNum);
        }

        if (_judgeService != null)
        {
            if (_boss != null)
            {
                if (!_boss.kujoJudgeSwitch) _judgeService.Judge(judgeTime);
            }
            else _judgeService.Judge(judgeTime);
        }

        if (alreadyChangeKujo) return;
        
        if (currentTime < 82.9 || _boss == null) return;
        
        if (jumpToKujo)
        {
            _boss.ChangeToKujo();
            alreadyChangeKujo = true;
            if (_judgeService != null) _judgeService.alreadyChangeKujo = true;
        }
        else
        {
            BossGimmicks.NotChangeToKujo();
            alreadyChangeKujo = true;
        }
    }
    
    private void LateUpdate()
    {
        isHolding = false;
        IEnumerable<AboveTapNote> aboveTapMove;
        IEnumerable<TapNote> tapNoteMove;
        IEnumerable<AboveChainNote> chainNoteMove;
        IEnumerable<NoteConnector> connectorMove;

        List<AboveHoldNote> aboveHoldNoteMove = new List<AboveHoldNote>();
        List<AboveSlideNote> aboveSlideNoteMove = new List<AboveSlideNote>();
        List<HoldNote> holdNoteMove = new List<HoldNote>();

        List<ReilasNoteLineEntity> orgAboveSlideNote = new List<ReilasNoteLineEntity>();
        
        if (orgAboveSlideNote == null) throw new ArgumentNullException(nameof(orgAboveSlideNote));

        if (_boss == null) _boss = GameObject.Find("BossGimmick").GetComponent<BossGimmicks>();
        IEnumerable<BarLine> barLineMove = BarLines.Where(note => note.judgeTime - audioTime < 5f);

        var indexNum = 0;
        var noteAddCutOff = audioTime + 5;
        
        // tail の時間 - currentTime < 5s の時 setActive => true & Render()
        if (jumpToKujo && alreadyChangeKujo)
        {
            aboveTapMove = AboveKujoTapNotes.Where(note => note.aboveTapTime - audioTime < 5f);
            tapNoteMove = TapKujoNotes.Where(note => note.tapTime - audioTime < 5f);
            chainNoteMove = AboveKujoChainNotes.Where(note => note.aboveChainTime - audioTime < 5f);
            connectorMove = NoteKujoConnectors.Where(note => note.judgeTime - audioTime < 5f);

            foreach (var aboveHold in reilasKujoAboveHold)
            {
                // _reilasKujoAboveHold から Destroy してない
                if (aboveHold.Head.JudgeTime < noteAddCutOff) aboveHoldNoteMove.Add(AboveKujoHoldNotes[indexNum]);
                else break;
                
                indexNum++;
            }
            indexNum = 0;

            foreach (var aboveSlide in reilasKujoAboveSlide)
            {
                if (aboveSlide.Head.JudgeTime < noteAddCutOff) aboveSlideNoteMove.Add(AboveKujoSlideNotes[indexNum]);
                else break;
                
                indexNum++;
            }
            indexNum = 0;

            // BossGimmickにて_reilasHoldを変更
            foreach (var hold in reilasKujoHold) 
            {
                if (hold.Head.JudgeTime < noteAddCutOff) holdNoteMove.Add(HoldKujoNotes[indexNum]);
                else break;
                
                indexNum++;
            }
        }
        else
        {
            aboveTapMove = AboveTapNotes.Where(note => note.aboveTapTime - audioTime < 5f);
            tapNoteMove = TapNotes.Where(note => note.tapTime - audioTime < 5f);
            chainNoteMove = AboveChainNotes.Where(note => note.aboveChainTime - audioTime < 5f);
            connectorMove = NoteConnectors.Where(note => note.judgeTime - audioTime < 5f);

            foreach (var aboveHold in reilasAboveHold)
            {
                if (aboveHold.Head.JudgeTime < noteAddCutOff)
                {
                    aboveHoldNoteMove.Add(AboveHoldNotes[indexNum]);
                }
                else break;
                indexNum++;
            }
            indexNum = 0;

            foreach (var aboveSlide in reilasAboveSlide)
            {
                if (aboveSlide.Head.JudgeTime < noteAddCutOff)
                {
                    try
                    {
                        aboveSlideNoteMove.Add(AboveSlideNotes[indexNum]);
                    }
                    catch
                    {
                        break;
                    }
                }
                else break;
                
                indexNum++;
            }
            indexNum = 0;

            // BossGimmickにて_reilasHoldを変更
            foreach (var hold in reilasHold)
            {
                try
                {
                    if (hold.Head.JudgeTime < noteAddCutOff) holdNoteMove.Add(HoldNotes[indexNum]);
                    else break;
                    
                    indexNum++;
                }
                catch
                {
                    break;
                }
            }
        }

        foreach (var note in HoldEffectors)
        {
            if (audioTime - note.holdEffectTime >= 0) note.Render(audioTime, _longPerfect);
            else break;
        }

        foreach (var note in AboveHoldEffectors)
        {
            if (audioTime - note.aboveHoldEffectTime >= 0) note.Render(audioTime, _longPerfect);
            else break;
        }

        foreach (var note in AboveSlideEffectors)
        {
            if (audioTime - note.aboveSlideEffectTime >= 0) note.Render(audioTime, _longPerfect);
            else break;
        }

        foreach (var tapNote in tapNoteMove) tapNote.Render(audioTime);

        foreach (var note in aboveTapMove) note.Render(audioTime);

        foreach (var note in chainNoteMove) note.Render(audioTime);

        for (var num = holdNoteMove.Count - 1; num >= 0; num--) holdNoteMove[num].Render(audioTime, num, reilasHold);

        for (var num = aboveHoldNoteMove.Count - 1; num >= 0; num--) aboveHoldNoteMove[num].Render(audioTime, num, reilasAboveHold);

        for (var num = aboveSlideNoteMove.Count - 1; num >= 0; num--) aboveSlideNoteMove[num].Render(audioTime, num, reilasAboveSlide);

        foreach (var barLine in barLineMove.ToList()) barLine.Render(audioTime);

        foreach (var connector in connectorMove.ToList()) connector.Render(audioTime);

        if (!isHolding) _longPerfect.Pause();
        
        isHolding = false;
    }

    private IEnumerator Tutorial()
    {
        tutorialText.text = "さあ、やってきました！\nここで一度プレイしてみましょう。";
        while (true)
        {
            TutorialText();
            yield return new WaitForEndOfFrame(); //Render終了と描写の間まで待つ
        }
    }
    private void TutorialText()
    {
        if (audioTime < 7.5f) return;
        if (audioTime < 16.875f) tutorialText.text = "最初は Tap-Note \n同時に二つ押すこともあるから注意して...";
        else if (audioTime < 30f) tutorialText.text = "";
        else if (audioTime < 33.75f) tutorialText.text = "次は Hold-Note\n離してはダメですよ！";
        else if (audioTime < 45f) tutorialText.text = "";
        else if (audioTime < 48.75f) tutorialText.text = "さあ、次に Above-Tap \nここからのノーツは上に流れてきます";
        else if (audioTime < 60f) tutorialText.text = "";
        else if (audioTime < 63.75f) tutorialText.text = "次は Above-Slide\nまずは簡単なものから";
        else if (audioTime < 75f) tutorialText.text = "";
        else if (audioTime < 78.75f) tutorialText.text = "Above-Slide は動くノーツです\n指は離さずにずらして...";
        else if (audioTime < 90f) tutorialText.text = "";
        else if (audioTime < 93.75f) tutorialText.text = "最後に Chain-Note\nSlide-Note と同じ押し方で大丈夫。";
        else if (audioTime < 108.75f) tutorialText.text = "";
        else if (audioTime < 116.25f) tutorialText.text = "お疲れ様。\nこれでチュートリアルは終了です。";
        else if (audioTime < 121.875f) tutorialText.text = "難しい曲もあるけど頑張って。";
        else if (audioTime < 129f) tutorialText.text = "健闘を祈っています...";
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
}
