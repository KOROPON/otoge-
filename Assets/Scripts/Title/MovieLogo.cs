using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(RawImage), typeof(VideoPlayer))]
public class MovieLogo : MonoBehaviour {
    RawImage image;
    VideoPlayer player;
    void Awake() {
        image = GetComponent<RawImage>();
        player = GetComponent<VideoPlayer>();

        player.EnableAudioTrack(0, true);
        
    }
    void Update() {
        if (player.isPrepared) {
            image.texture = player.texture;
        }
    }
}
