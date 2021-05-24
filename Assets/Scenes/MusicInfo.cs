using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
public class MusicInfo : MonoBehaviour
{
  void musicInfo(string music_name,string musicPath,int score) {
    GameObject.Find("ジャケット1").GetComponent<RawImage>().texture=AssetDatabase.LoadAssetAtPath(musicPath,typeof(Texture2D))as Texture2D;
    GameObject.Find("タイトル").GetComponent<Text>().text=music_name;

  }
    void Update()
    {
        if (GetComponent<MusicNumManage>().music_number==1){
          musicInfo("atomosphere","Assets/Scenes/ResultBack.png",1000000);
        }
        if (GetComponent<MusicNumManage>().music_number==2){
          GameObject.Find("ジャケット1").GetComponent<RawImage>().texture =AssetDatabase.LoadAssetAtPath("Assets/Scenes/Music_Is_My_Suicide_Jacket.png",typeof(Texture2D))as Texture2D ;
          GameObject.Find("タイトル").GetComponent<Text>().text="Collide";
        }

    }
}
