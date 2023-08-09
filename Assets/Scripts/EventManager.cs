using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
   public delegate void Pause(bool IsPaused);
   public static event Pause OnPause;

   public delegate void Damaged();
   public static event Damaged OnDamage;

   public delegate void Restarted();
   public static event Restarted OnRestart;

   public delegate void Mute(bool IsMuted);
   public static event Mute OnMute;

   public delegate void Killed();
   public static event Killed OnKill;

   public delegate void Healed();
   public static event Healed OnHeal;

   private bool _isPaused = true;


   public void Paused(bool IsPaused)
   {
      if (OnPause != null)
      {
         _isPaused = IsPaused;
         OnPause(IsPaused);
      }
   }

   public void Restart()
   {
      if (OnRestart != null)
      {
         _isPaused = false;
         OnRestart();
      }
   }

   public void Muted(bool IsMuted)
   {
      if (OnMute!=null)
         OnMute(IsMuted);
   }

   public void Damage()
   {
      if (OnDamage != null)
         OnDamage();
   }

   public void Kill()
   {
      if (OnKill!=null)
         OnKill();
   }

   public void Heal()
   {
      if (OnHeal != null)
         OnHeal();
   }

   // Update is called once per frame
   void FixedUpdate()
   {

   }
}
