/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour 
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var tag = collision.tag;
        PrettyLog.Log("{0} collides with {1}({2})", name, collision.name, tag);
        switch (tag)
        {            
            case Tags.Potion:
                var potion = collision.GetComponent<Potion>();
                if (potion != null)
                {
                    potion.Break();
                }
                break;
        }
    }
}
