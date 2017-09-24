/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
    public Transform target;

    Vector3 _offset2Target;
    private void Start()
    {
        _offset2Target = transform.position - target.position;
    }

    private void Update()
    {
        if (target != null)
        {
            transform.position = target.position + _offset2Target;
        }
    }
}
