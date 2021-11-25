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
        public float ratio;

        private bool _boss;
        private Vector2 judgeSize;

        public void Initialize(ReilasNoteEntity entity, bool boss)
        {
            _boss = boss;
            tapTime = entity.JudgeTime;
            _entity = entity;
            _noteSpeed = entity.Speed;
            judgeSize = NotePositionCalculatorService.GetScale(_entity, 0.4f);
        }

        public void Render(float currentTime, List<SpeedChangeEntity> speedChangeEntities)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                return;
            }
            //transform.position = NotePositionCalculatorService.GetPosition(_entity, currentTime, _noteSpeed, speedChangeEntities);
            ratio = NotePositionCalculatorService.NoteRatio(_entity, currentTime, _noteSpeed);
            if (ratio < 0) return;
            var sizeRatio = Mathf.Lerp(0.3f, 1f, ratio);
            transform.localScale = new Vector2(judgeSize.x * sizeRatio, judgeSize.y * sizeRatio);
            transform.position = new Vector2((_entity.LanePosition - 1.5f) * 0.75f + (_entity.LanePosition - 1.5f) * 1.5f * ratio, -3 * ratio);
        }

        public void NoteDestroy(bool kujo)
        {
            if (kujo) RhythmGamePresenter.TapKujoNotes.Remove(this);
            else RhythmGamePresenter.TapNotes.Remove(this);

            Destroy(gameObject);
        }
    }
}
