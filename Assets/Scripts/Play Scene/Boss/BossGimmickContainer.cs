using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;
public class BossGimmickContainer : MonoBehaviour
{
    private PostProcessVolume _cameraEffector;
    private Image _stagingFader;
    private Animation _stagingFaderAnim;
    private Animation _backGroundAnim;
    private Sprite _backClock;
    public Image _backGround;
    public Bloom _cameraShine;

    public bool effectStart = false;

    private void Start()
    {
        _cameraEffector = GameObject.Find("Camera Effectors").GetComponent<PostProcessVolume>();
        _cameraEffector.profile.TryGetSettings(out _cameraShine);
        _stagingFader = GameObject.Find("StagingFader").GetComponent<Image>();
        _stagingFader.enabled = false;
        _stagingFaderAnim = GameObject.Find("StagingFader").GetComponent<Animation>();
        _backGroundAnim = GameObject.Find("BackGround").GetComponent<Animation>();
        _backGround = GameObject.Find("BackGround").GetComponent<Image>();
        _backClock = Resources.Load<Sprite>("BackGround/Fader");
    }


    /// <summary>
    /// ノーツとか光らせるエフェクト
    /// </summary>
    public void ChangeObjectShine()
    {
        _cameraShine.intensity.value = 40f;
    }
    /// <summary>
    /// 最初に画面暗くしてって背景中身切り替え
    /// </summary>
    public void BlackOutFirst()
    {
        Debug.Log("FirstBlackOut");
        _stagingFader.enabled = true;
        _stagingFaderAnim.Play();
        _backGroundAnim.Play("BackGroundAnimFirst");
    }
    public void LastChorus()
    {
        _backGround.sprite = _backClock;
        _backGround.color = new Color32(255, 255, 255, 255);
        _stagingFader.enabled = false;
    }
    /// <summary>
    /// 時計の音に合わせた明暗
    /// </summary>
    public void BlackOut()
    {
        StartCoroutine("BlackOutIntermittently");
    }
    public IEnumerator BlackOutIntermittently()
    {
        Debug.Log("BlackOut");
        var i = 0;
        _stagingFaderAnim.Play("StagingFaderAnim");
        _cameraShine.intensity.value = 20f;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            i++;
            _cameraShine.intensity.value = (float) (_cameraShine.intensity.value - 0.16);
            if (i <= 60) continue;
            _cameraShine.intensity.value = 10f;
            break;
        }
    }
}
