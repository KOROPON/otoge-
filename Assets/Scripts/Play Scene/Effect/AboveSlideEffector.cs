using UnityEngine;
using Reilas;

public sealed class AboveSlideEffector : MonoBehaviour
{
    private ReilasNoteLineEntity _entity = null!;
    private ParticleSystem _effect1 = null!;
    private ParticleSystem _effect2 = null!;
    private SpriteRenderer _noteBlight = null!;
    private int _headPos;
    private int _tailPos;
    private float _headTime;
    private float _tailTime;
    private bool _blDone;
    private bool _blJudge;
    public void EffectorInitialize(ReilasNoteLineEntity entity)
    {
        _entity = entity;
        _headPos = _entity.Head.LanePosition + 4;
        _tailPos = _entity.Tail.LanePosition + 4;
        _headTime = _entity.Head.JudgeTime;
        _tailTime = _entity.Tail.JudgeTime;
        _blDone = true;
        _effect1 = gameObject.GetComponentsInChildren<ParticleSystem>()[0];
        _effect2 = gameObject.GetComponentsInChildren<ParticleSystem>()[1];
        _noteBlight = transform.root.gameObject.GetComponentInChildren<SpriteRenderer>();
        transform.localPosition = new Vector3(100, 0, 0);

    }

    public void Render (float currentTime, AudioSource effectAudio)
    {
        if (currentTime - _entity.Head.JudgeTime >= 0)
        {
            _blJudge = false;
            transform.position = PositionCal(currentTime);
            foreach (LaneTapState tapstate in InputService.AboveLaneTapStates)
            {
                if (tapstate.laneNumber == JudgeLaneCal(currentTime))
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
    private Vector3 PositionCal(float currentTime)
    {
        var x = 4.53 * Mathf.Cos(Mathf.PI * ((_tailPos - _headPos) * (currentTime - _headTime) / (_tailTime - _headTime) + _headPos) / 32);
        var y = 4.53 * Mathf.Sin(Mathf.PI * ((_tailPos - _headPos) * (currentTime - _headTime) / (_tailTime - _headTime) + _headPos) / 32);//�ǂ������� - �|���Ȃ���
        return new Vector3((float) x, (float) y, 0);
    }
    private int JudgeLaneCal(float currentTime)
    {
        return Mathf.RoundToInt((_tailPos - _headPos) * (currentTime - _headTime) / (_tailTime - _headTime) + 4); 
    }
}
