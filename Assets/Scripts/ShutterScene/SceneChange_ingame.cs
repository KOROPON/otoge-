using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange_ingame : MonoBehaviour
{
  public void Retry()
  {
      Time.timeScale = 1;
      //シャッター下げる
      SceneManager.UnloadSceneAsync("PlayScene", UnloadSceneOptions.None);
      SceneManager.LoadScene("PlayScene", LoadSceneMode.Additive);
  }
}
