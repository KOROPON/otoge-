using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rhythmium;
using Reilas;

public class Shutter : MonoBehaviour
{
  private Animator anim;
  public static string blChange;
  public AudioSource openSE;
  public AudioSource closeSE;

   void Start()
   {
       anim = gameObject.GetComponent<Animator>();
   }

   void Update()
   {
     switch (blChange)
     {
       case "ToPFrS_close": blChange = "";anim.SetBool("blTpFs",true);break;
       case "ToPFrS_open": blChange = "";anim.SetBool("blTpFs",false);break;
       case "ToR_close": blChange = "";anim.SetBool("blTr",true);break;
       case "ToR_open": blChange = "";anim.SetBool("blTr",false);break;
       case "ToS_F_close": blChange = "";anim.SetBool("blTs_F",true);break;
       case "ToS_F_open": blChange = "";anim.SetBool("blTs_F",false);break;
     }
   }

   void OpenAudio()
   {
     openSE.Play();
   }
   void CloseAudio()
   {
     closeSE.Play();
   }
   void PlayAudio()
   {
     RhythmGamePresenter.PlaySongs();
     ChangeScene_PlayScene.playNoticed =true;
   }
   void PlaySongAudio()
   {
     Invoke("PlayAudio",1f);
   }



   void SelectLoadFirst()
   {
     SceneManager.LoadScene("SelectScene", LoadSceneMode.Additive);
     SceneManager.UnloadSceneAsync("Title Scene", UnloadSceneOptions.None);
   }
   void PlayLoadFromSelect()
   {
     SceneManager.LoadScene("PlayScene", LoadSceneMode.Additive);
     SceneManager.UnloadSceneAsync("SelectScene", UnloadSceneOptions.None);
   }
   void ResultLoad()
   {
     SceneManager.LoadScene("ResultScene", LoadSceneMode.Additive);
     SceneManager.UnloadSceneAsync("PlayScene", UnloadSceneOptions.None);
   }
}
