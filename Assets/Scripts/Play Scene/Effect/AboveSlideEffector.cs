using UnityEngine;
using Reilas;

public class AboveSlideEffector : MonoBehaviour
{ 
    private ReilasNoteLineEntity _entity = null!;
    private ParticleSystem _effect1 = null!;
    private ParticleSystem _effect2 = null!;
    private ParticleSystem _effect3 = null!;
    private MeshRenderer _noteBlight;
    private int _headPos;
    private int _headMax;
    private int _headMin;
    private int _tailPos;
    private int _tailMax;
    private int _tailMin;
    private float _headTime;
    private float _tailTime;
    public bool blJudge = false;

    public float aboveSlideEffectTime;

    public void EffectorInitialize(ReilasNoteLineEntity entity)
    {
        _entity = entity;
        _headPos = _entity.Head.LanePosition + entity.Head.Size / 2;
        _tailPos = _entity.Tail.LanePosition + entity.Tail.Size / 2;
        _headMin = _entity.Head.LanePosition;
        _headMax = _entity.Head.LanePosition + _entity.Head.Size;
        _tailMin = _entity.Tail.LanePosition;
        _tailMax = _entity.Tail.LanePosition + _entity.Tail.Size;
        _headTime = _entity.Head.JudgeTime;
        _tailTime = _entity.Tail.JudgeTime;
        _effect1 = ((Component) this).gameObject.GetComponentsInChildren<ParticleSystem>()[0];
        _effect2 = ((Component) this).gameObject.GetComponentsInChildren<ParticleSystem>()[1];
        _effect3 = ((Component)this).gameObject.GetComponentsInChildren<ParticleSystem>()[2];
        Transform effectorTransform;
        _noteBlight = (effectorTransform = transform).root.GetComponent<MeshRenderer>();
        aboveSlideEffectTime = _entity.Head.JudgeTime;
        //Debug.Log("colorHere" + _noteBlight.r);
    }

    public void Render(float currentTime, AudioSource effectAudio)
    {
        //var _laneMax = Mathf.RoundToInt((_tailMax - _headMax) * (currentTime - _headTime) / (_tailTime - _headTime) + _headMax) + 4;
        //var _laneMin = Mathf.RoundToInt((_tailMin - _headMin) * (currentTime - _headTime) / (_tailTime - _headTime) + _headMin) + 4;
        var laneMax = Mathf.RoundToInt(Mathf.Lerp(_headMax, _tailMax, (currentTime - _headTime))) + 4;
        var laneMin = Mathf.RoundToInt(Mathf.Lerp(_headMin, _tailMin, (currentTime - _headTime))) + 4;
        Debug.Log("max" + laneMax);
        Debug.Log("min" + laneMin);

        transform.position = PositionCal(currentTime);
        transform.eulerAngles = AngleCal(currentTime);

        if (InputService.AboveLaneTapStates != null)
        {
            foreach (LaneTapState tapstate in InputService.AboveLaneTapStates)
            {
                Debug.Log("SlideEffectWorkaaaaaaaaaaaaaa");
                if (laneMin <= tapstate.laneNumber && tapstate.laneNumber <= laneMax)
                {
                    Debug.Log("SlideEffectWorkbbbbbbbbbbbbbbb");
                    if (!_effect1.isPlaying)
                    {
                        _effect1.Play();
                        _effect2.Play();
                        _effect3.Play();
                        _noteBlight.material.color = new Color32(255, 255, 255, 160);
                        effectAudio.Play();
                    }
                    blJudge = true;
                    return;
                }
            }
            blJudge = false;
        }
        else
        {
            blJudge = false;
        }

        if (!blJudge)
        {
            if (_effect1.isPlaying)
            {
                _effect1.Stop();
                _effect2.Stop();
                _effect3.Stop();
                _noteBlight.material.color = new Color32(130, 130, 130, 160);
                effectAudio.Pause();
            }
        }
    }
    private Vector3 PositionCal(float currentTime)
    {
        float pai = Mathf.PI * (32 - ((_tailPos - _headPos) * (currentTime - _headTime) / (_tailTime - _headTime) + _headPos)) / 32;
        float x = (float)(4.4 * Mathf.Cos(pai));
        float y = (float)4.4 * Mathf.Sin(pai);
        return new Vector3(x, y, -0.9f);
    }

    private Vector3 AngleCal(float currentTime)
    {
        float rot = (16 - Mathf.Lerp(_headPos, _tailPos, (currentTime - _headTime) / (_tailTime - _headTime))) / 32 * 180;
        return new Vector3(0, 0, rot);
    }
}
