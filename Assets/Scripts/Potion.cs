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
    public Sprite sprite;
    public string id;
    public bool isProjectile = true;

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

    public void Break()
    {
        Destroy(gameObject);
    }
}
