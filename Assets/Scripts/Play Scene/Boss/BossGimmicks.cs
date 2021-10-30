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

        if (time < 82.5f) return;

        if (time < 83f)
        {

            return;
        }
        else if (time < 93f)
        {

        }
        else if (time < 101.8f)
        {

        }
        else if (time < 103.2f)
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
        // 82.5s  �ڍs�����̘A�ŉ�
        // 83s �@ ���ԕ���
        // 93s    ���ڃ��[�v
        // 101.8s �K���X��
        // 103.2s ���X�T�r
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