using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
   public GameObject PausePanel;

   public void Pause()
   {
      EventManager.Instance.Pause(true);
      PausePanel.SetActive(true);
   }

   public void Resume()
   {
      EventManager.Instance.Pause(false);
   }

   public void Restart()
   {
      EventManager.Instance.Restart();
   }

   public void Exit()
   {
      Application.Quit();
   }

   public void MuteSounds()
   {
      EventManager.Instance.Mute(true);
   }
   public void UnmuteSounds()
   {
      EventManager.Instance.Mute(false);
   }
}
