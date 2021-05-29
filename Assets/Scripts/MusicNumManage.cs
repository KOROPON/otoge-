using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MusicNumManage : MonoBehaviour
{
    void musicInfo(string music_name,string musicPath,int score) {
      GameObject.Find("ジャケット1").GetComponent<RawImage>().texture=Resources.Load<Texture2D>(musicPath);
      GameObject.Find("タイトル").GetComponent<Text>().text=music_name;
    }
    public int music_number;
    public void music1() {
      musicInfo("Collide","Music_Is_My_Suicide_Jacket",1000000);
      if (music_number==1){
        SceneManager.LoadScene("PlayScene");
      }
      music_number=1;
    }
    public void music2(){
      musicInfo("Devourer Of Sol_Ⅲ","uchuu",1000000);
      if (music_number==2){
        SceneManager.LoadScene("PlayScene");
      }
      music_number=2;
    }
}
