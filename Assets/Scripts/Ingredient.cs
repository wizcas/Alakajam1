/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public IngredientData data;
    [SerializeField] SpriteRenderer _renderer;

    static FastPool Pool
    {
        get { return FastPoolManager.GetPool(1, null, false); }
    }

    public static Ingredient Make(string id, Vector3 pos)
    {
        var data = AlchemyLibrary.Instance.FindIngredient(id);
        if (data == null) return null;

        var ingredient = Pool.FastInstantiate<Ingredient>(pos, Quaternion.identity);
        ingredient.data = data;
        ingredient._renderer.sprite = data.icon;
        ingredient._renderer.color = Color.white;
        return ingredient;
    }

    public void Disappear()
    {
        Pool.FastDestroy(this);
    }

    public Tween Pop(float time)
    {
        return DOTween.Sequence()
            .Join(transform.DOJump(transform.position + Vector3.up, 1, 1, time))
            .Join(_renderer.DOFade(0, time * .3f).SetDelay(time * .7f))
            .OnComplete(() => Disappear());
    }
}

[System.Serializable]
public class IngredientData : IRuntimeData
{
    public string id;
    public Sprite icon;
    public string displayName;
    public Essence essence;
    public int stock;

    public object Copy()
    {
        return new IngredientData
        {
            id = id,
            icon = icon,
            displayName = displayName,
            essence = essence,
            stock = stock
        };
    }

    public void Use(int amount = 1)
    {
        stock--;
        Messenger.Broadcast(Messages.StockChanged, this);
    }
}
