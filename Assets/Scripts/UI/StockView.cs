/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockView : MonoBehaviour 
{
    [SerializeField] Transform _root;
    [SerializeField] StockItem _stockItemPrefab;

    Dictionary<string, StockItem> _stockItemMap = new Dictionary<string, StockItem>();

    private void Awake()
    {
        Messenger.AddListener<IngredientData>(Messages.StockChanged, UpdateItem);
        var ingredients = AlchemyLibrary.Instance.Ingredients;
        foreach(var ingredient in ingredients)
        {
            MakeItem(ingredient);
        }
    }

    void MakeItem(IngredientData data)
    {
        var item = Instantiate(_stockItemPrefab);
        item.UpdateData(data);
        item.transform.SetParent(_root, false);
        _stockItemMap[data.id] = item;
    }

    void UpdateItem(IngredientData data)
    {
        StockItem item;
        if(!_stockItemMap.TryGetValue(data.id, out item))
        {
            return;
        }
        item.UpdateData(data);
    }
}
