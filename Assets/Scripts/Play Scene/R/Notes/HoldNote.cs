#nullable enable

using UnityEngine;

namespace Reilas
{
    public sealed class HoldNote : MonoBehaviour
    {
        private ReilasNoteLineEntity _entity = null!;
        private Transform _note;

        public void Initialize(ReilasNoteLineEntity entity)
        {
            _entity = entity;
        }

        public void Render(float currentTime, int noteNum)
        {
            if (_entity.Tail.JudgeTime < currentTime)
            {
                Destroy(gameObject);
                RhythmGamePresenter._holdNotes.RemoveAt(noteNum);
            }
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            var scale = NotePositionCalculatorService.GetScale(_entity.Head);


            var headPos = NotePositionCalculatorService.GetPosition(_entity.Head, currentTime, false);
            var tailPos = NotePositionCalculatorService.GetPosition(_entity.Tail, currentTime, false);
           

            scale.z = tailPos.z - headPos.z;

            transform.localScale = scale; // NotePositionCalculatorService.GetScale(_entity.Head);

            transform.position = (headPos + tailPos) / 2f;
        }
        public void NoteDestroy()
        {
            //Debug.Log(this.gameObject);
            Destroy(this.gameObject);
            
        }
    }
}
