using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shutter : MonoBehaviour
{
  private Animator anim;
  public static bool blTs_F_cl;
  public static bool blTs_F_op;
  public static bool blTpFs_cl;
  public static bool blTpFs_op;
  public static AnimationEvent evt;
  public AnimationClip closeC;
  public AnimationClip openC;
  public AudioSource openSE;
  public AudioSource closeSE;

   void Start()
   {
       anim = gameObject.GetComponent<Animator>();
   }

   void Update()
   {
     if (blTs_F_cl)
     {
       blTs_F_cl = false;
       anim.SetBool("blTs_F",true);
     }
     if (blTs_F_op)
     {
       blTs_F_op = false;
       anim.SetBool("blTs_F",false);
     }
     if (blTpFs_cl)
     {
       blTpFs_cl = false;
       anim.SetBool("blTpFs",true);
     }
     if (blTpFs_op)
     {
       blTpFs_op = false;
       anim.SetBool("blTpFs",false);
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


   void SelectLoad()
   {
     SceneManager.LoadScene("SelectScene", LoadSceneMode.Additive);
     SceneManager.UnloadSceneAsync("Title Scene", UnloadSceneOptions.None);
   }
   void PlayLoad()
   {
     SceneManager.LoadScene("PlayScene", LoadSceneMode.Additive);
     SceneManager.UnloadSceneAsync("SelectScene", UnloadSceneOptions.None);
   }
}
