/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public string potionId;
    public float duration = 5;
    [SerializeField] ParticleSystem _fx;

    Coroutine _activeCoroutine;
    float _remainingSecs;
    public float remainingSeconds {
    get { return _remainingSecs; }
    set
        {
            _remainingSecs = value;
            Messenger.Broadcast(Messages.BuffUpdated, this);
        }
    }
    public float remaingRatio
    {
        get { return _remainingSecs / duration; }
    }

    private void Awake()
    {
        Disable();
    }

    public void Enable()
    {
        _fx.gameObject.SetActive(true);
        KillCoroutine();
        _activeCoroutine = StartCoroutine(WaitForDisable());
    }

    public void Disable()
    {
        KillCoroutine();
        remainingSeconds = -1;        
        _fx.gameObject.SetActive(false);
    }

    IEnumerator WaitForDisable()
    {
        remainingSeconds = duration;
        var oneSec = new WaitForSeconds(1);
        while(remainingSeconds > 0)
        {
            yield return oneSec;
            remainingSeconds--;
        }
        Disable();
    }

    void KillCoroutine()
    {
        if (_activeCoroutine == null)
            return;
        StopCoroutine(_activeCoroutine);
        _activeCoroutine = null;
    }
}
