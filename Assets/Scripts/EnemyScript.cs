using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
   [SerializeField] private RectTransform _rectTransform;
   [SerializeField] private Rigidbody2D _rigidbody;
   [SerializeField] private Animator _animator;
   [SerializeField] private float _moveSpeed;
   [SerializeField] private SpriteRenderer _spriteRenderer;
   [SerializeField] private Slider HealthBar;

   private GameObject Player;
   public GameObject EnemyObject;
   public GameObject SkeletonPrefab;
   public GameObject HealthPrefab;
   //public GameObject BackGround;
   public EventManager events;
   public AudioSource DeathSound;
   public AudioSource LifeDecrease;


   private Vector2 _direction;
   private Rigidbody2D _playerRigidbody2D;
   private System.Random rnd = new System.Random();
   private bool _isPaused = true;
   private bool _isDead = false;
   private bool _isDamaged = false; // флаг для отсчёта времени появления полосы здоровья
   private bool _isSpawning = false;
   private float _timeForAttack = 0.0f; // время от начала атаки после которого урон будет засчитан
   private float _timeForDecay = 0.0f; // время после смерти врага через которое он будет удалён
   private float _timeForHealthBar = 0.0f;
   private float _timeToSpawn = 0.0f;
   private short _health = 3;
   private const short _maxHealth = 3;

   void Awake()
   {
      EventManager.OnPause += PauseListener; // подписка на событие OnPause
      EventManager.OnDeath += DeathListener;
      //EventManager.OnRestart += RestartListener; // подписка на событие Restart

      Player = GameObject.Find("Player");
      _playerRigidbody2D = Player.GetComponent<Rigidbody2D>();
      EnemyObject.SetActive(true);
      HealthBar.gameObject.SetActive(false);
      _health = 3;
      _isPaused = false;
      _timeForAttack = 0.0f;
      _isDead = false;
      _isDamaged = false;
      _isSpawning = true;
      _timeForAttack = 0.0f;
      _timeForDecay = 0.0f;
      _timeForHealthBar = 0.0f;
      _rigidbody.velocity = new Vector2(0.0f, 0.0f);

      _animator.SetBool("IsRunning", false);
      _animator.SetBool("IsAttacking", false);
      _animator.SetBool("IsDead", false);
      _animator.SetBool("IsSpawning", true);
      _spriteRenderer.flipX = true;
      
   }

   void PauseListener(bool IsPaused) // функция которая будет выполняться при событии "пауза"
   {
      _isPaused = IsPaused;
   }

   void RestartListener()
   {

      _health = 3;
      _isPaused = false;
      _timeForAttack = 0.0f;
      _isDead = false;
      _isDamaged = false;
      _timeForAttack = 0.0f;
      _timeForDecay = 0.0f;
      _timeForHealthBar = 0.0f;
      _timeToSpawn = 0.0f;
      _rigidbody.velocity = new Vector2(0.0f, 0.0f);

      _animator.SetBool("IsRunning", false);
      _animator.SetBool("IsAttacking", false);
      _animator.SetBool("IsDead", false);
      _spriteRenderer.flipX = true;
   }

   void DeathListener()
   {
      _isPaused = true;
   }

   void FixedUpdate()
   {
      if (!_isPaused) // определяем стоит ли игра на паузе (не зависимо от того жив ли враг)
      {
         if (_isSpawning)
         {
            _timeToSpawn += Time.deltaTime;
            if (_timeToSpawn > 3.0f)
            {
               _timeToSpawn = 0.0f;
               _isSpawning = false;
               _animator.SetBool("IsSpawning", false);
            }
         }
         else // урон не может быть нанесён во время спавна
         {
            if (_isDamaged)
            {
               HealthBar.gameObject.SetActive(true);
               _timeForHealthBar += Time.deltaTime;
               if (_timeForHealthBar > 2.0f)
               {
                  _isDamaged = false;
                  HealthBar.gameObject.SetActive(false);
                  _timeForHealthBar = 0.0f;
               }
            }
            if (HealthBar.value > (float)_health / _maxHealth)
            {
               HealthBar.value -= 0.005f;
            }
            if (HealthBar.value <= 0)
            {
               HealthBar.gameObject.SetActive(false);
            }
         }
      }
      if (!_isPaused && !_isDead && !_isSpawning) // отдельный блок для случаев если игра не на паузе и враг не мёртв
      {
         _playerRigidbody2D = Player.GetComponent<Rigidbody2D>();
         _direction = _playerRigidbody2D.position - _rigidbody.position;
         _spriteRenderer.flipX = (_direction.x < 0.0f) ? true : false;
         if (_direction.magnitude > 1.2f && _direction.magnitude < 6.0f) // если игрок не слишком далеко но ещё недоступен для ближней атаки
         {
            _direction.Normalize();
            _rigidbody.velocity = new Vector2(_direction.x * _moveSpeed, _direction.y * _moveSpeed);
            _animator.SetBool("IsRunning", true);
            _animator.SetBool("IsAttacking", false);
            _timeForAttack = 0.0f;
         }
         else if (_direction.magnitude > 6.0f) // если игрок далеко
         {
            _rigidbody.velocity = new Vector2(0.0f, 0.0f);
            _animator.SetBool("IsRunning", false);
            _animator.SetBool("IsAttacking", false);
            _timeForAttack = 0.0f;
         }
         else // если игрок достаточно близок для ближней атаки
         {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x/1.2f, _rigidbody.velocity.y / 1.2f);
            _animator.SetBool("IsAttacking", true);
            _timeForAttack += Time.deltaTime;
            if (_timeForAttack > 1.1f) // если анимация атаки кончилась
            {
               events.Damage(); // сообщаем игроку что мы его ударили
               _timeForAttack = 0.0f; // обнуляем счётчик времени
            }
         }
         if (_health <= 0)
         {
            _animator.SetBool("IsDead", true);
            _isDead = true;
         }

      }
      else
      {
         _rigidbody.velocity = new Vector2(0.0f, 0.0f);
         _animator.SetBool("IsRunning", false);
         _animator.SetBool("IsAttacking", false);
         _timeForAttack = 0.0f;
         if (_isDead)
         {
            _timeForDecay += Time.deltaTime;
            if (_timeForDecay > 3.0f)
            {
               _timeForDecay = 0.0f;

               if (rnd.NextDouble() < 1.0/4.0) // есть шанс что из трупа выпадет лут
               {
                  GameObject Health = Instantiate(HealthPrefab, new Vector2(0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f)) as GameObject;
                  GameObject BackGround = GameObject.Find("BackGround");
                  //Health.transform.SetParent(BackGround.transform);
                  Health.transform.localPosition = _rectTransform.transform.localPosition;
               }
               Destroy(EnemyObject);
            }
         }
      }
   }

   public void TakeDamage()
   {
      if (_health > 0)
      {
         _health--;
      }
      if (_health == 0 && !_isDead)
      {
         DeathSound.Play();
         events.Kill();
      }
      else if (!_isDead)
      {
         LifeDecrease.Play();
      }
      _isDamaged = true;
      _timeForHealthBar = 0.0f;
   }

   public void Mute(bool mute)
   {
      DeathSound.mute = mute;
      LifeDecrease.mute = mute;
   }
}
