using UnityEngine;
using UnityEngine.SceneManagement;
using Rhythmium;

public class Shutter : MonoBehaviour
{
  private Animator anim;
  public static string blChange;
  private bool reserver;
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
       case "Open": blChange = "";reserver = true;anim.SetBool("blOpen",true);  anim.SetBool("blTpFs",false); anim.SetBool("blTsFr",false);
                    anim.SetBool("blTpFr",false); anim.SetBool("blTsFp",false); anim.SetBool("blTr",false); anim.SetBool("blTs_F",false);
                    anim.SetBool("blTpFp",false);
                    break;
       case "ToPFrS": blChange = ""; if (reserver) anim.SetBool("blOpen",false); anim.SetBool("blTpFs",true);break;
       case "ToPFrP": blChange = ""; if (reserver) anim.SetBool("blOpen",false); anim.SetBool("blTpFp",true);break;
       case "ToSFrR": blChange = ""; if (reserver) anim.SetBool("blOpen",false); anim.SetBool("blTsFr",true);break;
       case "ToPFrR": blChange = ""; if (reserver) anim.SetBool("blOpen",false); anim.SetBool("blTpFr",true);break;
       case "ToSFrP": blChange = ""; if (reserver) anim.SetBool("blOpen",false); anim.SetBool("blTsFp",true);break;
       case "ToR": blChange = ""; if (reserver) anim.SetBool("blOpen",false); anim.SetBool("blTr",true);break;
       case "ToS_F": blChange = ""; if (reserver) anim.SetBool("blOpen",false); anim.SetBool("blTs_F",true);break;

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
     SettingField.SetBool = true;
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
   void SelectLoadFromResult()
   {
     SceneManager.LoadScene("SelectScene", LoadSceneMode.Additive);
     SceneManager.UnloadSceneAsync("ResultScene", UnloadSceneOptions.None);
   }
   void PlayLoadFromResult()
   {
     SceneManager.LoadScene("PlayScene", LoadSceneMode.Additive);
     SceneManager.UnloadSceneAsync("ResultScene", UnloadSceneOptions.None);
   }
   void SelectLoadFromPlay()
   {
     SceneManager.LoadScene("SelectScene", LoadSceneMode.Additive);
     SceneManager.UnloadSceneAsync("PlayScene", UnloadSceneOptions.None);
   }
   void PlayLoadFromPlay()
   {
     SceneManager.UnloadSceneAsync("PlayScene", UnloadSceneOptions.None);
     SceneManager.LoadScene("PlayScene", LoadSceneMode.Additive);
   }
}
