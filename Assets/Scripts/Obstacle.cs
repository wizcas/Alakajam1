﻿/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using Cheers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[PrettyLog.Provider("Obstacle", "", "red", "")]
public class Obstacle : MonoBehaviour 
{
    public string[] effectivePotionIds;
    [SerializeField] protected int damage = 10;

    void Awake()
    {
        Reset();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var tag = collision.tag;
        PrettyLog.Log("{0} collides with {1}({2})", name, collision.name, tag);
        switch (tag)
        {
            case Tags.Player:
                Eliminate();
                var player = collision.GetComponent<PlayerStatus>();
                player.Damage(damage);
                break;
            case Tags.Potion:
                var potion = collision.GetComponent<Potion>();
                if (IsPotionEffective(potion))
                {
                    Eliminate();
                }
                potion.Break();
                break;
        }
    }

    bool IsPotionEffective(Potion potion)
    {
        if(potion == null)
        {
            PrettyLog.Error("{0} collides with a null potion", name);
            return false;
        }
        if (effectivePotionIds == null || effectivePotionIds.Length == 0) return false;
        return effectivePotionIds.Contains(potion.id);
    }

    [ClickMe("Reset")]
    protected virtual void Reset()
    {
        gameObject.SetActive(true);
    }

    [ClickMe("Eliminate")]
    protected virtual void Eliminate()
    {
        PrettyLog.Log("{0} is destroyed", name);
        gameObject.SetActive(false);
    }
}
