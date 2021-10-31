#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Reilas;
using Rhythmium;
using UnityEngine;

public class BossGimmicks : MonoBehaviour
{
    private RhythmGamePresenter _presenter = null!;

    private static List<ReilasNoteEntity> _tapKujoNotes = new List<ReilasNoteEntity>();
    private static List<ReilasNoteEntity> _internalKujoNotes = new List<ReilasNoteEntity>();
    private static List<ReilasNoteEntity> _chainKujoNotes = new List<ReilasNoteEntity>();
    public List<ReilasNoteLineEntity> reilasKujoAboveSlide = new List<ReilasNoteLineEntity>();
    public List<ReilasNoteLineEntity> reilasKujoAboveHold = new List<ReilasNoteLineEntity>();
    public List<ReilasNoteLineEntity> reilasKujoHold = new List<ReilasNoteLineEntity>();
    public List<ReilasNoteEntity> reilasKujoChain = new List<ReilasNoteEntity>();

    public bool kujoJudgeSwitch = false;

    public AudioSource bossClock;

    public new Camera? camera;

    private void Awake()
    {
        camera = Camera.main;
        _presenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
    }

    public async void BossAwake()
    {
        var kujoSongs = await Resources.LoadAsync<TextAsset>("Charts/Reilas_half.KUJO") as TextAsset;
        if (kujoSongs == null)
        {
            Debug.LogError("Reilas_KUJO ���ʃf�[�^��������܂���ł���");
            return;
        }

        var chartKujoJsonData = JsonUtility.FromJson<ChartJsonData>(kujoSongs.text);
        var chartKujoEntity = new ReilasChartConverter().Convert(chartKujoJsonData);

        NoteLineJsonData[] noteKujoJsonData = chartKujoJsonData.timeline.noteLines;

        _tapKujoNotes = new List<ReilasNoteEntity>(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "Tap"));
        _internalKujoNotes = new List<ReilasNoteEntity>(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "Internal"));
        _chainKujoNotes = new List<ReilasNoteEntity>(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "Chain"));

        reilasKujoAboveSlide = chartKujoEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveSlide).ToList();
        reilasKujoAboveHold = chartKujoEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveHold).ToList();
        reilasKujoHold = chartKujoEntity.NoteLines.Where(note => note.Head.Type == NoteType.Hold).ToList();
        reilasKujoChain = chartKujoEntity.Notes.Where(note => note.Type == NoteType.AboveChain).ToList();

        List<int> removeInt = new List<int>();

        foreach (ReilasNoteEntity note in _tapKujoNotes)
        {
            if (kujoSongs == null)
            {
                switch (note.Type)
                {
                    case NoteType.AboveSlide:
                        {
                            if (noteKujoJsonData.Any(jsonData => note.JsonData.guid == jsonData.tail))
                            {
                                note.Type = NoteType.AboveSlideInternal;
                                _internalKujoNotes.Add(note);
                                removeInt.Add(_tapKujoNotes.IndexOf(note));
                            }

                            break;
                        }
                    case NoteType.AboveHold:
                        {
                            if (noteKujoJsonData.Any(jsonData => note.JsonData.guid == jsonData.tail))
                            {
                                note.Type = NoteType.AboveSlideInternal;
                                _internalKujoNotes.Add(note);
                                removeInt.Add(_tapKujoNotes.IndexOf(note));
                            }

                            break;
                        }
                    case NoteType.Hold:
                        {
                            if (noteKujoJsonData.Any(jsonData => note.JsonData.guid == jsonData.tail))
                            {
                                note.Type = NoteType.AboveSlideInternal;
                                _internalKujoNotes.Add(note);
                                removeInt.Add(_tapKujoNotes.IndexOf(note));
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
        }

        for (int num = removeInt.Count - 1; num >= 0; num--)
        {
            _tapKujoNotes.RemoveAt(removeInt[num]);
        }


        _presenter.SpawnTapNotes(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "GroundTap"), true);
        _presenter.SpawnAboveTapNotes(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "AboveTap"), true);
        _presenter.SpawnChainNotes(reilasKujoChain, true);
        _presenter.SpawnHoldNotes(reilasKujoHold, true);
        _presenter.SpawnAboveHoldNotes(reilasKujoAboveHold, true);
        _presenter.SpawnAboveSlideNotes(reilasKujoAboveSlide, true);
    }

    void Update()
    {
        if (!_presenter.jumpToKujo) return;


        float time = _presenter.audioTime;

        if (time < 81.7f) return;

        if (time < 82.93f)
        {

            return;
        }
        else if (time < 92.98f)
        {
            if (!bossClock.isPlaying) bossClock.Play();
        }
        else if (time < 101.78f)
        {
            kujoJudgeSwitch = true;

        }
        else if (time < 103.04f)
        {
            kujoJudgeSwitch = false;

        }
        else if (time < 104)
        {

        }
        else
        {
            return;
        }

        List<Vector3> cameraPos = CameraPosCalculator.CameraPosCalculatorService(time, Camera.main.transform.rotation.z);
        camera.transform.position = cameraPos[0];
        camera.transform.eulerAngles = cameraPos[1];
    }

    public void ChangeToKujo()
    {
        // 81.7s  �ڍs�����̘A�ŉ�
        // 82.93s �@ ���ԕ���
        // 92.98s    ���ڃ��[�v
        // 101.78s �K���X��
        // 103.04s ���X�T�r
        for (int i = RhythmGamePresenter.TapNotes.Count() - 1; i >= 0; i--)
        {
            RhythmGamePresenter.TapNotes[i].NoteDestroy(false);
        }

        for (int i = RhythmGamePresenter.HoldNotes.Count() - 1; i >= 0; i--)
        {
            RhythmGamePresenter.HoldNotes[i].NoteDestroy(false);
        }

        for (int i = RhythmGamePresenter.AboveTapNotes.Count() - 1; i >= 0; i--)
        {
            RhythmGamePresenter.AboveTapNotes[i].NoteDestroy(false);
        }

        for (int i = RhythmGamePresenter.AboveHoldNotes.Count() - 1; i >= 0; i--)
        {
            RhythmGamePresenter.AboveHoldNotes[i].NoteDestroy(false);
        }

        for (int i = RhythmGamePresenter.AboveSlideNotes.Count() - 1; i >= 0; i--)
        {
            RhythmGamePresenter.AboveSlideNotes[i].NoteDestroy(false);
        }

        for (int i = RhythmGamePresenter.AboveChainNotes.Count() - 1; i >= 0; i--)
        {
            RhythmGamePresenter.AboveChainNotes[i].NoteDestroy(false);
        }
    }

    public void NotChangeToKujo()
    {
        for(int i = RhythmGamePresenter.TapKujoNotes.Count() - 1; i >= 0; i--)
        {
            RhythmGamePresenter.TapKujoNotes[i].NoteDestroy(true);
        }

        for(int i = RhythmGamePresenter.HoldKujoNotes.Count() - 1; i >= 0; i--)
        {
            RhythmGamePresenter.HoldKujoNotes[i].NoteDestroy(true);
        }

        for(int i = RhythmGamePresenter.AboveKujoTapNotes.Count() - 1; i >= 0; i--)
        {
            RhythmGamePresenter.AboveKujoTapNotes[i].NoteDestroy(true);
        }

        for(int i = RhythmGamePresenter.AboveKujoHoldNotes.Count() - 1; i >= 0; i--)
        {
            RhythmGamePresenter.AboveKujoHoldNotes[i].NoteDestroy(true);
        }

        for(int i = RhythmGamePresenter.AboveKujoSlideNotes.Count() - 1; i >= 0; i--)
        {
            RhythmGamePresenter.AboveKujoSlideNotes[i].NoteDestroy(true);
        }

        for(int i = RhythmGamePresenter.AboveKujoChainNotes.Count() - 1; i >= 0; i--)
        {
            RhythmGamePresenter.AboveKujoChainNotes[i].NoteDestroy(true);
        }
    }
}