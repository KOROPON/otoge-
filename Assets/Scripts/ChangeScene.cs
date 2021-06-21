using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
  public void Change() {
    SceneManager.LoadScene("SelectScene");
  }
  public void DelayChange() {
    Invoke("Change",0.4f);
  }
}
