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
        private float _thisNoteX;

        public float tapTime;
        public bool _kujo;

        public void Initialize(ReilasNoteEntity entity, bool kujo)
        {
            _kujo = kujo;
            tapTime = entity.JudgeTime;
            _entity = entity;
            _noteSpeed = entity.Speed;
            _thisNoteX = -3.3f + _entity.LanePosition * 2.2f;
            Transform transform1;
            (transform1 = transform).localScale = NotePositionCalculatorService.GetScale(_entity, 0.4f);
            transform1.position = new Vector3(_thisNoteX, 0f, -50);
        }

        public void Render(float currentTime)
        {
            if (!gameObject.activeSelf)
            {
                if (_entity.JudgeTime - currentTime < 10f) gameObject.SetActive(true);
            }
            else transform.position = new Vector3(_thisNoteX, 0f, NotePositionCalculatorService.GetPosition(_entity.JudgeTime, currentTime, _noteSpeed));
        }

        public void NoteDestroy(bool kujo)
        {
            if (kujo) RhythmGamePresenter.TapKujoNotes.Remove(this);
            else RhythmGamePresenter.TapNotes.Remove(this);

            Destroy(gameObject.transform.GetChild(0).gameObject);
            Destroy(gameObject);
        }
    }
}
