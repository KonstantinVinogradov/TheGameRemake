using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
   [SerializeField] private RectTransform _rectTransform;
   [SerializeField] private Rigidbody2D _rigidbody;
   [SerializeField] private FixedJoystick _joystick;
   [SerializeField] private Animator _animator;

   [SerializeField] private AudioSource HealSound;

   [SerializeField] private float _moveSpeed;
   [SerializeField] private SpriteRenderer _spriteRenderer;

   public AudioSource SwordAttack;
   public AudioSource LifeDecrease;
   private short _life = 3;
   private bool _isPaused;
   private bool _isRolling;
   private bool _isMoving; // перекат можно сделать только из состояния движения(вроде логично)
   private bool _isAttacking;
   private bool hitted;
   private float _timeForRolling = 0.0f;
   private float _timeForAttack = 0.0f;

   public Transform AttackPoint;
   public float AttackRange;
   public LayerMask DamageableLayer;

   void Awake()
   {
      EventManager.OnDamage += DamageListener; //игрок подписан на получение урона
      EventManager.OnPause += PauseListener; // игрок реагирует на паузу
      EventManager.OnRestart += RestartListener; // игрок реагирует на рестарт игры
      EventManager.OnMute += Mute;
   }

   void DamageListener()
   {
      if (_life > 0)
      {
         _life--;
         LifeDecrease.Play();
      }
      if (_life == 0)
      {
         EventManager.Instance.Die();
         _isPaused= true;
         _animator.SetBool("IsDead", true);
         _animator.SetBool("IsAttacking", false);
         _animator.SetBool("IsRunning", false);
      }
   }

   void PauseListener(bool IsPaused)
   {
      _isPaused = IsPaused;
   }

   void RestartListener()
   {
      _life = 3;
      _rectTransform.anchoredPosition = new Vector2(400.0001f, 225.0f);
      _rigidbody.velocity = new Vector2(0, 0);
      _spriteRenderer.flipX = false;
      _isPaused = false;
      _isAttacking = false;
      _animator.SetBool("IsRunning", false);
      _animator.SetBool("IsAttacking", false);
      _animator.SetBool("IsRolling", false);
      _animator.SetBool("IsDead", false);
   }

   void Mute(bool IsMuted)
   {
      SwordAttack.mute = IsMuted;
      LifeDecrease.mute = IsMuted;
      HealSound.mute = IsMuted;
   }

   void MoveByJoystick()
   {
      _rigidbody.velocity = new Vector2(_joystick.Horizontal * _moveSpeed, _joystick.Vertical * _moveSpeed);
      if (_joystick.Horizontal > 0.01f)
      {
         _spriteRenderer.flipX = false;
         _animator.SetBool("IsRunning", true);
         _isMoving = true;
         _animator.SetBool("IsAttacking", false);
         AttackPoint.localPosition = new Vector2(0.2f, 0f);
      }
      else if (_joystick.Horizontal < -0.01f)
      {
         _spriteRenderer.flipX = true;
         _isMoving = true;
         _animator.SetBool("IsRunning", true);
         _animator.SetBool("IsAttacking", false);
         AttackPoint.localPosition = new Vector2(-0.2f, 0f);
      }
      else
      {
         if (_joystick.Vertical < 0.01f && _joystick.Vertical > -0.01f)
         {
            _isMoving = false;
            _animator.SetBool("IsRunning", false);
         }
         else
         {
            _isMoving = true;
            _animator.SetBool("IsRunning", true);
         }
      }
      if (_isRolling == true)
      {
         _timeForRolling += Time.deltaTime;
         if (_timeForRolling > 1.1f)
         {
            _moveSpeed *= 0.5f;
            _isRolling = false;
            _animator.SetBool("IsRolling", false);
            _timeForRolling = 0;
         }
      }
   }

   void MoveByKeyboard()
   {
      Vector2 direction = new Vector2(0, 0);

      if (Input.GetKey(KeyCode.W))
         direction.y += 1.0f;
      else
         direction.y += 0.0f;

      if (Input.GetKey(KeyCode.A))
         direction.x += -1.0f;
      else
         direction.x += 0.0f;

      if (Input.GetKey(KeyCode.S))
         direction.y += -1.0f;
      else
         direction.y += 0.0f;

      if (Input.GetKey(KeyCode.D))
         direction.x += 1.0f;
      else
         direction.x += 0.0f;

      direction.Normalize();
      _rigidbody.velocity = new Vector2(direction.x * _moveSpeed, direction.y * _moveSpeed);
      if (_rigidbody.velocity.x > 0.01f)
      {
         _spriteRenderer.flipX = false;
         _animator.SetBool("IsRunning", true);
         _isMoving = true;
         AttackPoint.localPosition = new Vector2(0.2f, 0f);
      }
      else if (_rigidbody.velocity.x < -0.01f)
      {
         _spriteRenderer.flipX = true;
         _isMoving = true;
         _animator.SetBool("IsRunning", true);
         AttackPoint.localPosition = new Vector2(-0.2f, 0f);
      }
      else
      {
         if (_rigidbody.velocity.y < 0.01f && _rigidbody.velocity.y > -0.01f)
         {
            _isMoving = false;
            _animator.SetBool("IsRunning", false);
         }
         else
         {
            _isMoving = true;
            _animator.SetBool("IsRunning", true);
         }
      }
      if (_isRolling == true)
      {
         _timeForRolling += Time.deltaTime;
         if (_timeForRolling > 0.6666666666666666f) // длина анимации в секундах делить на параметр ускорения анимации
         {
            _moveSpeed *= 0.5f;
            _isRolling = false;
            _animator.SetBool("IsRolling", false);
            _timeForRolling = 0;
         }
      }
   }

   private void FixedUpdate()
   {
      if (_rectTransform.transform.localPosition.x < -407.0f || _rectTransform.transform.localPosition.x > 407.0f ||
         _rectTransform.transform.localPosition.y < -220.0f || _rectTransform.transform.localPosition.y > 220.0f)
      {
         _rectTransform.transform.localPosition = new Vector2(0.0f, 0.0f);
      }
      if (!_isPaused) // определяем стоит ли игра на паузе
      {
         if (!_isAttacking)
         {
            MoveByJoystick();
            //MoveByKeyboard();
         }
         else
         {
            _rigidbody.velocity = new Vector2(0, 0);
         }
         if (_isAttacking)
         {
            _timeForAttack += Time.deltaTime;
            if (_timeForAttack > 0.1f)
            {
               if (!hitted) // спустя половину анимации засчитываем удар
               {
                  SwordAttack.Play();
                  Collider2D[] enemies = Physics2D.OverlapCircleAll(AttackPoint.position, AttackRange, DamageableLayer);
                  foreach (var enemy in enemies)
                     if (enemy != null)
                        enemy.GetComponent<Enemy>().TakeDamage();
                  hitted = true;
               }
               if (_timeForAttack > 0.7f) // спустя вторую половину разрешаем бить снова
               {
                  _isAttacking = false;
                  _animator.SetBool("IsAttacking", false);
                  _timeForAttack = 0;
                  hitted = false;
               }
            }
         }
      }
      else
      {
         _rigidbody.velocity = new Vector2(0, 0);
         _animator.SetBool("IsRunning", false);
         _animator.SetBool("IsAttacking", false);
         _isMoving = false;
      }
   }

   private void OnDrawGizmosSelected()
   {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
   }

   public void Attack()
   {
      if (!_isRolling)
      {
         _animator.SetBool("IsRunning", false);
         _animator.SetBool("IsAttacking", true); // анимация атаки включается сразу после нажатия кнопки, а урон только по прошествии времени равному длительности анимации
         _isMoving = false;
         _isAttacking = true;
      }
      //урон наносится в FixedUpdate
   }

   public void Roll()
   {
      if (_isMoving && !_isRolling && !_isAttacking)
      {
         _animator.SetBool("IsRolling", true);
         _moveSpeed *= 2.0f;
         _isRolling = true;
      }
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.gameObject.tag == "Health")
      {
         if (_life < 3 && _life > 0)
         {
            _life++;
            EventManager.Instance.Heal();
            HealSound.Play();
            Destroy(other.gameObject);
         }
      }
   }
}