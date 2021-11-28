using System.Linq;
using UnityEngine;
using Reilas;

public sealed class HoldEffector : MonoBehaviour
{
    [SerializeField] private bool blDone;
    
    private ReilasNoteLineEntity _entity = null!;
    private ParticleSystem _effect1 = null!;
    private ParticleSystem _effect2 = null!;
    private ParticleSystem _effect3 = null!;
    private MeshRenderer _noteBlight = null!;
    private int _lanePos;
    private bool _blJudge;
    
    public float holdEffectTime;

    private Material material;

    public void EffectorInitialize(ReilasNoteLineEntity entity)
    {
        _entity = entity;
        _lanePos = _entity.Head.LanePosition;
        blDone = true;
        _effect1 = gameObject.GetComponentsInChildren<ParticleSystem>()[0];
        _effect2 = gameObject.GetComponentsInChildren<ParticleSystem>()[1];
        _effect3 = ((Component)this).gameObject.GetComponentsInChildren<ParticleSystem>()[2];
        Transform effectorTransform;
        _noteBlight = (effectorTransform = transform).root.GetComponent<MeshRenderer>();
        effectorTransform.position = RhythmGamePresenter.LanePositions[_lanePos];
        holdEffectTime = _entity.Head.JudgeTime;
    }
    public void Render(float currentTime, AudioSource effectAudio)
    {
        _blJudge = false;
        if (InputService.AboveLaneTapStates.Any(tapState => tapState.laneNumber == _lanePos))
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
        }

        if (_blJudge || !_effect1.isPlaying) return;
        _effect1.Stop();
        _effect2.Stop();
        _effect3.Stop();
        _noteBlight.material.color = new Color32(230, 230, 230, 160);
    }
}