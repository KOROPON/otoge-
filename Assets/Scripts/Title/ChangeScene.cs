using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    private bool _startBool;
    public AudioSource titleMusic;

    public static float aspect;

    private void Start()
    {
        _startBool = true;
        SceneManager.LoadScene("ShutterScene", LoadSceneMode.Additive);

        float width = Screen.width;
        float height = Screen.height;
        aspect = width / height;
    }
    public void Change()
    {
        if (!titleMusic.isPlaying || !_startBool) return;
        
        _startBool = false;
        Shutter.blChange = "ToS_F";
        Shutter.blShutterChange = "Close";
        Invoke(nameof(StopTitle), 0.5f);
    }
    private void StopTitle()
    {
        titleMusic.Stop();
    }
}