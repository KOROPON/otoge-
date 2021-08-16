using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace Reilas
{
  public class MusicNumManage : MonoBehaviour
  {
      RawImage jack;
      private Text text1;
      private AudioSource audioSource;
      public int music_number = -1;
      private AudioClip collide;
      private AudioClip devou;


      void Start()
      {
          jack = GameObject.Find("ジャケット1").GetComponent<RawImage>();
          audioSource = GameObject.Find("Audio Source Intro").GetComponent<AudioSource>();
      }

      private void musicInfo(string music_name,string jacketPath,int score) {
          audioSource.clip = Resources.Load<AudioClip>(music_name);
          audioSource.Play();
          jack.texture = Resources.Load<Texture2D>(jacketPath);
          text1.text = music_name;
      }

      public void Tap(GameObject obj)
      {
          Debug.Log(obj.name + "_intro");
          if(music_number == obj.transform.GetSiblingIndex())
          {
              RhythmGamePresenter.musicname = obj.name;
              SceneManager.LoadScene("PlayScene");
          }
          else
          {
              music_number = obj.transform.GetSiblingIndex();
          }
          musicInfo("Songs/Music Select/" + obj.name + "_intro", "Jacket/" + obj.name + "_jacket", 1000000);
      }

      /*public void Music1()
      {
          musicInfo("Collide","Music_Is_My_Suicide_Jacket",1000000);
          audioSource.clip = collide;
          audioSource.Play();
          if (music_number == 1)
          {
              SceneManager.LoadScene("PlayScene");
          }
          music_number = 1;
      }
      public void Music2() {
        musicInfo("Devourer Of Sol_Ⅲ","uchuu",1000000);
        audioSource.clip = devou;
        audioSource.Play();
        if (music_number == 2) {
          SceneManager.LoadScene("PlayScene");
        }
        music_number = 2;
      }*/
  }
}
