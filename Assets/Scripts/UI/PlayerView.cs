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
    [SerializeField] Transform _buffRoot;
    [SerializeField] BuffItem _buffItemPrefab;
    Dictionary<string, BuffItem> _buffItemMap = new Dictionary<string, BuffItem>();

    protected override void Awake()
    {
        base.Awake();
        Messenger.AddListener<PlayerStatus>(Messages.PlayerStatusChanged, UpdatePlayerStatus);
        Messenger.AddListener<Buff>(Messages.BuffUpdated, UpdateBuff);
    }

    void UpdatePlayerStatus(PlayerStatus status)
    {
        PrettyLog.Log("updating player status");
        _hpBar.fillAmount = status.CurrentHpRatio;
    }

    void UpdateBuff(Buff buff)
    {
        BuffItem item;
        if(!_buffItemMap.TryGetValue(buff.potionId, out item))
        {
            item = Instantiate(_buffItemPrefab);
            item.transform.SetParent(_buffRoot, false);
            _buffItemMap[buff.potionId] = item;
        }
        if (buff.remainingSeconds >= 0)
        {
            item.UpdateData(buff);
        }
        else
        {
            Destroy(item.gameObject);
            _buffItemMap.Remove(buff.potionId);
        }
    }
}
