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
    [SerializeField] float _footMarginY = .05f;

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
        var groundCheckYOffset = 0f;
        // ground check
        var rayStartPos = selfPos + Vector3.up * groundCheckYOffset;
        var hit = Physics2D.Raycast(rayStartPos, Vector2.down, 2f, LayerMask.GetMask("Ground"));
        Vector2 slopeDir = Vector2.zero;
        if (hit.transform == null)
        {
            _isGrounded = false;
        }
        else
        {
            slopeDir = Quaternion.Euler(0, 0, -90) * hit.normal;
            // if the slope is super steep, Unity returns a normal of value (0,1). 
            // In such case, we're going to ignore the value
            if (Mathf.Abs(slopeDir.y) < .0005)
            {
                slopeDir = Vector2.zero;
            }
            _isGrounded = Mathf.Abs(_footPos.position.y - hit.point.y) < _footMarginY || _footPos.position.y < hit.point.y;
        }
        float yDelta = -gravity;
        float xDelta = speed;
        if (_status.IsDead)
        {
            xDelta = 0;
            yDelta = 0;
        }
        // determine the move velocity
        var vec = new Vector3(xDelta, yDelta, 0f);
        var moveDir = vec.normalized;
        if (moveDir == Vector3.zero)
            return;
        
        var mag = vec.magnitude;
        if (slopeDir != Vector2.zero && _isGrounded)
        {
            // slide along the slope and keep speed, if it's holding the player
            var slopeAngle = Vector2.SignedAngle(Vector2.right, slopeDir);
            var forwardAngle = Vector2.SignedAngle(Vector2.right, moveDir);
            if(forwardAngle < slopeAngle)
            {
                moveDir = slopeDir;
            }
        }
        
        var nextPos = selfPos + moveDir * mag;
        transform.position = Vector3.Lerp(selfPos, nextPos, Time.fixedDeltaTime);
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
