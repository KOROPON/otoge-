using System.Linq;
using UnityEngine;
using Reilas;

public sealed class HoldEffector : MonoBehaviour
{
    [SerializeField] private bool blDone;
    
    private ReilasNoteLineEntity _entity = null!;
    private ParticleSystem _effect1 = null!;
    private ParticleSystem _effect2 = null!;
    private SpriteRenderer _noteBlight = null!;
    private int _lanePos;
    private bool _blJudge;

    public float holdEffectTime;
    

    public void EffectorInitialize(ReilasNoteLineEntity entity)
    {
        _entity = entity;
        _lanePos = _entity.Head.LanePosition;
        blDone = true;
        _effect1 = gameObject.GetComponentsInChildren<ParticleSystem>()[0];
        _effect2 = gameObject.GetComponentsInChildren<ParticleSystem>()[1];
        Transform effectorTransform;
        _noteBlight = (effectorTransform = transform).root.GetChild(0).GetComponentInChildren<SpriteRenderer>();
        effectorTransform.position = RhythmGamePresenter.LanePositions[_lanePos];
        holdEffectTime = _entity.Head.JudgeTime;
    }
    public void Render(float currentTime, AudioSource effectAudio)
    {
        _blJudge = false;
        if (InputService.AboveLaneTapStates.Any(tapState => tapState.laneNumber == _lanePos))
        {
            if (!_effect1.isPlaying)
            {
                _effect1.Play();
                _effect2.Play();
                _noteBlight.color = new Color32(255, 255, 255, 255);
                //effectAudio.Play();
            }
            _blJudge = true;
        }

        if (_blJudge || !_effect1.isPlaying) return;
        _effect1.Stop();
        _effect2.Stop();
        _noteBlight.color = new Color32(111, 111, 111, 255);
        //effectAudio.Pause();
    }
}