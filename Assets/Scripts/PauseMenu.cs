using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
   public EventManager events;
   public GameObject PausePanel;
   void Awake()
   {
      EventManager.OnPause += PauseListener;
   }

   public void PauseListener(bool IsPaused)
   {
      PausePanel.SetActive(true);
   }

   public void Pause()
   {
      Debug.Log("Paused");
   }

   public void Resume()
   {
      events.Paused(false);
   }

   public void Restart()
   {
      events.Restart();
   }

   public void Exit()
   {
      Application.Quit();
   }
}
