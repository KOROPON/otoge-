using UnityEngine.UI;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    private Sprite _backSprite;

    private void Start()
    {
        _backSprite = gameObject.GetComponent<Image>().sprite;
        _backSprite = Resources.Load<Sprite>("BackGround/SelectBack");
    }

    public void InAnimationBehaviour()
    {
        _backSprite = Resources.Load<Sprite>("BackGround/Fader");
    }
}
