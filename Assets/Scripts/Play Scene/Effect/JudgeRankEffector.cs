using UnityEngine;

public class JudgeRankEffector : MonoBehaviour
{
    public GameObject perfect;
    public GameObject good;
    public GameObject bad;
    public GameObject miss;

    private RhythmGamePresenter presenter = null!;

    private Vector3 finScale = new Vector3(1.5f, 1.5f, 0);
    private Vector3 resetScale = new Vector3(0f, 0f, 0);

    private int trueNum = -1;
    private float startTime;

    private void Start()
    {
        presenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
    }

    private void Update() // MusicNumManage JumpToSong éQè∆
    {
        if(startTime + 0.8f <= presenter.judgeTime)
        {
            switch (trueNum)
            {
                case 0:
                    {
                        perfect.transform.localScale = Vector3.Lerp(perfect.transform.localScale, resetScale, 0.5f);
                        break;
                    }
                case 1:
                    {
                        good.transform.localScale = Vector3.Lerp(good.transform.localScale, resetScale, 0.5f);
                        break;
                    }
                case 2:
                    {
                        bad.transform.localScale = Vector3.Lerp(bad.transform.localScale, resetScale, 0.5f);
                        break;
                    }
                case 3:
                    {
                        miss.transform.localScale = Vector3.Lerp(miss.transform.localScale, resetScale, 0.5f);
                        break;
                    }
                default: break;
            }
            return;
        }

        switch (trueNum)
        {
            case 0:
                {
                    perfect.transform.localScale = Vector3.Lerp(perfect.transform.localScale, finScale, 0.4f);
                    break;
                }
            case 1:
                {
                    good.transform.localScale = Vector3.Lerp(good.transform.localScale, finScale, 0.4f);
                    break;
                }
            case 2:
                {
                    bad.transform.localScale = Vector3.Lerp(bad.transform.localScale, finScale, 0.4f);
                    break;
                }
            case 3:
                {
                    miss.transform.localScale = Vector3.Lerp(miss.transform.localScale, finScale, 0.4f);
                    break;
                }
            default: break;
        }
    }

    public void JudgeRankDisplay(string rank)
    {
        Debug.Log(rank);
        switch (trueNum)
        {
            case 0:
                {
                    perfect.gameObject.SetActive(false);
                    break;
                }
            case 1:
                {
                    good.gameObject.SetActive(false);
                    break;
                }
            case 2:
                {
                    bad.gameObject.SetActive(false);
                    break;
                }
            case 3:
                {
                    miss.gameObject.SetActive(false);
                    break;
                }
            default: break;
        }

        switch (rank)
        {
            case "perfect" :
                {
                    trueNum = 0;
                    perfect.gameObject.SetActive(true);
                    perfect.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0);
                    startTime = presenter.judgeTime;
                    break;
                }
            case "good" :
                {
                    trueNum = 1;
                    good.gameObject.SetActive(true);
                    good.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0);
                    startTime = presenter.judgeTime;
                    break;
                }
            case "bad":
                {
                    trueNum = 2;
                    bad.gameObject.SetActive(true);
                    bad.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0);
                    startTime = presenter.judgeTime;
                    break;
                }
            case "miss" :
                {
                    trueNum = 3;
                    miss.gameObject.SetActive(true);
                    miss.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0);
                    startTime = presenter.judgeTime;
                    break;
                }
            default : break;
        }

    }
}
