#nullable enable

using Rhythmium;
using UnityEngine;

namespace Reilas
{
    public sealed class TapNote : MonoBehaviour
    {
        private NoteEntity _entity = null!;
        private float _noteSpeed;
        public float tapTime;

        public void Initialize(ReilasNoteEntity entity)
        {
            tapTime = entity.JudgeTime;
            _entity = entity;
            _noteSpeed = entity.Speed;
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
                transform.position = NotePositionCalculatorService.GetPosition(_entity, currentTime, true, _noteSpeed);
            }
        }

        public void NoteDestroy()
        {
            RhythmGamePresenter._tapNotes.Remove(this);
            Destroy(this.gameObject);
        }
    }
}