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
      Shutter.blTs_F_cl = true;
      titleMusic.Stop();
    }
  }

}
