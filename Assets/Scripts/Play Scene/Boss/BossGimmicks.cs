using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Reilas;
using Rhythmium;
using UnityEngine;

public class BossGimmicks : MonoBehaviour
{
    private RhythmGamePresenter presenter = null!;

    public static List<ReilasNoteEntity> tapKujoNotes = new List<ReilasNoteEntity>();
    public static List<ReilasNoteEntity> internalKujoNotes = new List<ReilasNoteEntity>();
    public static List<ReilasNoteEntity> chainKujoNotes = new List<ReilasNoteEntity>();
    public List<ReilasNoteLineEntity> reilasKujoAboveSlide = new List<ReilasNoteLineEntity>();
    public List<ReilasNoteLineEntity> reilasKujoAboveHold = new List<ReilasNoteLineEntity>();
    public List<ReilasNoteLineEntity> reilasKujoHold = new List<ReilasNoteLineEntity>();
    public List<ReilasNoteEntity> reilasKujoChain = new List<ReilasNoteEntity>();

    public Camera camera;

    private void Awake()
    {
        camera = Camera.main;
        presenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
    }

    public async void BossAwake()
    {
        TextAsset? kujyoSongs = await Resources.LoadAsync<TextAsset>("Charts/Reilas_half.KUJO") as TextAsset;
        if (kujyoSongs == null)
        {
            Debug.LogError("Reilas_KUJO 譜面データが見つかりませんでした");
            return;
        }

        var chartKujoJsonData = JsonUtility.FromJson<ChartJsonData>(kujyoSongs.text);
        var chartKujoEntity = new ReilasChartConverter().Convert(chartKujoJsonData);

        NoteLineJsonData[] noteKujoJsonData = chartKujoJsonData.timeline.noteLines;

        tapKujoNotes = new List<ReilasNoteEntity>(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "Tap"));
        internalKujoNotes = new List<ReilasNoteEntity>(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "Internal"));
        chainKujoNotes = new List<ReilasNoteEntity>(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "Chain"));

        reilasKujoAboveSlide = chartKujoEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveSlide).ToList();
        reilasKujoAboveHold = chartKujoEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveHold).ToList();
        reilasKujoHold = chartKujoEntity.NoteLines.Where(note => note.Head.Type == NoteType.Hold).ToList();
        reilasKujoChain = chartKujoEntity.Notes.Where(note => note.Type == NoteType.AboveChain).ToList();

        List<int> removeInt = new List<int>();

        foreach (ReilasNoteEntity note in tapKujoNotes)
        {
            if (kujyoSongs == null)
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

        for (int num = removeInt.Count() - 1; num >= 0; num--)
        {
            tapKujoNotes.RemoveAt(removeInt[num]);
        }


        presenter.SpawnTapNotes(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "GroundTap"), true);
        presenter.SpawnAboveTapNotes(RhythmGamePresenter.GetNoteTypes(chartKujoEntity, "AboveTap"), true);
        presenter.SpawnChainNotes(reilasKujoChain, true);
        presenter.SpawnHoldNotes(reilasKujoHold, true);
        presenter.SpawnAboveHoldNotes(reilasKujoAboveHold, true);
        presenter.SpawnAboveSlideNotes(reilasKujoAboveSlide, true);
    }

    void Update()
    {
        if (!presenter.jumpToKujo) return;

        float time = presenter._audioTime;

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
        // 82.5s  移行部分の連打音
        // 83s 　 中間部分
        // 93s    二回目ループ
        // 101.8s ガラス音
        // 103.2s ラスサビ
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