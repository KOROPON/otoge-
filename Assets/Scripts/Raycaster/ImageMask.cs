using UnityEngine;
using UnityEngine.UI;

public class ImageMask : MonoBehaviour,ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return false;
    }
}
