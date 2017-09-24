/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Potion : MonoBehaviour 
{
    public PotionData data;
    [SerializeField] SpriteRenderer _renderer;

    Rigidbody2D _rigid;
    public Rigidbody2D Rigid
    {
        get
        {
            if (_rigid == null)
                _rigid = GetComponent<Rigidbody2D>();
            return _rigid;
        }
    }

    static FastPool Pool
    {
        get
        {
            return FastPoolManager.GetPool(2, null, false);
        }
    }

    public void Break(bool isOnGround)
    {
        AfterBreak(isOnGround);
        Pool.FastDestroy(this);
    }

    public static Potion Make(PotionData data, Vector3 pos)
    {
        if (data == null) return null;
        var potion = Pool.FastInstantiate<Potion>(pos, Quaternion.identity);
        potion.data = data;
        potion._renderer.sprite = data.icon;
        return potion;
    }

    void AfterBreak(bool isOnGround)
    {
        if(data.kind == PotionData.Kind.Plant && isOnGround)
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.down, 1, LayerMask.GetMask("Ground"));
            var plant = Instantiate(data.plantPrefab);
            plant.transform.position = hit.point + Vector2.down * .2f;
        }
    }
}

[System.Serializable]
public class PotionData : IRuntimeData
{
    public enum Kind
    {
        Toss,
        Drink,
        Plant
    }
    public string id;
    public Sprite icon;
    public string displayName;
    public Kind kind;
    public GameObject plantPrefab;

    public object Copy()
    {
        return new PotionData
        {
            id = id,
            icon = icon,
            displayName = displayName,
            kind = kind,
            plantPrefab = plantPrefab
        };
    }
}