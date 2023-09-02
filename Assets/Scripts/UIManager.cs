using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // required when using UI elements in scripts
using TMPro;

public class UIManager : MonoBehaviour
{
   public static UIManager Instance;

   private short _life = 3;
   public GameObject[] Life;

   public float _stamina { get; private set; } = 3.0f;
   public float _maxStamina { get; private set; } = 3.0f;
   public Slider Stamina;
   public bool StaminaRecover { get; set; } = false;

   public float _magic { get; private set; } = 3.0f;
   public float _maxMagic { get; private set; } = 3.0f;
   public Slider Magic;
   private bool _magicRecover = false;
   public bool MagicRecover 
   { 
      get
      {
         return _magicRecover;
      }
     set { _magicRecover = value; MagicDecreasing = !value; }
   }
   public bool MagicDecreasing { get; private set; } = false;

   private bool _isDead;
   private bool _isPaused;

   public TextMeshProUGUI Score;
   private int _score = 0;

   public Button AttackButton;
   public Button RollButton;
   public Button ExplodeButton;

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
      EventManager.OnExplode += ExplodeListener;
      _stamina = 3.0f;
      _maxStamina = 3.0f;
      _magic = 3.0f;
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
      ExplodeButton.interactable = false;
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
      foreach (GameObject heart in Life)
         heart.SetActive(true);

      _stamina = 3.0f;
      _maxStamina = 3.0f;
      Stamina.value = 1.0f;
      StaminaRecover = false;

      _magic = 3.0f;
      Magic.value = 1.0f;
      MagicRecover = false;

      _score = 0;
      Score.text = "Score:" + _score.ToString();

      AttackButton.interactable = true;
      RollButton.interactable = true;
      ExplodeButton.interactable = true;

      _isDead = false;

   }

   private void PauseListener(bool IsPaused)
   {
      _isPaused = IsPaused;
   }

   private void ExplodeListener()
   {
      if (_magic >= 3.0f)
      {
         MagicRecover = false;
         _magic -= 3.0f;
      }
   }

   public void SpendStamina()
   {
      if (_stamina >= 1.0f)
      {
         _stamina -= 1.0f;
      }
   }

   public void RecoverStamina(float value) // fast recover by stamina potion
   {
      _stamina = _stamina + value > _maxStamina ? _maxStamina : _stamina + value;
      Stamina.value = _stamina / _maxStamina;
      AttackButton.interactable = true;
      RollButton.interactable = true;
   }

   public void RecoverMagic(float value) // fast recover by magic potion
   {
      _magic = _magic + value > _maxMagic ? _maxMagic : _magic + value;
      Magic.value = _magic / _maxMagic;
      ExplodeButton.interactable = true;
   }

   void FixedUpdate()
   {
      if (!_isDead && !_isPaused)
      {
         if (!StaminaRecover && Stamina.value > _stamina / _maxStamina) // Decreasing Stamina
         {
            Stamina.value -= 0.005f;
            if (_stamina <= 1.0f)
            {
               AttackButton.interactable = false;
               RollButton.interactable = false;
            }
         }
         if (!MagicRecover && Magic.value > _magic / _maxMagic) // Decreasing Magic
         {
            Magic.value -= 0.005f;
            if (Magic.value <= _magic / _maxMagic)
            {
               MagicDecreasing = false;
            }
            if (_magic <= 3.0f)
            {
               ExplodeButton.interactable = false;
            }
         }
      }
      if (!_isDead && !_isPaused)
      {
         if (StaminaRecover && Stamina.value < 1.0f) // Recover Stamina
         {
            Stamina.value += 0.0005f;
            _stamina = Stamina.value * _maxStamina;
            if (_stamina > 1.0)
            {
               AttackButton.interactable = true;
               RollButton.interactable = true;
            }
         }
         if (MagicRecover && Magic.value < 1.0f) // Recover Magic
         {
            Magic.value += 0.00025f;
            _magic = Magic.value * _maxMagic;
            if (_magic >= 3.0f)
            {
               ExplodeButton.interactable = true;
            }
         }
      }
   }
}
