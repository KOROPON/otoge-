using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    private bool _startBool;
    public AudioSource titleMusic;

    public static float aspect;
    void Start()
    {
        _startBool = true;
        SceneManager.LoadScene("ShutterScene", LoadSceneMode.Additive);

        float width = Screen.width;
        float hight = Screen.height;
        aspect = width / hight;
        Debug.Log(aspect);
    }
    public void Change()
    {
        if (titleMusic.isPlaying)
        {
            if (_startBool)
            {
                _startBool = false;
                Shutter.blChange = "ToS_F";
                Shutter.blShutterChange = "Close";
                Invoke("StopTitle", 0.5f);
            }
        }
    }
    private void StopTitle()
    {
        titleMusic.Stop();
    }
}