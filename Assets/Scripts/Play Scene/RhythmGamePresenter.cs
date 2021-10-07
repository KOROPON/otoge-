#nullable enable

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Rhythmium;
using UnityEngine;
using Reilas;
using UnityEngine.UI;
using System;

public class HeadGuide
{
    public int indexNum;
    public float time;
}

public sealed class RhythmGamePresenter : MonoBehaviour
{
    private GetHighScores _getHighScores;
    
    public Text text2;
    public AudioSource songAudio = null!;

    public static int currentHighScore;

    [SerializeField] private TapNote _tapNotePrefab = null!;
    [SerializeField] private HoldNote _holdNotePrefab = null!;
    [SerializeField] private AboveTapNote _aboveTapNotePrefab = null!;
    [SerializeField] private AboveChainNote _aboveChainNotePrefab = null!;
    [SerializeField] private AboveHoldNote _aboveHoldNotePrefab = null!;
    [SerializeField] private AboveSlideNote _aboveSlideNotePrefab = null!;
    [SerializeField] private BarLine _barLinePrefab = null!;

    public GameObject button;
    public Canvas canvas;

    [SerializeField] private GameObject _keyBeamPrefab = null!;
    private List<GameObject> allKeyBeam = new List<GameObject>();
        
    [SerializeField] private static AudioSource _audioSource = null!;
    public static AudioSource longPerfect = null!;

    public static List<TapNote> _tapNotes = new List<TapNote>();
    public static List<AboveTapNote> _aboveTapNotes = new List<AboveTapNote>();
    public static List<AboveChainNote> _aboveChainNotes = new List<AboveChainNote>();
    public static List<HoldNote> _holdNotes = new List<HoldNote>();
    public static List<AboveHoldNote> _aboveHoldNotes = new List<AboveHoldNote>();
    public static List<AboveSlideNote> _aboveSlideNotes = new List<AboveSlideNote>();

    private List<ReilasNoteEntity> reilasTap = new List<ReilasNoteEntity>();
    private List<ReilasNoteEntity> reilasAboveTap = new List<ReilasNoteEntity>();
    private List<ReilasNoteLineEntity> reilasAboveSlide = new List<ReilasNoteLineEntity>();
    private List<ReilasNoteLineEntity> reilasAboveHold = new List<ReilasNoteLineEntity>();
    private List<ReilasNoteLineEntity> reilasHold = new List<ReilasNoteLineEntity>();
    private List<ReilasNoteEntity> reilasChain = new List<ReilasNoteEntity>();

    public static List<BarLine> _barLines = new List<BarLine>();

    public static List<ReilasNoteEntity> tapNotes = new List<ReilasNoteEntity>();
    public static List<ReilasNoteEntity> internalNotes = new List<ReilasNoteEntity>();
    public static List<ReilasNoteEntity> chainNotes = new List<ReilasNoteEntity>();

    public static int countNotes;
    
    //Effect用
    public static List<HoldEffector> _holdEffectors = new List<HoldEffector>();
    public static List<AboveHoldEffector> _aboveHoldEffectors = new List<AboveHoldEffector>();
    public static List<AboveSlideEffector> _aboveSlideEffectors = new List<AboveSlideEffector>();

    //Judge用
    public static bool[] tapNoteJudge = Array.Empty<bool>();
    public static bool[] internalNoteJudge = Array.Empty<bool>();
    public static bool[] chainNoteJudge = Array.Empty<bool>();
    
    private ReilasChartEntity _chartEntity = null!;

    public static string musicname = null!;
    public static string dif = null!;
    
    public static bool[] laneTapStates = new bool[36];

    List<HeadGuide> aboveHoldHead = new List<HeadGuide>();
    List<HeadGuide> aboveSlideHead = new List<HeadGuide>();
    List<HeadGuide> holdHead = new List<HeadGuide>();
    

    JudgeService judgeService;

    float judgeTime;
    float audioTime;

    static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    /// <summary>
    /// 判定結果を処理する
    /// </summary>

    private List<float> _barLineTimes = BarLine.BarLines;

    private IEnumerable<ReilasNoteEntity> GetNoteTypes(ReilasChartEntity chart, string type)
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
    
    private void Awake()
    {
        _getHighScores = gameObject.AddComponent<GetHighScores>();
        judgeService = new JudgeService();
        AwakeAsync().Forget();

        /*
        foreach(var a in screenPoints)
        {
            var b = Instantiate(button);
            b.transform.SetParent(canvas.transform,false);
            b.transform.position = new Vector3(a.x,a.y,a.z);
        }
        */
    }
    
