#nullable enable
using Rhythmium;
using UnityEngine;

namespace Reilas
{
    public sealed class TapNote : MonoBehaviour
    {
        private NoteEntity _entity = null!;

        private bool _kujo;
        
        private float _noteSpeed;
        private float _thisNoteX;
        private float _position;

        private int _speedChangeIndex;
        
        public float tapTime;

        public void Initialize(ReilasNoteEntity entity, bool kujo)
        {
            tapTime = entity.JudgeTime;
            _entity = entity;
            _noteSpeed = entity.Speed;
            _kujo = kujo;
            _thisNoteX = -3.3f + _entity.LanePosition * 2.2f;
            _position = NotePositionCalculatorService.LeftOverPositionCalculator(tapTime, _noteSpeed);
            _speedChangeIndex = 0;
            Transform transform1;
            (transform1 = transform).localScale = NotePositionCalculatorService.GetScale(_entity, 0.4f);
            transform1.position = new Vector3(_thisNoteX, 0f, -50);
        }

        public void Render(float currentTime)
        {
            var difference = tapTime - currentTime;
            
            if (!gameObject.activeSelf && difference < 10f) gameObject.SetActive(true);
            else transform.position = new Vector3(_thisNoteX, 0f, NotePositionCalculatorService.GetPosition(tapTime, currentTime, _noteSpeed, _position, _speedChangeIndex));
            
            if (!RhythmGamePresenter.checkSpeedChangeEntity ||
                currentTime < RhythmGamePresenter.CalculatePassedTime(_speedChangeIndex)) return;
            _position -= NotePositionCalculatorService.SpanCalculator(_speedChangeIndex, tapTime, _noteSpeed);
            _speedChangeIndex++;
        }

        public void NoteDestroy(bool kujo)
        {
            if (kujo) RhythmGamePresenter.TapKujoNotes.Remove(this);
            else RhythmGamePresenter.TapNotes.Remove(this);

            Destroy(gameObject);
        }
    }
}
