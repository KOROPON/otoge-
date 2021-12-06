#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Reilas;
using Rhythmium;
using UnityEngine;
using UnityEngine.UI;

namespace Boss
{
    public class BossGimmicks : MonoBehaviour
    {
        private AllJudgeService _judgeService = null!;
        public AudioSource bossClock = null!;
        private BossGimmickContainer _bossContainer = null!;
        private Image _whiteOut = null!;
        private MeshRenderer _tunnel = null!;
        private MeshRenderer _underTunnel = null!;
        private MeshRenderer _judgeLine = null!;
        private RectTransform _backGround = null!;
        private RhythmGamePresenter _presenter = null!;

        public Material bossLane = null!;
        public Material bossCube = null!;
        
        public new Camera camera = null!;

        private bool _gaugeCheck;
        private bool _firstAnimCheck;
        
        private byte _tunnelAlpha;

        public List<ReilasNoteEntity> tapKujoNotes = new List<ReilasNoteEntity>();
        public List<ReilasNoteEntity> internalKujoNotes = new List<ReilasNoteEntity>();
        public List<ReilasNoteEntity> chainKujoNotes = new List<ReilasNoteEntity>();
        
        public bool kujoJudgeSwitch = false;
        public bool gimmickPause;

        [SerializeField] private byte color = 0;
        
        private void Awake()
        {
            if (Camera.main != null) camera = Camera.main;
            
            _gaugeCheck = true;
            gimmickPause = false;
            _presenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
            _bossContainer = gameObject.GetComponent<BossGimmickContainer>();
            _tunnel = GameObject.Find("立方体").GetComponent<MeshRenderer>();
            _underTunnel = GameObject.Find("トンネル").GetComponent<MeshRenderer>();
            _backGround = GameObject.Find("BackGround").GetComponent<RectTransform>();
            _judgeLine = GameObject.Find("AboveJudgeLine").GetComponent<MeshRenderer>();
            _whiteOut = GameObject.Find("WhiteOut").GetComponent<Image>();
            _underTunnel = GameObject.Find("立方体").transform.GetComponent<MeshRenderer>();
            _tunnel = GameObject.Find("トンネル").transform.GetComponent<MeshRenderer>();
            _tunnelAlpha = 255;
        }

