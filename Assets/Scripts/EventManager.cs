using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
   public delegate void Paused(bool IsPaused);
   public static event Paused OnPause;

   public delegate void Damaged();
   public static event Damaged OnDamage; // событие если игрок получает урон

   public delegate void Restarted();
   public static event Restarted OnRestart;

   public delegate void Killed();
   public static event Killed OnKill;

   public delegate void Healed();
   public static event Healed OnHeal;

   public delegate void Dead();
   public static event Dead OnDeath; // событие если игрок умер

   public delegate void Muted(bool IsMuted);
   public static event Muted OnMute;

   public delegate void Exploded();
   public static event Exploded OnExplode;

   public static EventManager Instance { get; private set; }

   private System.Random rnd = new();

   private static bool _isPaused = true;
   private static bool _isMuted = false;
   private static bool _isDead = false;

   public GameObject DeathScreen;
   public GameObject EnemyPrefab;
   public GameObject MushroomPrefab;
   public GameObject ExplosionPrefab;

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
      if (Instance)
      {
         Destroy(this);
      }
      else
      {
         Instance = this;
         DontDestroyOnLoad(Instance);
      }
      _isPaused = true;
      
      GameObject Enemy = Instantiate(EnemyPrefab, new Vector2(0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f)) as GameObject;
      GameObject Space = GameObject.Find("Space");
      Enemy.transform.SetParent(Space.transform);
      Enemy.transform.localPosition = new Vector2(5.6f, 0.6f);
      Enemies.Add(Enemy);
      _timeForNewEnemy = 0.0f;
      if (OnPause != null)
         OnPause(true);
   }

   public void Restart()
   {
      if (OnRestart != null)
      {
         _isPaused = false;
         _isDead = false;
         foreach (GameObject enemy in Enemies)
         {
            Destroy(enemy);
         }
         Enemies.Clear();
         GameObject Enemy = Instantiate(EnemyPrefab, new Vector2(0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f)) as GameObject;
         GameObject Space = GameObject.Find("Space");
         Enemy.transform.SetParent(Space.transform);
         Enemy.transform.localPosition = new Vector2(5.6f, 0.6f);
         Enemy.GetComponent<Enemy>().Mute(_isMuted);
         Enemies.Add(Enemy);
         _timeForNewEnemy = 0.0f;
         DeathScreen.SetActive(false);
         OnRestart();
         OnPause(false);
      }
   }

   public void Damage() // событие если игрок получает урон
   {
      if (OnDamage != null)
         OnDamage();
   }

   public void Kill()
   {
      if (OnKill != null)
      {
         StartCoroutine(ClearNullReferenceWithDelay(4.0f));
         OnKill();
      }
   }

   public void Heal()
   {
      if (OnHeal != null)
         OnHeal();
   }

   public void Die() // событие если игрок умер
   {
      if (OnDeath != null)
      {
         DeathScreen.SetActive(true);
         _isPaused = true;
         _isDead = true;
         OnDeath();
      }
   }

   public void Mute(bool IsMuted)
   {
      _isMuted = IsMuted;
      if (OnMute!=null)
         OnMute(IsMuted);
      foreach (GameObject enemy in Enemies)
         enemy.GetComponent<Enemy>().Mute(IsMuted);
   }

   public void Explode()
   {
      if (OnExplode != null)
      {
         OnExplode();
         GameObject Space = GameObject.Find("Space");
         Enemies.RemoveWhere(item => item == null);
         foreach (GameObject enemy in Enemies)
         {
            if (enemy)
            {
               GameObject Explosion = Instantiate(ExplosionPrefab, new Vector2(0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f)) as GameObject;
               Explosion.transform.SetParent(Space.transform);
               Explosion.transform.localPosition = enemy.GetComponent<RectTransform>().anchoredPosition + new Vector2((float)rnd.NextDouble() * 2.0f - 1.0f, (float)rnd.NextDouble() * 2.0f - 1.0f);
               StartCoroutine(DestroyExplosionObjectWithDelay(Explosion, 1.167f));
            }
         }
      }
   }

   private System.Collections.IEnumerator DestroyExplosionObjectWithDelay(GameObject Explosion, float delay)
   {
      yield return new WaitForSeconds(delay);
      // Code will be executed after delay
      Destroy(Explosion);
   }

   private System.Collections.IEnumerator ClearNullReferenceWithDelay(float delay)
   {
      yield return new WaitForSeconds(delay);
      // Code will be executed after delay
      Enemies.RemoveWhere(item => item == null);
   }

   void FixedUpdate()
   {
      if (!_isPaused && !_isDead)
      {
         _timeForNewEnemy += Time.deltaTime;
         if (_timeForNewEnemy > 5.0f)
         {
            if (Enemies.Count < 100)
            {
               GameObject Enemy = (rnd.NextDouble() < 0.5) ? Instantiate(EnemyPrefab, new Vector2(0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f)) : Instantiate(MushroomPrefab, new Vector2(0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));
               GameObject Space = GameObject.Find("Space");
               Enemy.transform.SetParent(Space.transform);
               Enemy.transform.localPosition = new Vector2((float)rnd.NextDouble() * 16.99f - 8.48f, (float)rnd.NextDouble() * 6.45f - 3.2f);
               Enemy.GetComponent<Enemy>().Mute(_isMuted);
               Enemies.Add(Enemy);

               _timeForNewEnemy = 0.0f;
            }
         }
      }
   }
}
