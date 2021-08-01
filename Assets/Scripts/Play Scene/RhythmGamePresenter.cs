#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Rhythmium;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Reilas
{
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

        private void Awake()
        {
            AwakeAsync().Forget();
        }

        private async UniTask AwakeAsync()
        {
            FindObjectOfType<Variable>().enabled = false;

            var chartTextAsset = await Resources.LoadAsync<TextAsset>("Charts/I") as TextAsset;

            if (chartTextAsset == null)
            {
                Debug.LogError("譜面データが見つかりませんでした");
                return;
            }

            var chartJsonData = JsonUtility.FromJson<ChartJsonData>(chartTextAsset.text);
            
            
            var chartEntity = new ReilasChartConverter().Convert(chartJsonData);
            
            // chartEntity to song
            
            
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

        private void Update()
        {
            var currentTime = _audioSource.time - _chartEntity.StartTime;

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
}