using UnityEngine;
using Reilas;

public sealed class AboveHoldEffector : MonoBehaviour
{
    private ReilasNoteLineEntity _entity = null!;
    private ParticleSystem _effect1 = null!;
    private ParticleSystem _effect2 = null!;
    private Color _noteBlight;
    [SerializeField]private GameObject _gameObject;
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
        _effect1 = gameObject.GetComponentsInChildren<ParticleSystem>()[0];
        _effect2 = gameObject.GetComponentsInChildren<ParticleSystem>()[1];
        _noteBlight = transform.root.GetComponent<MeshRenderer>().material.color;
        transform.position = RhythmGamePresenter.LanePositions[_lanePos];
        aboveHoldEffectTime = _entity.Head.JudgeTime;
    }

    public void Render(float currentTime, AudioSource effectAudio)
    {
        if (!_gameObject.activeSelf)
        {
            _gameObject.SetActive(true);
        }
        _blJudge = false;
        if (InputService.AboveLaneTapStates != null)
        {
            foreach (var tapstate in InputService.AboveLaneTapStates)
            {
                if (_laneMin <= tapstate.laneNumber && tapstate.laneNumber <= _laneMax)
                {
                    if (!_effect1.isPlaying)
                    {
                        _effect1.Play();
                        _effect2.Play();
                        _noteBlight = new Color32(255, 255, 255, 160);
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
                _noteBlight = new Color32(130, 130, 130, 160);
                effectAudio.Pause();
            }
        }
    }
}