using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
   public delegate void Paused(bool IsPaused);
   public static event Paused OnPause;

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

   public delegate void Dead();
   public static event Dead OnDeath;

   private System.Random rnd = new();

   private bool _isPaused = true;

   public GameObject DeathScreen;
   public GameObject EnemyPrefab;

   private float _timeForNewEnemy = 0.0f;

   private HashSet<GameObject> Enemies = new();

   public void Pause(bool IsPaused)
   {
      if (OnPause != null)
      {
         _isPaused = IsPaused;
         OnPause(IsPaused);
      }
   }

   public void Start()
   {
      _isPaused = false;
      GameObject Enemy = Instantiate(EnemyPrefab, new Vector2(0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f)) as GameObject;
      GameObject Space = GameObject.Find("Space");
      Enemy.transform.SetParent(Space.transform);
      Enemy.transform.localPosition = new Vector2(240.2f, 9.6f);
      Enemies.Add(Enemy);
      _timeForNewEnemy = 0.0f;
   }

   public void Restart()
   {
      if (OnRestart != null)
      {
         _isPaused = false;
         foreach (GameObject enemy in Enemies)
         {
            Destroy(enemy);
         }
         Enemies.Clear();
         GameObject Enemy = Instantiate(EnemyPrefab, new Vector2(0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f)) as GameObject;
         GameObject Space = GameObject.Find("Space");
         Enemy.transform.SetParent(Space.transform);
         Enemy.transform.localPosition = new Vector2(240.2f, 9.6f);
         Enemies.Add(Enemy);
         _timeForNewEnemy = 0.0f;
         DeathScreen.SetActive(false);
         OnRestart();
      }
   }

   public void Muted(bool IsMuted)
   {
      if (OnMute != null)
         OnMute(IsMuted);
   }

   public void Damage()
   {
      if (OnDamage != null)
         OnDamage();
   }

   public void Kill()
   {
      if (OnKill != null)
         OnKill();
   }

   public void Heal()
   {
      if (OnHeal != null)
         OnHeal();
   }

   public void Die()
   {
      if (OnDeath != null)
      {
         DeathScreen.SetActive(true);
         _isPaused = true;
         OnDeath();
      }
   }

   void FixedUpdate()
   {
      if (!_isPaused)
      {
         _timeForNewEnemy += Time.deltaTime;
         if (_timeForNewEnemy > 5.0f)
         {
            GameObject Enemy = Instantiate(EnemyPrefab, new Vector2(0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f)) as GameObject;
            GameObject Space = GameObject.Find("Space");
            Enemy.transform.SetParent(Space.transform);
            Enemy.transform.localPosition = new Vector2(   (float)rnd.Next(-376, 376)   , (float)rnd.Next(-188, 183)   );
            Enemies.Add(Enemy);
            _timeForNewEnemy = 0.0f;
         }
      }
   }
}
