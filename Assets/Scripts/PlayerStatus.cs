/*****************************************************
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

[PrettyLog.Provider("Player", "status", "green", "magenta")]
public class PlayerStatus : MonoBehaviour
{

    public int hp = 100;
    public Buff[] buffs;

    [ReadOnly, SerializeField]
    int _currentHp;
    [ReadOnly, SerializeField]
    Buff _activeBuff;

    public int CurrentHp
    {
        get
        {
            return _currentHp;
        }
        set
        {
            bool changed = _currentHp != value;
            _currentHp = value;
            if (changed)
            {
                Messenger.Broadcast(Messages.PlayerStatusChanged, this);
            }
        }
    }

    public float CurrentHpRatio
    {
        get { return (float)CurrentHp / hp; }
    }

    public bool IsDead
    {
        get
        {
            return CurrentHp <= 0;
        }
    }

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        CurrentHp = hp;
    }

    public void Damage(int dmg)
    {
        CurrentHp = Mathf.Max(0, CurrentHp - dmg);
        if (CurrentHp <= 0) Die();
    }

    void Die()
    {
        PrettyLog.Log("You are dead");
    }

    public void SetBuff(string potionId, bool isOn)
    {
        if (!isOn)
        {
            if (_activeBuff != null && _activeBuff.potionId.ToLower() == potionId.ToLower())
                _activeBuff.Disable();
            return;
        }

        var buff = buffs.FirstOrDefault(b => b.potionId == potionId);
        if (buff == null)
        {
            return;
        }
        if (_activeBuff != null)
        {
            _activeBuff.Disable();
        }
        _activeBuff = buff;
        _activeBuff.Enable();

    }
}