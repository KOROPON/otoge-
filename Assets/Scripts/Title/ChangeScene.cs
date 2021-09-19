using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
  private bool startBool;
  void Start()
  {
    startBool = true;
    SceneManager.LoadScene("ShutterScene", LoadSceneMode.Additive);
  }
  public void Change(AudioSource titleMusic)
  {
    if(startBool)
    {
      startBool = false;
      titleMusic.Stop();
      //シャッターを閉じる
      SceneManager.LoadScene("SelectScene", LoadSceneMode.Additive);
      SceneManager.UnloadSceneAsync("Title Scene", UnloadSceneOptions.None);
    }
  }
}
