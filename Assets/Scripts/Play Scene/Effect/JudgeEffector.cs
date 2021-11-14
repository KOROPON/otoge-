using UnityEngine;

public class JudgeEffector : MonoBehaviour
{
    private GameObject[] _effectors;
    private int[] _effectorLanePos;
    private AudioSource _perfectOn;
    private AudioSource _goodOn;
    private AudioSource _badOn;

    void Start()
    {
        _effectors = new GameObject[10];
        _effectorLanePos = new int[10];
        var i = 0;
        foreach (Transform child in transform)
        {
            _effectors[i] = child.gameObject;
            _effectorLanePos[i] = -1;
            i++;
        }
        _perfectOn = GameObject.Find("Perfect").GetComponent<AudioSource>();
        _goodOn = GameObject.Find("Good").GetComponent<AudioSource>();
        _badOn = GameObject.Find("Bad").GetComponent<AudioSource>();
    }

    public void TapJudgeEffector(int lanePos, string judgeLevel)
    {
        for (var i =0; i <= 9; i++)
        {
            var effector = _effectors[i];
            if (effector.GetComponentsInChildren<ParticleSystem>()[0].isPlaying && _effectorLanePos[i] != lanePos)
            {
                continue;
            }
            //lanepos�ォ��������Effector�̌������킹�ċN��
            if (_effectorLanePos[i] == lanePos)
            {
                EffectAudiou(judgeLevel);
                effector.GetComponentsInChildren<ParticleSystem>()[0].Stop();
                effector.GetComponentsInChildren<ParticleSystem>()[1].Stop();
                effector.GetComponentsInChildren<ParticleSystem>()[2].Stop();
            }
            _effectorLanePos.SetValue(lanePos, i);
            effector.transform.position = RhythmGamePresenter.LanePositions[lanePos];
            if (lanePos <= 3)//below
            {
                EffectAudiou(judgeLevel);
                effector.transform.eulerAngles = new Vector3(0, 0, 0);
                effector.GetComponentsInChildren<ParticleSystem>()[0].Play();
                effector.GetComponentsInChildren<ParticleSystem>()[1].Play();
                effector.GetComponentsInChildren<ParticleSystem>()[2].Play();
            }
            else//above
            {
                EffectAudiou(judgeLevel);
                //Debug.Log("lanePos = " + lanePos + "  " + (90 - (180 * (lanePos + 0.5f)) / 32) + "度");
                effector.transform.eulerAngles = new Vector3(0, 0, 90 - (180 * (lanePos - 4 + 0.5f)) / 32);
                effector.GetComponentsInChildren<ParticleSystem>()[0].Play();
                effector.GetComponentsInChildren<ParticleSystem>()[1].Play();
                effector.GetComponentsInChildren<ParticleSystem>()[2].Play();
            }
            break;
        }
    }
    private void EffectAudiou(string judgeLevel)
    {
        switch (judgeLevel)
        {
            case "Perfect": _perfectOn.Play(); break;
            case "Good": _goodOn.Play(); break;
            case "Bad": _badOn.Play(); break;
        }
    }
}
