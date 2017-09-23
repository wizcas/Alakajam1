/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using Cheers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[PrettyLog.Provider("Player", "status", "green", "magenta")]
public class PlayerStatus : MonoBehaviour 
{
    public int hp = 100;

    [ReadOnly, SerializeField]
    int _currentHp;

    public bool IsDead
    {
        get
        {
            return _currentHp <= 0;
        }
    }

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        _currentHp = hp;
    }

    public void Damage(int dmg)
    {
        _currentHp = Mathf.Max(0, _currentHp - dmg);
        if (_currentHp <= 0) Die();
    }

    void Die()
    {
        PrettyLog.Log("You are dead");
    }
}
