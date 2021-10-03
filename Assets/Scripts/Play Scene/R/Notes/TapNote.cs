#nullable enable

using Rhythmium;
using UnityEngine;

namespace Reilas
{
    public sealed class TapNote : MonoBehaviour
    {
        private NoteEntity _entity = null!;
        public float _tapTime;

        public void Initialize(ReilasNoteEntity entity)
        {
            _tapTime = entity.JudgeTime;
            _entity = entity;
            transform.localScale = NotePositionCalculatorService.GetScale(_entity, 0.4f);
        }

        public void Render(float currentTime)
        {
            if (!this.gameObject.activeSelf)
            {

                if (_entity.JudgeTime - currentTime < 10f)
                {
                    this.gameObject.SetActive(true);
                }
            }
            else
            {
                transform.position = NotePositionCalculatorService.GetPosition(_entity, currentTime, true);
            }
        }

        public void NoteDestroy(int noteNum)
        {
            Debug.Log(this.gameObject);
            Destroy(this.gameObject);
            RhythmGamePresenter._tapNotes.RemoveAt(noteNum);
        }
    }
}