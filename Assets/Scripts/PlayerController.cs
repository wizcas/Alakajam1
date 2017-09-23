/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerStatus))]
public class PlayerController : MonoBehaviour 
{
    public float speed = 3;
    public Vector2 tossDir = new Vector2(1, .5f);
    public float tossSpeed = 8f;
    public float gravity = 9.8f;
    [SerializeField] Transform _groundCheck;

    [SerializeField] Potion _effectivePotion;
    [SerializeField] Potion _ineffectivePotion;

    PlayerStatus _status;
    bool _isGrounded;
    Vector2 _groundOffset;

    private void Start()
    {
        _status = GetComponent<PlayerStatus>();
        _groundOffset = transform.position - _groundCheck.position;
    }

    private void Update()
    {
        UpdatePosition();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TossPotion(_effectivePotion);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            TossPotion(_ineffectivePotion);
        }
    }

    void UpdatePosition()
    {
        // ground check
        var ground = Physics2D.OverlapPoint(_groundCheck.position, LayerMask.GetMask("Ground"));
        _isGrounded = ground != null;
        float y;
        if (_isGrounded)
        {
            // place on the ground
            y = _groundCheck.position.y + _groundOffset.y;
        }
        else
        {
            // fall by gravity
            y = transform.position.y - gravity;
        }
        float x = transform.position.x;
        if (!_status.IsDead)
        {
            x += speed;
        }
        var nextPos = new Vector3(x, y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, nextPos, Time.deltaTime);
    }

    void UsePotion(Potion potionPrefab)
    {
        if (potionPrefab.isProjectile)
            TossPotion(potionPrefab);
        else
            DrinkPotion(potionPrefab);
    }

    void TossPotion(Potion potionPrefab)
    {
        var potion = Instantiate(potionPrefab);
        potion.transform.position = transform.position;
        potion.gameObject.SetActive(true);
        PrettyLog.LogEasy(potion.name, potion.gameObject.activeSelf);
        potion.Rigid.AddForce(tossDir.normalized * tossSpeed, ForceMode2D.Impulse);
    }

    void DrinkPotion(Potion potionPrefab)
    {
        //todo: potion takes effect
    }
}
