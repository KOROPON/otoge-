using UnityEngine;
using Reilas;

public sealed class AboveHoldEffector : MonoBehaviour
{
    private ReilasNoteLineEntity _entity = null!;
    private ParticleSystem _effect1 = null!;
    private ParticleSystem _effect2 = null!;
    private ParticleSystem _effect3 = null!;
    private MeshRenderer _noteBlight;
    private int _lanePos;
    private int _laneMax;
    private int _laneMin;
    private bool _blJudge;

    public float aboveHoldEffectTime;
    public void EffectorInitialize(ReilasNoteLineEntity entity)
    {
        _entity = entity;
        _lanePos = Mathf.RoundToInt(_entity.Head.LanePosition + _entity.Head.Size / 2);
        _laneMin = _entity.Head.LanePosition;
        _laneMax = _entity.Head.LanePosition + _entity.Head.Size;
        _effect1 = ((Component) this).gameObject.GetComponentsInChildren<ParticleSystem>()[0];
        _effect2 = ((Component)this).gameObject.GetComponentsInChildren<ParticleSystem>()[1];
        _effect3 = ((Component)this).gameObject.GetComponentsInChildren<ParticleSystem>()[2];
        Transform effectorTransform;
        _noteBlight = (effectorTransform = transform).root.GetComponent<MeshRenderer>(); ;
        transform.position = RhythmGamePresenter.LanePositions[_lanePos];
        transform.eulerAngles = new Vector3(0, 0, 16 - _lanePos / 32 * 180);
        aboveHoldEffectTime = _entity.Head.JudgeTime;
    }

    public void Render(float currentTime, AudioSource effectAudio)
    {
        _blJudge = false;
        if (InputService.AboveLaneTapStates != null)
        {
            foreach (var tapstate in InputService.AboveLaneTapStates)
            {
                if (_laneMin <= tapstate.laneNumber && tapstate.laneNumber <= _laneMax)
                {
                    RhythmGamePresenter.isHolding = true;
                    if (!_effect1.isPlaying)
                    {
                        _effect1.Play();
                        _effect2.Play();
                        _effect3.Play();
                        _noteBlight.material.color = new Color32(255, 255, 255, 160);
                        effectAudio.Play();
                    }
                    _blJudge = true;
                    break;
                }
            }
        }
        if (!_blJudge)
        {
            if (_effect1.isPlaying)
            {
                _effect1.Stop();
                _effect2.Stop();
                _effect3.Stop();
                _noteBlight.material.color = new Color32(160, 160, 160, 160);
            }
        }
    }
}