using UnityEngine;
using Reilas;

public sealed class AboveHoldEffector : MonoBehaviour
{
    private ReilasNoteLineEntity _entity = null!;
    private ParticleSystem _effect1 = null!;
    private ParticleSystem _effect2 = null!;
    private SpriteRenderer _noteBlight = null!;
    private int _lanePos;
    private bool _blDone;
    private bool _blJudge;
    public void EffectorInitialize(ReilasNoteLineEntity entity)
    {
        _entity = entity;
        _lanePos = _entity.Head.LanePosition;
        _blDone = true;
        _effect1 = gameObject.GetComponentsInChildren<ParticleSystem>()[0];
        _effect2 = gameObject.GetComponentsInChildren<ParticleSystem>()[1];
        _noteBlight = transform.root.gameObject.GetComponentInChildren<SpriteRenderer>();
        transform.localPosition = new Vector3(100, 0, 0);

    }

    public void Render(float currentTime, AudioSource effectAudio)
    {
        if (currentTime - _entity.Head.JudgeTime >= 0)
        {
            _blJudge = false;
            transform.position = RhythmGamePresenter.LanePositions[_lanePos];
            foreach (LaneTapState tapstate in InputService.AboveLaneTapStates)
            {
                if (tapstate.laneNumber == _lanePos)
                {
                    if (!_effect1.isPlaying)
                    {
                        _effect1.Play();
                        _effect2.Play();
                        _noteBlight.color = new Color32(255, 255, 255, 255);
                        effectAudio.Play();
                    }
                    _blJudge = true;
                    break;
                }
            }
            if (!_blJudge)
            {
                if (_effect1.isPlaying)
                {
                    _effect1.Stop();
                    _effect2.Stop();
                    _noteBlight.color = new Color32(111, 111, 111, 255);
                    effectAudio.Pause();
                }
            }
        }

    }
}