using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
   public EventManager events;
   public GameObject PausePanel;

   public void Pause()
   {
      events.Pause(true);
      PausePanel.SetActive(true);
   }

   public void Resume()
   {
      events.Pause(false);
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
