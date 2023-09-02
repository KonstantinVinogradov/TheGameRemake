using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
   [SerializeField] private Animator _animator;
   private GameObject self;

   private bool _isPaused = false;
   private bool _blinking = false;

   private float _timeToDestroy = 0.0f;
   public float TTL;
   public float TimeB4Blink;

   void Awake()
   {
      self = this.gameObject;
      EventManager.OnPause += PauseListener; // подписка на событие OnPause
      EventManager.OnRestart += RestartListener;
   }

   void PauseListener(bool IsPaused) // функция которая будет выполняться при событии "пауза"
   {
      _isPaused = IsPaused;
   }

   void RestartListener()
   {
      _isPaused = false;
      _blinking = false;
      _timeToDestroy = 0.0f;
      Destroy(self);
   }

   void FixedUpdate()
   {
      if (!_isPaused)
      {
         _timeToDestroy += Time.deltaTime;
         if (_timeToDestroy > TimeB4Blink && !_blinking)
         {
            _animator.SetBool("Blinking", true);
            _blinking = true;
         }
         else if (_timeToDestroy > TTL)
         {
            Destroy(self);
         }
      }
   }
}
