#nullable enable

using System.Collections.Generic;
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

        public void Render(float currentTime, List<SpeedChangeEntity> speedChangeEntities)
        {
            if (!gameObject.activeSelf)
            {
                if (_entity.JudgeTime - currentTime < 10f) gameObject.SetActive(true);
            }
            else transform.position = NotePositionCalculatorService.GetPosition(_entity, currentTime, _noteSpeed, speedChangeEntities);
    }

        public void NoteDestroy(bool kujo)
        {
            if (kujo) RhythmGamePresenter.TapKujoNotes.Remove(this);
            else RhythmGamePresenter.TapNotes.Remove(this);

            Destroy(gameObject);
        }
    }
}