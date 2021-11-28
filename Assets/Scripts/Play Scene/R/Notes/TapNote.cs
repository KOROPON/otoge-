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

        private float thisNoteX;

        public float tapTime;

        public void Initialize(ReilasNoteEntity entity)
        {
            tapTime = entity.JudgeTime;
            _entity = entity;
            _noteSpeed = entity.Speed;
            thisNoteX = -3.3f + _entity.LanePosition * 2.2f;
            transform.localScale = NotePositionCalculatorService.GetScale(_entity, 0.4f);
            transform.position = new Vector3(thisNoteX, 0f, -50);
        }

        public void Render(float currentTime, List<SpeedChangeEntity> speedChangeEntities)
        {
            if (!gameObject.activeSelf)
            {
                if (_entity.JudgeTime - currentTime < 10f) gameObject.SetActive(true);
            }
            else transform.position = new Vector3(thisNoteX, 0f, NotePositionCalculatorService.GetPosition(_entity.JudgeTime, currentTime, _noteSpeed, speedChangeEntities));
        }

        public void NoteDestroy(bool kujo)
        {
            if (kujo) RhythmGamePresenter.TapKujoNotes.Remove(this);
            else RhythmGamePresenter.TapNotes.Remove(this);

            Destroy(this.gameObject.transform.GetChild(0).gameObject);
            Destroy(this.gameObject);
        }
    }
}
