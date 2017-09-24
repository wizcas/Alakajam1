/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LabView : UIBehaviour 
{
    [SerializeField] Sprite _emptySprite;
    [SerializeField] Image[] _ingredientSlots;
    [SerializeField] Image _potionSlot;
    [SerializeField] Text _potionName;

    protected override void Awake()
    {
        Messenger.AddListener<LabData>(Messages.LabChanged, UpdateLabData);
    }

    void UpdateLabData(LabData data)
    {
        PrettyLog.Log("Updating lab data");
        for(int i = 0; i < _ingredientSlots.Length; i++)
        {
            Sprite icon = _emptySprite;
            if (i < data.ingredients.Count())
            {
                icon = data.ingredients.ElementAt(i).icon;
            }
            _ingredientSlots[i].sprite = icon;
        }
        _potionSlot.sprite = data.outcomePotion == null ? null : data.outcomePotion.icon;
        _potionSlot.gameObject.SetActive(_potionSlot.sprite != null);
        _potionName.text = data.outcomePotion == null ? string.Empty : data.outcomePotion.displayName;
        _potionName.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(_potionName.text));
    }
}
