using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // required when using UI elements in scripts

public class UIManager : MonoBehaviour
{
   private short _life = 3;
   public GameObject[] Life;

   public Button AttackButton;
   public Button RollButton;

   void Awake()
   {
      EventManager.OnDamage += DamageListener;
      EventManager.OnDeath += DeathListener;
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

}
