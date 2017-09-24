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
[RequireComponent(typeof(PlayerController))]
public class PlayerStatus : MonoBehaviour
{

    public int hp = 100;
    public Buff[] buffs;

    [SerializeField] ParticleSystem _dustFx;
    [SerializeField] ParticleSystem _bubbleFx;

    [ReadOnly, SerializeField]
    int _currentHp;
    [ReadOnly, SerializeField]
    List<Buff> _activeBuffs = new List<Buff>();

    PlayerController _controller;

    public bool IsInWater
    {
        set
        {
            _dustFx.gameObject.SetActive(!value);
            _bubbleFx.gameObject.SetActive(value);
        }
    }

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

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
        Messenger.AddListener<Buff>(Messages.BuffUpdated, OnBuffUpdated);
    }

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        CurrentHp = hp;
        IsInWater = false;
        _dustFx.gameObject.SetActive(true);
    }

    public void Damage(int dmg, DamageType dmgType)
    {
        if (dmgType == DamageType.Normal && HasBuff("shield"))
            return;
        if (dmgType == DamageType.Drown && HasBuff(("waterproof")))
            return;

        CurrentHp = Mathf.Max(0, CurrentHp - dmg);
        if (CurrentHp <= 0) Die();
    }

    void Die()
    {
        _controller.PlayDie();
        _dustFx.gameObject.SetActive(false);
    }

    public void SetBuff(string potionId, bool isOn)
    {
        Buff buff = _activeBuffs.FirstOrDefault(b => b.potionId.ToLower() == potionId.ToLower());
        if (!isOn)
        {
            if (buff != null)
            {
                buff.Disable();
                _activeBuffs.Remove(buff);
            }
            return;
        }

        bool newbuff = false;
        if (buff == null) {
            buff = buffs.FirstOrDefault(b => b.potionId.ToLower() == potionId.ToLower());
            newbuff = true;
        }
        if (buff == null)
        {
            return;
        }
        if (newbuff)
        {
            _activeBuffs.Add(buff);
        }
        buff.Enable();
    }

    public bool HasBuff(string potionId)
    {
        return _activeBuffs.Any(b => b.potionId.ToLower() == potionId.ToLower());
    }

    void OnBuffUpdated(Buff buff)
    {
        if (buff.remainingSeconds < 0)
            _activeBuffs.Remove(buff);
    }
}

public enum DamageType
{
    Normal,
    Drown,
    InstantDeath
}