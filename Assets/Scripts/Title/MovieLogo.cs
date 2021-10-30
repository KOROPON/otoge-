using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(RawImage), typeof(VideoPlayer))]
public class MovieLogo : MonoBehaviour
{
    RawImage _image;
    VideoPlayer _player;
    void Awake()
    {
        Application.targetFrameRate = 60;
        _image = GetComponent<RawImage>();
        _player = GetComponent<VideoPlayer>();

        _player.EnableAudioTrack(0, true);

    }
    void Update()
    {
        if (_player.isPrepared)
        {
            _image.texture = _player.texture;
        }
    }
}