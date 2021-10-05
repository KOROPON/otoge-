#nullable enable

using System.Collections.Generic;
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

        public void Render(float currentTime, int noteNum, List<ReilasNoteLineEntity> noteList)
        {
            if (_entity.Tail.JudgeTime < currentTime)
            {
                foreach (Transform child in this.transform)
                {
                    Destroy(child.gameObject);
                }
                noteList.RemoveAt(noteNum);
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