    private async UniTask AwakeAsync()
    {
        //FindObjectOfType<Variable>().enabled = false;

        var chartTextAsset = await Resources.LoadAsync<TextAsset>("Charts/" + musicname + "." + dif) as TextAsset;

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

        if (_audioSource.clip != null) BarLine.GetBarLines(musicname, _audioSource.clip.length);


        if (PlayerPrefs.HasKey("volume"))
        {
        // tap音調整
        }



        // chartEntity
        _chartEntity = chartEntity;

        tapNotes = new List<ReilasNoteEntity>(GetNoteTypes(_chartEntity, "Tap"));
        internalNotes = new List<ReilasNoteEntity>(GetNoteTypes(_chartEntity, "Internal"));
        chainNotes = new List<ReilasNoteEntity>(GetNoteTypes(_chartEntity, "Chain"));
        
        tapNoteJudge = new bool[tapNotes.Count];
        chainNoteJudge = new bool[chainNotes.Count];
        reilasTap = _chartEntity.Notes.Where(note => note.Type == NoteType.Tap).ToList();
        reilasAboveTap = _chartEntity.Notes.Where(note => note.Type == NoteType.AboveTap).ToList();
        reilasAboveSlide = _chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveSlide).ToList();
        reilasAboveHold = _chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveHold).ToList();
        reilasHold = _chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.Hold).ToList();
        reilasChain = _chartEntity.Notes.Where(note => note.Type == NoteType.AboveChain).ToList();

        SpawnChainNotes(reilasChain);
        SpawnHoldNotes(reilasHold);
        SpawnAboveHoldNotes(reilasAboveHold);
        SpawnAboveSlideNotes(reilasAboveSlide);
        SpawnBarLines(_barLineTimes);

        foreach (var note in tapNotes)
        {
            switch (note.Type)
            {
                case NoteType.AboveSlide:
                    {
                        foreach (var jsonData in noteJsonData)
                        {
                            if (note.JsonData.guid != jsonData.tail)
                            {/*
                                aboveSlideHead.Add(new HeadGuide
                                {
                                    time = note.JudgeTime,
                                    indexNum = _chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveSlide).IndexOf(note)
                                }) ;*/
                                continue;
                            }
                            note.Type = NoteType.AboveSlideInternal;
                            internalNotes.Add(note);
                            break;
                        }

                        break;
                    }
                case NoteType.AboveHold:
                    {
                        foreach (var jsonData in noteJsonData)
                        {
                            if (note.JsonData.guid != jsonData.tail)
                            {/*
                                aboveHoldHead.Add(new HeadGuide
                                {
                                    time = note.JudgeTime,
                                    indexNum = _chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveHold).IndexOf(note)
                                });*/
                                continue;
                            }
                            note.Type = NoteType.AboveHoldInternal;
                            internalNotes.Add(note);
                            break;
                        }

                        break;
                    }
                case NoteType.Hold:
                    {
                        foreach (var jsonData in noteJsonData)
                        {
                            if (note.JsonData.guid != jsonData.tail)
                            {/*
                                holdHead.Add(new HeadGuide
                                {
                                    time = note.JudgeTime,
                                    indexNum = _chartEntity.NoteLines.Where(note => note.Head.Type == NoteType.Hold).IndexOf(note)
                                });*/
                                continue;
                            }
                            note.Type = NoteType.HoldInternal;
                            internalNotes.Add(note);
                            break;
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

        internalNotes.OrderBy(note => note.JudgeTime);
        internalNoteJudge = new bool[internalNotes.Count];

        countNotes = tapNoteJudge.Length + internalNoteJudge.Length + chainNoteJudge.Length;
        
        SpawnTapNotes(GetNoteTypes(_chartEntity, "GroundTap"));
        SpawnAboveTapNotes(GetNoteTypes(_chartEntity, "AboveTap"));
        
        Shutter.bltoPlay = true;
        Shutter.blShutterChange = "Open";//シーンを開く
    }

    public static void PlaySongs()
    {
        _audioSource.Play();
        stopwatch.Start();
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
            var holdEffector = tapNote.transform.Find("HoldEffector").gameObject.GetComponent<HoldEffector>();
            tapNote.Initialize(note);
            tapNote.gameObject.SetActive(false);
            holdEffector.EffectorInitialize(note);
            _holdEffectors.Add(holdEffector);
            _holdNotes.Add(tapNote);
        }
    }

    private void SpawnAboveHoldNotes(IEnumerable<ReilasNoteLineEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(_aboveHoldNotePrefab);
            var aboveHoldEffector = tapNote.transform.Find("AboveHoldEffector").gameObject.GetComponent<AboveHoldEffector>();
            tapNote.Initialize(note);
            tapNote.gameObject.SetActive(false);
            aboveHoldEffector.EffectorInitialize(note);
            _aboveHoldEffectors.Add(aboveHoldEffector);
            _aboveHoldNotes.Add(tapNote);
        }
    }
    private void SpawnAboveSlideNotes(IEnumerable<ReilasNoteLineEntity> notes)
    {
        foreach (var note in notes)
        {
            var tapNote = Instantiate(_aboveSlideNotePrefab);
            var aboveSlideEffector = tapNote.transform.Find("AboveSlideEffector").gameObject.GetComponent<AboveSlideEffector>();
            tapNote.Initialize(note);
            tapNote.gameObject.SetActive(false);
            aboveSlideEffector.EffectorInitialize(note);
            _aboveSlideEffectors.Add(aboveSlideEffector);
            _aboveSlideNotes.Add(tapNote);
        }
    }

    private void SpawnBarLines(IEnumerable<float> lines)
    {
        foreach (var line in lines)
        {
            var barLine = Instantiate(_barLinePrefab);
            barLine.gameObject.SetActive(false);
            _barLines.Add(barLine);
        }
    }

    static float z = -0.9f;
    public static readonly Vector3[] LanePositions = new Vector3[]
    {
        //下のレーン
        new Vector3(-3f, 0, z),
        new Vector3(-1f, 0, z),
        new Vector3(1f, 0,z),
        new Vector3(3f, 0, z),

        //上のレーン
        new Vector3(-4.5f,0.22f,z),
        new Vector3(-4.45f,0.66f,z),
        new Vector3(-4.36f,1.09f,z),
        new Vector3(-4.23f,1.51f,z),
        new Vector3(-4.06f,1.92f,z),
        new Vector3(-3.86f,2.31f,z),
        new Vector3(-3.61f,2.68f,z),
        new Vector3(-3.33f,3.02f,z),
        new Vector3(-3.02f,3.33f,z),
        new Vector3(-2.68f,3.61f,z),
        new Vector3(-2.31f,3.86f,z),
        new Vector3(-1.92f,4.06f,z),
        new Vector3(-1.51f,4.23f,z),
        new Vector3(-1.09f,4.36f,z),
        new Vector3(-0.66f,4.45f,z),
        new Vector3(-0.22f,4.5f,z),
        new Vector3(0.22f,4.5f,z),
        new Vector3(0.66f,4.45f,z),
        new Vector3(1.09f,4.36f,z),
        new Vector3(1.51f,4.23f,z),
        new Vector3(1.92f,4.06f,z),
        new Vector3(2.31f,3.86f,z),
        new Vector3(2.68f,3.61f,z),
        new Vector3(3.02f,3.33f,z),
        new Vector3(3.33f,3.02f,z),
        new Vector3(3.61f,2.68f,z),
        new Vector3(3.86f,2.31f,z),
        new Vector3(4.06f,1.92f,z),
        new Vector3(4.23f,1.51f,z),
        new Vector3(4.36f,1.09f,z),
        new Vector3(4.45f,0.66f,z),
        new Vector3(4.5f,0.22f,z),
    };

     IEnumerable<Vector3> screenPoints = LanePositions.Select(lanePosition3D => Camera.main.WorldToScreenPoint(lanePosition3D));// Camera.main.WorldToScreenPoint(lanePosition3D))  "レーンの位置を"2D変換  //

    private void Update()
    {
        if (_audioSource == null)
        {
          return;
        }
    
        InputService.AboveLaneTapStates.Clear();
        for (var i = 0; i < laneTapStates.Length; i++)
        {
            laneTapStates[i] = false;
        }
        
        var allTouch = Input.touches;
        Array.Resize(ref allTouch,0);
        var touches = Input.touches;


        foreach (var touch in touches)
        {
            //gameObject.transform.position = new Vector3()
            var lane = screenPoints.Select((screenPoint, index) => (screenPoint, index))
                .OrderBy(screenPoint => Vector2.Distance(screenPoint.screenPoint, touch.position)).First();
            var distance = Vector2.Distance(lane.screenPoint, touch.position);
            var nearestLaneIndex = distance < 150 ? lane.index : 40;//押した場所に一番近いレーンの番号
            text2.text = nearestLaneIndex.ToString();
            //Debug.Log(nearestLaneIndex);
            bool start = touch.phase == TouchPhase.Began;
            // touch.position
            // このフレームで押されたよん
            
            if (nearestLaneIndex < 36)
            {
                laneTapStates[nearestLaneIndex] = true;
            }
            else
            {
                continue;
            }
            
            InputService.AboveLaneTapStates.Add(new LaneTapState{laneNumber = nearestLaneIndex, tapStarting = start});
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

        for(int keyIndex = allKeyBeam.Count() - 1; keyIndex >= 0; keyIndex--)
        {
            Destroy(allKeyBeam[keyIndex].gameObject);
            allKeyBeam.RemoveAt(keyIndex);
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
            var keyBeam = Instantiate(_keyBeamPrefab);
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
            allKeyBeam.Add(keyBeam);
            dupLane.Add(laneNum);
        }

      //  Debug.Log(judgeTime + "   " + stopwatch.Elapsed);

        judgeService.Judge(judgeTime);

    }

    private void LateUpdate()
    {
        var _aboveTapMove = _aboveTapNotes.Where(note => note.aboveTapTime - audioTime < 5f);
        var _tapNoteMove = _tapNotes.Where(note => note._tapTime - audioTime < 5f);
        var _chainNoteMove = _aboveChainNotes.Where(note => note.aboveChainTime - audioTime < 5f);

        List<AboveHoldNote> _aboveHoldNoteMove = new List<AboveHoldNote>();
        List<AboveSlideNote> _aboveSlideNoteMove = new List<AboveSlideNote>();
        List<HoldNote> _holdNoteMove = new List<HoldNote>();
        int indexNum = 0;
        foreach(var aboveHold in reilasAboveHold)
        {
            if (aboveHold.Head.JudgeTime - audioTime < 5) // tail の時間 - currentTime < 5s の時 setActive => true & Render()
            {
                _aboveHoldNoteMove.Add(_aboveHoldNotes[indexNum]);
            }
            else
            {
                break;
            }
            indexNum++;
        }
        indexNum = 0;
        foreach(var aboveSlide in reilasAboveSlide)
        {
            if (aboveSlide.Head.JudgeTime - audioTime < 5) // tail の時間 - currentTime < 5s の時 setActive => true & Render()
            {
                _aboveSlideNoteMove.Add(_aboveSlideNotes[indexNum]);
            }
            else
            {
                break;
            }
            indexNum++;
        }
        indexNum = 0;
        foreach(var hold in reilasHold)
        {
            if (hold.Head.JudgeTime - audioTime < 5) // tail の時間 - currentTime < 5s の時 setActive => true & Render()
            {
                _holdNoteMove.Add(_holdNotes[indexNum]);
            }
            else
            {
                break;
            }
            indexNum++;
        }

        foreach (var tapNote in _tapNoteMove)
        {
            tapNote.Render(audioTime);
        }

        foreach (var note in _aboveTapMove)
        {
            note.Render(audioTime);
        }

        foreach (var note in _chainNoteMove)
        {
            note.Render(audioTime);
        }

        for (int num = _holdNoteMove.Count - 1; num >= 0; num--)
        {
            _holdNoteMove[num].Render(audioTime, num, reilasHold);
        }

        for (int num = _aboveHoldNoteMove.Count - 1; num >= 0; num--)
        {
            _aboveHoldNoteMove[num].Render(audioTime, num, reilasAboveHold);
        }

        for (int num = _aboveSlideNoteMove.Count - 1; num >= 0; num--)
        {
            _aboveSlideNoteMove[num].Render(audioTime, num, reilasAboveSlide);
        }

        foreach (var note in _holdEffectors)
        {
            note.EffectJudge(audioTime, longPerfect);
        }

        foreach (var note in _aboveHoldEffectors)
        {
            note.Render(audioTime, longPerfect);
        }

        foreach (var note in _aboveSlideEffectors)
        {
            note.Render(audioTime, longPerfect);
        }
        for (int i = 0; i < _barLines.Count; i++)
        {
            _barLines[i].Render(_barLineTimes[i], audioTime);
        }

        if (!(_audioSource.time > 0) || _audioSource.isPlaying) return;
        currentHighScore = _getHighScores.GetHighScore(musicname, dif);
        _getHighScores.SetHighScore(musicname, dif, ScoreComboCalculator.currentScore);
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
    public static readonly List<LaneTapState> AboveLaneTapStates = new List<LaneTapState>();

    // 下レーン 最大 4 つ
    //public static List<LaneTapState> LaneTapStates = new List<LaneTapState>();
}
