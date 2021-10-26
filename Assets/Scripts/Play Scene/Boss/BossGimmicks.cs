using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Reilas;
using Rhythmium;
using UnityEngine;

public class BossGimmicks : MonoBehaviour
{
    private RhythmGamePresenter presenter = null!;

    TextAsset? kujyoSongs;

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
        kujyoSongs = await Resources.LoadAsync<TextAsset>("Charts/Reilas_half.KUJO") as TextAsset;
        if (kujyoSongs == null)
        {
            Debug.LogError("Reilas_KUJO ïàñ ÉfÅ[É^Ç™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÇ≈ÇµÇΩ");
            return;
        }

        var chartKujoJsonData = JsonUtility.FromJson<ChartJsonData>(kujyoSongs.text);
        var chartKujoEntity = new ReilasChartConverter().Convert(chartKujoJsonData);

        NoteLineJsonData[] noteKujoJsonData = chartKujoJsonData.timeline.noteLines;

        tapKujoNotes = new List<ReilasNoteEntity>(GetNoteTypes(chartKujoEntity, "Tap"));
        internalKujoNotes = new List<ReilasNoteEntity>(GetNoteTypes(chartKujoEntity, "Internal"));
        chainKujoNotes = new List<ReilasNoteEntity>(GetNoteTypes(chartKujoEntity, "Chain"));

        reilasKujoAboveSlide = chartKujoEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveSlide).ToList();
        reilasKujoAboveHold = chartKujoEntity.NoteLines.Where(note => note.Head.Type == NoteType.AboveHold).ToList();
        reilasKujoHold = chartKujoEntity.NoteLines.Where(note => note.Head.Type == NoteType.Hold).ToList();
        reilasKujoChain = chartKujoEntity.Notes.Where(note => note.Type == NoteType.AboveChain).ToList();
    }
}