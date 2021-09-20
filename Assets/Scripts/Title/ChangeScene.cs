using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
  private bool startBool;
  public AudioSource titleMusic;
  void Start()
  {
    startBool = true;
    SceneManager.LoadScene("ShutterScene", LoadSceneMode.Additive);
  }
  public void Change()
  {
    if(startBool)
    {
      startBool = false;
      Shutter.blChange = "ToS_F_close";
      Invoke("StopTitle",0.5f);
    }
  }
  private void StopTitle()
  {
    titleMusic.Stop();
  }
}
