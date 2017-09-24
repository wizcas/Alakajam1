/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StockItem : MonoBehaviour 
{
    [SerializeField] Image _icon;
    [SerializeField] Text _amount;
    public Text key;

    public void UpdateData(IngredientData data)
    {
        _icon.sprite = data.icon;
        _amount.text = data.stock.ToString();
    }
}
