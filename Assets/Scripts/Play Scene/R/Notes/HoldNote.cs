#nullable enable

using System.Collections.Generic;
using Rhythmium;
using UnityEngine;

namespace Reilas
{
    public sealed class HoldNote : MonoBehaviour
    {
        private ReilasNoteLineEntity _entity = null!;
        private float _noteSpeed;
        private Transform? _note;
        private Transform? _hold;

        public HoldNote(Transform? note)
        {
            _note = note;
        }

        public float time;
        public void Initialize(ReilasNoteLineEntity entity)
        {
            time = entity.Head.JudgeTime;
            _noteSpeed = entity.Head.Speed;
            _entity = entity;
            _hold = transform.GetChild(0);
        }

        public void Render(float currentTime, int noteNum, List<ReilasNoteLineEntity> noteList, List<SpeedChangeEntity> speedChangeEntities)
        {
            if (_entity.Tail.JudgeTime < currentTime)
            {
                foreach (Transform child in transform) foreach (Transform inChild in child) Destroy(inChild.gameObject);
                foreach (Transform child in transform) Destroy(child.gameObject);
                noteList.RemoveAt(noteNum);
                Destroy(gameObject);
                RhythmGamePresenter.HoldNotes.RemoveAt(noteNum);
                RhythmGamePresenter.HoldEffectors.RemoveAt(noteNum);
            }
            if (!gameObject.activeSelf) gameObject.SetActive(true);

            var scale = NotePositionCalculatorService.GetScale(_entity.Head);
            
            var headPos = NotePositionCalculatorService.GetPosition(_entity.Head, currentTime, _noteSpeed, false, speedChangeEntities);
            var tailPos = NotePositionCalculatorService.GetPosition(_entity.Tail, currentTime, _noteSpeed, false, speedChangeEntities);
            
            scale.z = tailPos.z - headPos.z;

            if (_hold == null) return;
            // NotePositionCalculatorService.GetScale(_entity.Head);
            _hold.localScale = scale;
            _hold.position = (headPos + tailPos) / 2f;
        }

        public void NoteDestroy(bool kujo)
        {
            if (kujo) RhythmGamePresenter.HoldKujoNotes.Remove(this);
            else RhythmGamePresenter.HoldNotes.Remove(this);
            foreach (Transform child in this.transform)
            {
                foreach (Transform inChild in child)
                {
                    Destroy(inChild.gameObject);
                }
            }
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
            Destroy(gameObject);
            RhythmGamePresenter.HoldEffectors.Remove(this.transform.GetChild(1).GetComponent<HoldEffector>());
        }
    }
}
