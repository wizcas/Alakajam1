/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public int dps = 5;
    public float speedModifier = -3f;
    public float damageInterval = 1;

    float _nextDamageTime = float.NaN;

    PlayerStatus _playerStatus;
    PlayerController _playerController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _nextDamageTime = Time.time;
            _playerController = collision.GetComponent<PlayerController>();
            _playerStatus = collision.GetComponent<PlayerStatus>();
            _playerController.speedModifier = speedModifier;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PrettyLog.Log("player leaves water");
            _nextDamageTime = float.NaN;
            _playerController.speedModifier -= speedModifier;
            _playerController = null;
            _playerStatus = null;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && Time.time >= _nextDamageTime)
        {
            PrettyLog.Log("player stays in water");

            _playerStatus.Damage(dps);
            _nextDamageTime = Time.time + damageInterval;
        }
    }
}
