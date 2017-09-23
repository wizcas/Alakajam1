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
    public float speedModifier = 0;
    public Vector2 tossDir = new Vector2(1, .5f);
    public float tossSpeed = 8f;
    public float gravity = 9.8f;
    [SerializeField] Transform _footPos;

    [SerializeField] Potion _effectivePotion;
    [SerializeField] Potion _ineffectivePotion;

    PlayerStatus _status;
    bool _isGrounded;
    Vector2 _footOffset;

    float Speed
    {
        get { return speed + speedModifier; }
    }

    private void Awake()
    {
        Messenger.AddListener<LabData>(Messages.UsePotion, UsePotion);
    }

    private void Start()
    {
        _status = GetComponent<PlayerStatus>();
        _footOffset = transform.position - _footPos.position;
        //Debug.Break();
    }

    private void Update()
    {
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

    private void FixedUpdate()
    {
        UpdatePosition();        
    }

    void UpdatePosition()
    {
        var selfPos = transform.position;
        var groundCheckYOffset = 3f;
        // ground check
        var rayStartPos = selfPos + Vector3.up * groundCheckYOffset;
        Debug.DrawRay(rayStartPos, Vector2.down, Color.blue, 1f);
        var hit = Physics2D.Raycast(rayStartPos, Vector2.down, 5f, LayerMask.GetMask("Ground"));
        if (hit.transform == null)
        {
            _isGrounded = false;
        }
        else
        {
            _isGrounded = Mathf.Approximately(_footPos.position.y, hit.point.y) || _footPos.position.y < hit.point.y;
        }
        PrettyLog.LogEasy("is on ground?", _isGrounded);
        float yDelta = -1;
        if (_isGrounded)
        {
            // place on the ground
            Debug.DrawLine(hit.point, hit.point + Vector2.left * 2f, Color.red, 1f);
            yDelta = hit.point.y + _footOffset.y - selfPos.y;
            PrettyLog.Log("hit: {0}, offset: {1}, y: {2}", hit.point.y, _footOffset.y, yDelta);
        }
        float xDelta = 1;
        if (_status.IsDead)
        {
            xDelta = 0;
        }
        var dir = new Vector2(xDelta, yDelta).normalized;
        var nextPos = selfPos + new Vector3(dir.x * speed, dir.y * gravity, selfPos.z);
        
        transform.position = Vector3.Lerp(selfPos, nextPos, Time.fixedDeltaTime);
        PrettyLog.Log("frame end: {0}", transform.position.y);

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
        var potion = Potion.Make(data.outcomePotion, transform.position);
        switch (potion.data.kind)
        {
            case PotionData.Kind.Toss:
            case PotionData.Kind.Plant:
                TossPotion(potion);
                break;
            case PotionData.Kind.Drink:
                DrinkPotion(potion);
                break;
        }
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
