using UnityEngine;
using UnityEngine.UI;

public class ClearRankDirector : MonoBehaviour
{
    private Animation _anim;
    private Image _clearRank;
    private Image _clearRankBar;
    private Image _fader;
    private AudioSource _clearAud;
    

    private void Start()
    {
        _anim = gameObject.GetComponent<Animation>();
        _clearRank = gameObject.GetComponentsInChildren<Image>()[0];
        _clearRankBar = gameObject.GetComponentsInChildren<Image>()[1];
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        _clearAud = gameObject.GetComponent<AudioSource>();
        _fader = GameObject.Find("Fader").GetComponent<Image>();
        _fader.color = new Color32(0, 0, 0, 180);
        _fader.enabled = false;
    }

    public void SelectRank(string clearRank)
    {
        _fader.enabled = true;
        _clearRank.sprite = Resources.Load<Sprite>("ClearRank/" + clearRank);
        _clearRankBar.sprite = Resources.Load<Sprite>("ClearRank/" + clearRank + "Bar");
        //_clearAud.clip = Resources.Load<AudioSource>("ClearAudio/" + clearRank);
        _anim.Play();
    }

    private void ShutterOpen()
    {
        Shutter.blChange = "ToR";
        Shutter.blShutterChange = "Close";
    }

    private void ClearSE()
    {
        _clearAud.Play();
    }
}
