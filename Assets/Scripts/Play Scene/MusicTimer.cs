using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicTimer : MonoBehaviour
{
    public static float musicLength = 180f;
    public AnimationClip clip;
    void Start()
    {
        AnimationCurve curve;
        clip.legacy = true;
        // create a curve to move the GameObject and assign to the clip
        Keyframe[] keys;
        keys = new Keyframe[2];
        keys[0] = new Keyframe(0.0f, 0.0f);
        keys[1] = new Keyframe(musicLength, 180f);
        curve = new AnimationCurve(keys);
        clip.SetCurve("", typeof(Transform), "rotation.z", curve);

    }
}
