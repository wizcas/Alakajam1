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

public class PlayerView : UIBehaviour 
{
    [SerializeField] Image _hpBar;

    protected override void Awake()
    {
        base.Awake();
        Messenger.AddListener<PlayerStatus>(Messages.PlayerStatusChanged, UpdatePlayerStatus);
    }

    void UpdatePlayerStatus(PlayerStatus status)
    {
        PrettyLog.Log("updating player status");
        _hpBar.fillAmount = status.CurrentHpRatio;
    }
}
