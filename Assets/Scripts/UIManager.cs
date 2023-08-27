using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // required when using UI elements in scripts
using TMPro;

public class UIManager : MonoBehaviour
{
   public static UIManager Instance;

   private short _life = 3;
   public float _stamina { get; private set; } = 3.0f;
   private float _maxStamina = 3.0f;
   public GameObject[] Life;
   public Slider Stamina;
   public bool StaminaRecover { get; set; } = false;
   private bool _isDead;
   private bool _isPaused;

   public TextMeshProUGUI Score;
   private int _score = 0;

   public Button AttackButton;
   public Button RollButton;

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
   }

   private void Awake()
   {
      EventManager.OnDamage += DamageListener;
      EventManager.OnDeath += DeathListener;
      EventManager.OnHeal += HealListener;
      EventManager.OnKill += KillListener;
      EventManager.OnRestart += RestartListener;
      EventManager.OnPause += PauseListener;
      _stamina = 3;
      _maxStamina = 3;
   }

   private void DamageListener()
   {
      _life--;
      if (_life == 2)
      {
         Life[2].SetActive(false);
      }
      else if (_life == 1)
      {
         Life[1].SetActive(false);
      }
      else
      {
         Life[0].SetActive(false);
      }
   }

   private void DeathListener()
   {
      AttackButton.interactable = false;
      RollButton.interactable = false;
      _isDead = true;

   }

   private void HealListener()
   {
      _life++;
      if (_life == 3)
      {
         Life[2].SetActive(true);
      }
      else if (_life == 2)
      {
         Life[1].SetActive(true);
      }
      else
      {
         Life[0].SetActive(true);
      }
   }

   private void KillListener()
   {
      _score++;
      SaveManager.Instance.SaveScore(_score);
      Score.text = "Score:" + _score.ToString();
   }

   private void RestartListener()
   {
      _life = 3;
      _stamina = 3.0f;
      _maxStamina = 3.0f;
      Stamina.value = 1.0f;
      StaminaRecover = false;
      foreach (GameObject heart in Life)
         heart.SetActive(true);
      _score = 0;
      Score.text = "Score:" + _score.ToString();
      AttackButton.interactable = true;
      RollButton.interactable = true;
      _isDead = false;
      
   }

   private void PauseListener(bool IsPaused)
   {
      _isPaused = IsPaused;
   }

   public void SpendStamina()
   {
      if (_stamina >= 1.0)
      {
         _stamina-=1.0f;
      }
   }

   void FixedUpdate()
   {
      if (!StaminaRecover && Stamina.value > _stamina / _maxStamina && !_isDead && !_isPaused) // Decreasing Stamina
      {
         Stamina.value -= 0.005f;
         if (_stamina <= 1.0)
         {
            AttackButton.interactable = false;
            RollButton.interactable = false;
         }
      }
      if (StaminaRecover && Stamina.value < 1.0f && !_isDead && !_isPaused) // Recover Stamina
      {
         Stamina.value += 0.0005f;
         _stamina = Stamina.value * _maxStamina;
         if (_stamina > 1.0)
         {
            AttackButton.interactable = true;
            RollButton.interactable = true;
         }
      }
   }
}
