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
    [SerializeField] ParticleSystem _fx;

    private void Awake()
    {
        Disable();
    }

    public void Enable()
    {
        _fx.gameObject.SetActive(true);
    }

    public void Disable()
    {
        _fx.gameObject.SetActive(false);
    }
}