        public async void BossAwake()
        {
            _judgeService = GameObject.Find("Main").GetComponent<AllJudgeService>();

            var kujoSongs = await Resources.LoadAsync<TextAsset>("Charts/Reilas_half.KUJO") as TextAsset;
            
            if (kujoSongs == null) return;

            var chartKujoJsonData = JsonUtility.FromJson<ChartJsonData>(kujoSongs.text);
            var chartKujoEntity = new ReilasChartConverter().Convert(chartKujoJsonData);

            NoteLineJsonData[] noteKujoJsonData = chartKujoJsonData.timeline.noteLines;

            tapKujoNotes = new List<ReilasNoteEntity>(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "Tap"));
            internalKujoNotes = new List<ReilasNoteEntity>(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "Internal"));
            chainKujoNotes = new List<ReilasNoteEntity>(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "Chain"));
            _presenter.reilasKujoAboveSlide = chartKujoEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveSlide).ToList();
            _presenter.reilasKujoAboveHold = chartKujoEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveHold).ToList();
            _presenter.reilasKujoHold = chartKujoEntity.NoteLines.Where(note => note.Head.Type == NoteType.Hold).ToList();
            _presenter.reilasKujoChain = chartKujoEntity.Notes.Where(note => note.Type == NoteType.AboveChain).OrderBy(note => note.JudgeTime).ToList();

            List<int> removeInt = new List<int>();

            foreach (var note in tapKujoNotes.Where(note => kujoSongs == null))
            {
                switch (note.Type)
                {
                    case NoteType.AboveSlide:
                    {
                        if (noteKujoJsonData.Any(jsonData => note.JsonData.guid == jsonData.tail))
                        {
                            note.Type = NoteType.AboveSlideInternal;
                            
                            internalKujoNotes.Add(note);
                            removeInt.Add(tapKujoNotes.IndexOf(note));
                        }

                        break;
                    }
                    case NoteType.AboveHold:
                    {
                        if (noteKujoJsonData.Any(jsonData => note.JsonData.guid == jsonData.tail))
                        {
                            note.Type = NoteType.AboveSlideInternal;
                            
                            internalKujoNotes.Add(note);
                            removeInt.Add(tapKujoNotes.IndexOf(note));
                        }

                        break;
                    }
                    case NoteType.Hold:
                    {
                        if (noteKujoJsonData.Any(jsonData => note.JsonData.guid == jsonData.tail))
                        {
                            note.Type = NoteType.AboveSlideInternal;
                            
                            internalKujoNotes.Add(note);
                            removeInt.Add(tapKujoNotes.IndexOf(note));
                        }

                        break;
                    }
                    case NoteType.None:
                    case NoteType.Tap:
                    case NoteType.HoldInternal:
                    case NoteType.AboveTap:
                    case NoteType.AboveHoldInternal:
                    case NoteType.AboveSlideInternal:
                    case NoteType.AboveChain:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            for (var num = removeInt.Count - 1; num >= 0; num--) tapKujoNotes.RemoveAt(removeInt[num]);

            _presenter.SpawnTapNotes(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "GroundTap"), true);
            _presenter.SpawnAboveTapNotes(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "AboveTap"), true);
            _presenter.SpawnChainNotes(_presenter.reilasKujoChain, true);
            _presenter.SpawnHoldNotes(_presenter.reilasKujoHold, true);
            _presenter.SpawnAboveHoldNotes(_presenter.reilasKujoAboveHold, true);
            _presenter.SpawnAboveSlideNotes(_presenter.reilasKujoAboveSlide, true);
        }


        private void Update()
        {
            if (!(RhythmGamePresenter.jumpToKujo && _presenter.alreadyChangeKujo)) return;
            if (gimmickPause) return;

            var time = _presenter.audioTime;
            switch (time)
            {
                case var t when t > 120:
                    return;
                case var t when t < 92.98f:
                {
                    if (_tunnelAlpha > 9) _tunnelAlpha -= 10;
                    else _tunnelAlpha = 0;
                    
                    _tunnel.material.color = new Color32(0, 0, 40, _tunnelAlpha);
                    _underTunnel.material.color = new Color32(0, 0, 0, _tunnelAlpha);
                    
                    var effectTime = Mathf.Abs((time - 82.93f) % 1.257f);

                    if (effectTime <= 0.057f && _firstAnimCheck)
                        StartCoroutine(_bossContainer.BlackOutIntermittently());

                    if (_firstAnimCheck) return;
                    StartCoroutine(_bossContainer.BlackOutIntermittentlyFirst());
                    bossClock.Play();
                    _bossContainer.BlackOutFirst();
                    
                    _firstAnimCheck = true;
                    
                    return;
                }
                case var t when t < 101.78f:
                {
                    kujoJudgeSwitch = true;
                    var effectTime = Mathf.Abs((time - 82.93f) % 0.628f);
                    
                    if (effectTime <= 0.028f) StartCoroutine(_bossContainer.BlackOutIntermittently());
                    
                    _bossContainer._cameraShine.intensity.value += 0.5f;
                    
                    var colorReset = (byte)Mathf.Lerp(0, 225, (time - 92.98f) / 8.8f);
                    _whiteOut.color = new Color32(255, 255, 255, colorReset);
                    
                    break;
                }
                case var t when t < 103.04f:
                {
                    var colorReset = (byte)Mathf.Lerp(255, 0, (time - 101.78f) / 1.26f);
                    _whiteOut.color = new Color32(255, 255, 255, colorReset);
                    
                    _bossContainer.ChangeObjectShine();
                    
                    break;
                }
                case var t when t < 104:
                {
                    if (_gaugeCheck)
                    {
                        _whiteOut.color = new Color32(255, 255, 255, 0);
                        
                        _bossContainer.LastChorus();
                        _judgeLine.gameObject.SetActive(true);
                        
                        kujoJudgeSwitch = false;
                        
                        GameObject.Find("Main").transform.GetComponent<ScoreComboCalculator>().GaugeChange();
                        
                        _gaugeCheck = false;
                    }
                    var timeRatio = time - 103.04f;
                    var size = Mathf.Lerp(40, 1, timeRatio / 16);
                    
                    _backGround.localScale = new Vector3(size, size, 1);
                    
                    break;
                }
                default:
                {
                    var timeRatio = time - 103.04f;
                    var size = Mathf.Lerp(40, 1, timeRatio / 16);
                    
                    _backGround.localScale = new Vector3(size, size, 1);
                    
                    return;
                }
            }

            if (camera == null && Camera.main != null) camera = Camera.main;

            if (camera == null) return;
            List<Vector3> cameraPos =
                CameraPosCalculator.CameraPosCalculatorService(time, camera.transform.eulerAngles.z);
            
            var transform1 = camera.transform;
            
            transform1.position = cameraPos[0];
            transform1.eulerAngles = cameraPos[1];
        }

        public void ChangeToKujo()
        {
            if (_judgeService == null) return;

            _tunnel.material = bossCube;
            _underTunnel.material = bossLane;
            
            _judgeLine.gameObject.SetActive(false);

            for (var i = 0; i < 36; i++)
            {
                if(_judgeService.tapJudgeStartIndex == null)
                {
                    continue;
                }
                
                _judgeService.tapJudgeStartIndex[i] = 0;
            }

            for (var i = RhythmGamePresenter.TapNotes.Count - 1; i >= 0; i--)
                RhythmGamePresenter.TapNotes[i].NoteDestroy(false);

            for (var i = RhythmGamePresenter.HoldNotes.Count - 1; i >= 0; i--)
                RhythmGamePresenter.HoldNotes[i].NoteDestroy(false);

            for (var i = RhythmGamePresenter.AboveTapNotes.Count - 1; i >= 0; i--)
                RhythmGamePresenter.AboveTapNotes[i].NoteDestroy(false);

            for (var i = RhythmGamePresenter.AboveHoldNotes.Count - 1; i >= 0; i--)
                RhythmGamePresenter.AboveHoldNotes[i].NoteDestroy(false);

            for (var i = RhythmGamePresenter.AboveSlideNotes.Count - 1; i >= 0; i--)
                RhythmGamePresenter.AboveSlideNotes[i].NoteDestroy(false);

            for (var i = RhythmGamePresenter.AboveChainNotes.Count - 1; i >= 0; i--)
                RhythmGamePresenter.AboveChainNotes[i].NoteDestroy(false);

            for (var i = RhythmGamePresenter.NoteConnectors.Count - 1; i >= 0; i--)
                RhythmGamePresenter.NoteConnectors[i].NoteConnectorDestroy(false);
        }

        public static void NotChangeToKujo()
        {
            for (var i = RhythmGamePresenter.TapKujoNotes.Count - 1; i >= 0; i--)
                RhythmGamePresenter.TapKujoNotes[i].NoteDestroy(true);

            for (var i = RhythmGamePresenter.HoldKujoNotes.Count - 1; i >= 0; i--)
                RhythmGamePresenter.HoldKujoNotes[i].NoteDestroy(true);

            for (var i = RhythmGamePresenter.AboveKujoTapNotes.Count - 1; i >= 0; i--)
                RhythmGamePresenter.AboveKujoTapNotes[i].NoteDestroy(true);

            for (var i = RhythmGamePresenter.AboveKujoHoldNotes.Count - 1; i >= 0; i--)
                RhythmGamePresenter.AboveKujoHoldNotes[i].NoteDestroy(true);

            for (var i = RhythmGamePresenter.AboveKujoSlideNotes.Count - 1; i >= 0; i--)
                RhythmGamePresenter.AboveKujoSlideNotes[i].NoteDestroy(true);

            for (var i = RhythmGamePresenter.AboveKujoChainNotes.Count - 1; i >= 0; i--)
                RhythmGamePresenter.AboveKujoChainNotes[i].NoteDestroy(true);

            for (var i = RhythmGamePresenter.NoteKujoConnectors.Count - 1; i >= 0; i--)
                RhythmGamePresenter.NoteKujoConnectors[i].NoteConnectorDestroy(true);
        }
    }
}