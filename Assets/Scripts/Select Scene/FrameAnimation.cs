using UnityEngine;

public class FrameAnimation : MonoBehaviour
{
    public static Animator anim2;
    

    void Start()
    {
        anim2 = gameObject.GetComponent<Animator>();
        anim2.SetBool("blDifChange", false);
    }

   

    void BoolToFalse()
    {
        anim2.SetBool("blDifChange", false);
    }
}
