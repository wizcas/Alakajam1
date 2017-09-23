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

    private void Awake()
    {
        Messenger.AddListener<LabData>(Messages.UsePotion, UsePotion);
    }

    private void Start()
    {
        _status = GetComponent<PlayerStatus>();
        _groundOffset = transform.position - _groundCheck.position;
    }

    private void Update()
    {
        UpdatePosition();
        // todo: change keyboard mapping
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            AlchemyLab.Instance.ClearIngredients();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            AddIngredient(0);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            AddIngredient(1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            AddIngredient(2);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            AlchemyLab.Instance.UseCurrentPotion();
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

    void AddIngredient(int index)
    {
        var ingredientData = AlchemyLibrary.Instance.GetIngredientDataAt(index);
        if (ingredientData == null)
            return;
        AlchemyLab.Instance.AddIngredient(ingredientData);
    }

    void UsePotion(LabData data)
    {
        PlayCombineAnim(data.ingredients);
        if (data.outcomePotion == null)
        {
            //todo: fail / mysterious
            return;
        }
        // update stock

        var potion = Potion.Make(data.outcomePotion, transform.position);
        if (potion.data.isProjectile)
            TossPotion(potion);
        else
            DrinkPotion(potion);
    }

    void PlayCombineAnim(IEnumerable<IngredientData> datas)
    {
        var tween = DOTween.Sequence();
        float delay = .1f;
        foreach (var data in datas)
        {
            tween.AppendCallback(() =>
            {
                var ingredient = Ingredient.Make(data.id, transform.position);
                tween.Join(ingredient.Pop(.4f));
            });
            tween.AppendInterval(delay);
        }
    }

    void TossPotion(Potion potion)
    {
        potion.gameObject.SetActive(true);
        PrettyLog.LogEasy(potion.name, potion.gameObject.activeSelf);
        potion.Rigid.AddForce(tossDir.normalized * tossSpeed, ForceMode2D.Impulse);
    }

    void DrinkPotion(Potion potion)
    {
        _status.SetBuff(potion.data.id, true);
    }
}
