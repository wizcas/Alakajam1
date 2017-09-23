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
    [SerializeField] Transform _footPos;

    [SerializeField] Potion _effectivePotion;
    [SerializeField] Potion _ineffectivePotion;

    PlayerStatus _status;
    bool _isGrounded;
    Vector2 _footOffset;

    private void Awake()
    {
        Messenger.AddListener<LabData>(Messages.UsePotion, UsePotion);
    }

    private void Start()
    {
        _status = GetComponent<PlayerStatus>();
        _footOffset = transform.position - _footPos.position;
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
        var selfPos = transform.position;
        var groundCheckYOffset = 3f;
        // ground check
        var rayStartPos = selfPos + Vector3.up * groundCheckYOffset;
        Debug.DrawRay(rayStartPos, Vector2.down, Color.blue, 1f);
        var hit = Physics2D.Raycast(rayStartPos, Vector2.down, 5f, LayerMask.GetMask("Ground"));
        _isGrounded = hit.transform != null;
        float y;
        if (_isGrounded)
        {
            // place on the ground
            Debug.DrawLine(hit.point, hit.point + Vector2.left * 2f, Color.red, 1f);
            y = hit.point.y + _footOffset.y;
        }
        else
        {
            // fall by gravity
            y = selfPos.y - gravity;
        }
        transform.position = new Vector3(selfPos.x, y, selfPos.z);
        selfPos = transform.position;
        float x = selfPos.x;
        if (!_status.IsDead)
        {
            x += speed;
        }
        var nextPos = new Vector3(x, y, selfPos.z);
        transform.position = Vector3.Lerp(selfPos, nextPos, Time.deltaTime);
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
