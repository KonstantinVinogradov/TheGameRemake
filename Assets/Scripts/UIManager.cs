using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // required when using UI elements in scripts
using TMPro;

public class UIManager : MonoBehaviour
{
   private short _life = 3;
   public GameObject[] Life;

   public TextMeshProUGUI Score;
   private int _score = 0;

   public Button AttackButton;
   public Button RollButton;

   private void Awake()
   {
      EventManager.OnDamage += DamageListener;
      EventManager.OnDeath += DeathListener;
      EventManager.OnHeal += HealListener;
      EventManager.OnKill += KillListener;
      EventManager.OnRestart += RestartListener;
   }

   private void DamageListener()
   {
      _life--;
      if (_life == 2)
      {
         Life[2].SetActive(false);
      }
      else if (_life==1)
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
      _score = 0;
      Score.text = "Score:" + _score.ToString();
      AttackButton.interactable = true;
      RollButton.interactable = true;
   }

}
