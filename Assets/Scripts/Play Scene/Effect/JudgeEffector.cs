using UnityEngine;

public class JudgeEffector : MonoBehaviour
{
    private GameObject[] _effectors;
    private AudioSource _sePerfect;
    private AudioSource _seGood;
    private AudioSource _seBad;

    void Start()
    {
        _effectors = new GameObject[transform.childCount];
        var i = 0;
        foreach (Transform child in transform)
        {
            _effectors[i] = child.gameObject;
            i++;
        }

    }

    public void TapJudgeEffector(int lanePos)
    {

    }
}