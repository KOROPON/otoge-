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
    private List<ReilasNoteLineEntity> reilasKujoAboveSlide = new List<ReilasNoteLineEntity>();
    private List<ReilasNoteLineEntity> reilasKujoAboveHold = new List<ReilasNoteLineEntity>();
    private List<ReilasNoteLineEntity> reilasKujoHold = new List<ReilasNoteLineEntity>();
    private List<ReilasNoteEntity> reilasKujoChain = new List<ReilasNoteEntity>();

    private void Start()
    {
        presenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
    }

    public async void BossAwake()
    {
        TextAsset? kujyoSongs = await Resources.LoadAsync<TextAsset>("Charts/Reilas_half.KUJO") as TextAsset;
        if (kujyoSongs == null)
        {
            Debug.LogError("Reilas_KUJO ïàñ ÉfÅ[É^Ç™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÇ≈ÇµÇΩ");
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

        foreach(ReilasNoteEntity note in tapKujoNotes)
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
}