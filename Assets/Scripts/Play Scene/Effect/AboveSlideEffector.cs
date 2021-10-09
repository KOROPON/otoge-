using UnityEngine;
using Reilas;

public sealed class AboveSlideEffector : MonoBehaviour
{ 
    [SerializeField]private GameObject _gameObject;
    private ReilasNoteLineEntity _entity = null!;
    private ParticleSystem _effect1 = null!;
    private ParticleSystem _effect2 = null!;
    private Material _noteBlight = null!;
    private int _headPos;
    private int _headMax;
    private int _headMin;
    private int _tailPos;
    private int _tailMax;
    private int _tailMin;
    private float _headTime;
    private float _tailTime;
    private bool _blJudge;

    public float aboveSlideEffectTime;

    public void EffectorInitialize(ReilasNoteLineEntity entity)
    {
        _entity = entity;
        _headPos = Mathf.RoundToInt(4 + _entity.Head.LanePosition + _entity.Head.Size);
        _tailPos = Mathf.RoundToInt(4 + _entity.Tail.LanePosition + _entity.Head.Size / 2);
        _headMax = 4 + _entity.Head.LanePosition;
        _headMin = 4 + _entity.Head.LanePosition + _entity.Head.Size;
        _tailMax = 4 + _entity.Tail.LanePosition;
        _tailMin = 4 + _entity.Tail.LanePosition + _entity.Tail.Size;
        _headTime = _entity.Head.JudgeTime;
        _tailTime = _entity.Tail.JudgeTime;
        _effect1 = gameObject.GetComponentsInChildren<ParticleSystem>()[0];
        _effect2 = gameObject.GetComponentsInChildren<ParticleSystem>()[1];
        _noteBlight = transform.GetComponent<Material>();
        aboveSlideEffectTime = _entity.Head.JudgeTime;
    }

    public void Render (float currentTime, AudioSource effectAudio)
    {
        var _laneMax = Mathf.RoundToInt((_tailMax - _headMax) * (currentTime - _headTime) / (_tailTime - _headTime) + 4);
        var _laneMin = Mathf.RoundToInt((_tailMin - _headMin) * (currentTime - _headTime) / (_tailTime - _headTime) + 4);
        if (!_gameObject.activeSelf)
        {
            _gameObject.SetActive(true);
        }
        _blJudge = false;
        transform.position = PositionCal(currentTime);
        if (InputService.AboveLaneTapStates != null)
        {
            foreach (LaneTapState tapstate in InputService.AboveLaneTapStates)
            {
                if (_laneMin <= tapstate.laneNumber && tapstate.laneNumber <= _laneMin)
                {
                    if (!_effect1.isPlaying)
                    {
                        _effect1.Play();
                        _effect2.Play();
                        _noteBlight.color = new Color32(255, 255, 255, 160);
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
                _noteBlight.color = new Color32(130, 130, 130, 160);
                effectAudio.Pause();
            }
        }
    }
    private Vector3 PositionCal(float currentTime)
    {
        float pai = Mathf.PI * ((_tailPos - _headPos) * (currentTime - _headTime) / (_tailTime - _headTime) + _headPos) / 32;
        var x = 4.53 * Mathf.Cos(pai);
        var y = 4.53 * Mathf.Sin(pai);//�ǂ������� - �|���Ȃ���
        return new Vector3((float) x, (float) y,(float) -0.9);
    }
   
}
