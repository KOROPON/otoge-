#nullable enable

using Rhythmium;
using UnityEngine;

namespace Reilas
{
    public sealed class TapNote : MonoBehaviour
    {
        private NoteEntity _entity = null!;
        public float _tapTime;

        public void Initialize(NoteEntity entity)
        {
            _tapTime = entity.JudgeTime;
            _entity = entity;
            transform.localScale = NotePositionCalculatorService.GetScale(_entity, 0.3f);
        }

        public void Render(float currentTime)
        {
            transform.position = NotePositionCalculatorService.GetPosition(_entity, currentTime, true);
        }

        public void NoteDestroy()
        {
            Destroy(this.gameObject);
        }
    }
}