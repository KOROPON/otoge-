using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
  void Start()
  {
    SceneManager.LoadScene("ShutterScene", LoadSceneMode.Additive);
  }
  public void Change(AudioSource titleMusic)
  {
    titleMusic.Stop();
    //シャッターを閉じる
    SceneManager.LoadScene("SelectScene", LoadSceneMode.Additive);

  }
  /*public void DelayChange()
  {
    Invoke("Change",0.4f);
  }
  */
}
