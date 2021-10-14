using UnityEngine;
using Reilas;

public sealed class AboveSlideEffector : MonoBehaviour
{ 
    [SerializeField]private GameObject _gameObject;
    private ReilasNoteLineEntity _entity = null!;
    private ParticleSystem _effect1 = null!;
    private ParticleSystem _effect2 = null!;
    private Color _noteBlight;
    private float _headPos;
    private float _headMax;
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
        _headPos = 4 + _entity.Head.LanePosition + _entity.Head.Size / 2;
        _tailPos = 4 + _entity.Tail.LanePosition + _entity.Head.Size / 2;
        _headMin = _entity.Head.LanePosition;
        _headMax = _entity.Head.LanePosition + _entity.Head.Size;
        _tailMin = _entity.Tail.LanePosition;
        _tailMax = _entity.Tail.LanePosition + _entity.Tail.Size;
        _headTime = _entity.Head.JudgeTime;
        _tailTime = _entity.Tail.JudgeTime;
        _effect1 = gameObject.GetComponentsInChildren<ParticleSystem>()[0];
        _effect2 = gameObject.GetComponentsInChildren<ParticleSystem>()[1];
        _noteBlight = transform.root.GetComponent<MeshRenderer>().material.color;
        aboveSlideEffectTime = _entity.Head.JudgeTime;
        Debug.Log("colorHere" + _noteBlight.r);
    }

    public void Render (float currentTime, AudioSource effectAudio)
    {
        var _laneMax = Mathf.RoundToInt((_tailMax - _headMax) * (currentTime - _headTime) / (_tailTime - _headTime) + _headMax);
        var _laneMin = Mathf.RoundToInt((_tailMin - _headMin) * (currentTime - _headTime) / (_tailTime - _headTime) + _headMin);
        Debug.Log("max"+_laneMax);
        Debug.Log("min"+_laneMin);
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
                //Debug.Log("SlideEffectWorkaaaaaaaaaaaaaa");
                if (_laneMin <= tapstate.laneNumber && tapstate.laneNumber <= _laneMax)
                {
                    //Debug.Log("SlideEffectWorkbbbbbbbbbbbbbbb");
                    if (!_effect1.isPlaying)
                    {
                        //Debug.Log("SlideEffectWorkcccccccccccccc");
                        _effect1.Play();
                        _effect2.Play();
                        _noteBlight = new Color32(255, 255, 255, 160);
                        effectAudio.Play();
                        //Debug.Log("SlideEffectWork");
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
                Debug.Log("SlideEffectStop");
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
