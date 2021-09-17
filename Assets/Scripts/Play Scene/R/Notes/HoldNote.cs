#nullable enable

using Rhythmium;
using UnityEngine;

namespace Reilas
{
    public sealed class HoldNote : MonoBehaviour
    {
        //[SerializeField] private SpriteRenderer _spriteRenderer = null!;

        private ReilasNoteLineEntity _entity = null!;

        public void Initialize(ReilasNoteLineEntity entity)
        {
            _entity = entity;
        }

        public void Render(float currentTime)
        {
            var scale = NotePositionCalculatorService.GetScale(_entity.Head);


            var headPos = NotePositionCalculatorService.GetPosition(_entity.Head, currentTime, false);
            var tailPos = NotePositionCalculatorService.GetPosition(_entity.Tail, currentTime, false);


            scale.z = tailPos.z - headPos.z;
            ;
            transform.localScale = scale; // NotePositionCalculatorService.GetScale(_entity.Head);

            transform.position = (headPos + tailPos) / 2f;
        }
    }
}
