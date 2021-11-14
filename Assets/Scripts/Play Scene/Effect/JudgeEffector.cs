using UnityEngine;

public class JudgeEffector : MonoBehaviour
{
    private GameObject[] _effectors;
    private int[] _effectorLanePos;

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
    }

    public void TapJudgeEffector(int lanePos)
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
                effector.GetComponentsInChildren<ParticleSystem>()[0].Stop();
                effector.GetComponentsInChildren<ParticleSystem>()[1].Stop();
                effector.GetComponentsInChildren<ParticleSystem>()[2].Stop();
            }
            _effectorLanePos.SetValue(lanePos, i);
            effector.transform.position = RhythmGamePresenter.LanePositions[lanePos];
            if (lanePos <= 3)//below
            {
                effector.GetComponentsInChildren<ParticleSystem>()[0].Play();
                effector.GetComponentsInChildren<ParticleSystem>()[1].Play();
                effector.GetComponentsInChildren<ParticleSystem>()[2].Play();
            }
            else//above
            {
                effector.transform.eulerAngles = new Vector3(0, 0, (16 - lanePos) / 32 * 180);
                effector.GetComponentsInChildren<ParticleSystem>()[0].Play();
                effector.GetComponentsInChildren<ParticleSystem>()[1].Play();
                effector.GetComponentsInChildren<ParticleSystem>()[2].Play();
            }
            break;
        }
    }
}
