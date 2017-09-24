/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffItem : UIBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] Image _cdMask;
    [SerializeField] Text _cdText;

    public void UpdateData(Buff buff)
    {
        var potion = AlchemyLibrary.Instance.FindPotion(buff.potionId);
        _icon.gameObject.SetActive(potion != null);
        if (potion != null)
            _icon.sprite = potion.icon;
        _cdMask.fillAmount = buff.remaingRatio;
        _cdText.text = buff.remainingSeconds.ToString();
    }
}
