using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicNumManage : MonoBehaviour
{
    public int music_number;
    public void music1() {
      if (music_number==1){
        SceneManager.LoadScene("PlayScene");
      }
      music_number=1;
    }
    public void music2(){
      if (music_number==2){
        SceneManager.LoadScene("PlayScene");
      }
      music_number=2;
    }
}
