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

   void Awake()
   {
      EventManager.OnDamage += DamageListener;
      EventManager.OnDeath += DeathListener;
      EventManager.OnHeal += HealListener;
      EventManager.OnKill += KillListener;
   }

   void DamageListener()
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

   void DeathListener()
   {
      AttackButton.interactable = false;
      RollButton.interactable = false;
   }

   void HealListener() 
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

   void KillListener()
   {
      _score++;
      Score.text = "Score:" + _score.ToString();
   }

}
