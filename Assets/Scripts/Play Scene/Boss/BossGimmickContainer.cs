using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;
public class BossGimmickContainer : MonoBehaviour
{
    private PostProcessVolume _cameraEffector;
    private Bloom _cameraShine;
    private Image _stagingFader;
    private Animation _stagingFaderAnim;
    private Animation _backGroundAnim;

    public bool effectStart = true;

    private void Start()
    {
        _cameraEffector = GameObject.Find("Camera Effectors").GetComponent<PostProcessVolume>();
        _cameraEffector.profile.TryGetSettings(out _cameraShine);
        _stagingFader = GameObject.Find("StagingFader").GetComponent<Image>();
        _stagingFader.enabled = false;
        _stagingFaderAnim = GameObject.Find("StagingFader").GetComponent<Animation>();
        _backGroundAnim = GameObject.Find("BackGround").GetComponent<Animation>();
    }


    /// <summary>
    /// �m�[�c�Ƃ����点��G�t�F�N�g
    /// </summary>
    public void ChangeObjectShine()
    {
        _cameraShine.intensity.value = 40f;
    }
    /// <summary>
    /// �ŏ��ɉ�ʈÂ����Ă��Ĕw�i���g�؂�ւ�
    /// </summary>
    public void BlackOutFirst()
    {
        _stagingFader.enabled = true;
        //_stagingFaderAnim.Play("StagingFaderAnimFirst");
        _backGroundAnim.Play("BackGroundAnimFirst");
    }
    /// <summary>
    /// ���v�̉��ɍ��킹������
    /// </summary>
    public IEnumerator BlackOutIntermittently()
    {
        var i = 0;
        _stagingFaderAnim.Play("StagingFaderAnim");
        _cameraShine.intensity.value = 20f;
        while (true)
        {
            if (effectStart)
            {
                _stagingFaderAnim.Play("StagingFaderAnim");
                _cameraShine.intensity.value = 20f;
                effectStart = false;
            }
            yield return new WaitForFixedUpdate();
            i++;
            _cameraShine.intensity.value = (float) (_cameraShine.intensity.value - 0.16);
            if (i <= 60) continue;
            _cameraShine.intensity.value = 10f;
            break;
        }
    }
}
