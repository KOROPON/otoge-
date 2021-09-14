#nullable enable

using Rhythmium;
using UnityEngine;

namespace Reilas
{
    public sealed class TapNote : MonoBehaviour
    {
        private NoteEntity _entity = null!;

        public void Initialize(NoteEntity entity)
        {
            _entity = entity;
            transform.localScale = NotePositionCalculatorService.GetScale(_entity, 0.3f);
        }

        public void Render(float currentTime)
        {
            transform.position = NotePositionCalculatorService.GetPosition(_entity, currentTime, true);
        }
    }
}