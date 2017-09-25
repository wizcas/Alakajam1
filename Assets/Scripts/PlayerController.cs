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
    public Vector2 plantDir = new Vector2(1, 1f);
    public float plantSpeed = 4f;
    public float gravity = 9.8f;
    [SerializeField] Transform _footPos;
    [SerializeField] float _footMarginY = .05f;
    [SerializeField] Transform _tossPos;
    [SerializeField] Transform _drinkPos;
    [SerializeField] Transform _endingPoint;
    [SerializeField] Transform _stopPoint;

    [SerializeField] Animator _anim;
    [SerializeField] GameObject _menu;
    [SerializeField] GameObject _deadNote;
    [SerializeField] GameObject _resumeTip;

    PlayerStatus _status;
    bool _isGrounded;
    Vector2 _footOffset;
    bool _isLevelCleared;
    bool _isGameOver;
    bool _isMenuShown;
    bool IsMenuShown
    {
        get
        {
            return _isMenuShown;
        }
        set
        {
            _isMenuShown = value;
            Time.timeScale = _isMenuShown ? 0 : 1;
            _menu.SetActive(_isMenuShown);
        }
    }

    float Speed
    {
        get { return speed + speedModifier; }
    }

    private void Awake()
    {
        Messenger.AddListener<LabData>(Messages.UsePotion, UsePotion);
        _status = GetComponent<PlayerStatus>();
        IsMenuShown = false;
    }

    private void Start()
    {
        _footOffset = transform.position - _footPos.position;
        _deadNote.gameObject.SetActive(false);
        _resumeTip.gameObject.SetActive(true);
    }

    IEnumerator ShowMenuWhenDead(){
        yield return new WaitForSeconds(1.5f);
        IsMenuShown = true;
    }

    private void Update()
    {
        if (_status.IsDead && !_isGameOver)
        {   
            StartCoroutine(ShowMenuWhenDead());
            _deadNote.gameObject.SetActive(true);
            _resumeTip.gameObject.SetActive(false);
            _isGameOver = true;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsMenuShown = !IsMenuShown;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            AlchemyLab.Instance.ClearIngredients();
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            AddIngredient(0);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            AddIngredient(1);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            AddIngredient(2);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            AddIngredient(3);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            AddIngredient(4);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            AlchemyLab.Instance.UseCurrentPotion();
        }

        if (transform.position.x >= _endingPoint.position.x)
        {
            Camera.main.GetComponent<CameraController>().target = null;
        }
        if (transform.position.x >= _stopPoint.position.x)
        {
            _anim.SetBool("Stopped", true);
            speedModifier = -speed;
        }
    }

    private void FixedUpdate()
    {
        if (_status.IsDead) return;
        UpdatePosition();
    }

    void UpdatePosition()
    {
        var selfPos = transform.position;
        var groundCheckYOffset = .5f;
        // ground check
        var rayStartPos = selfPos + Vector3.up * groundCheckYOffset;
        var hit = Physics2D.Raycast(rayStartPos, Vector2.down, 1f, LayerMask.GetMask("Ground"));
        Vector2 slopeDir = Vector2.zero;
        if (hit.transform == null)
        {
            PrettyLog.Log("no hit");
            _isGrounded = false;
        }
        else
        {
            //Debug.DrawRay(hit.point, Vector3.left, Color.blue);
            slopeDir = Quaternion.Euler(0, 0, -90) * hit.normal;
            //Debug.DrawRay(selfPos, slopeDir, Color.yellow);
            // if the slope is super steep, Unity returns a normal of value (0,1). 
            // In such case, we're going to ignore the value
            if (hit.normal.y == 1 && hit.transform.rotation != Quaternion.identity)
            {
                slopeDir = Vector2.zero;
            }
            _isGrounded = Mathf.Abs(_footPos.position.y - hit.point.y) < _footMarginY || _footPos.position.y < hit.point.y;
        }
        float yDelta = -gravity;
        float xDelta = Speed;
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
        //Debug.DrawRay(selfPos, moveDir, Color.red);
        var mag = vec.magnitude;
        if (slopeDir != Vector2.zero && _isGrounded)
        {
            // slide along the slope and keep speed, if it's holding the player
            var slopeAngle = Vector2.SignedAngle(Vector2.right, slopeDir);
            var forwardAngle = Vector2.SignedAngle(Vector2.right, moveDir);
            if (forwardAngle < slopeAngle)
            {
                moveDir = slopeDir;
            }
        }
        //Debug.DrawRay(selfPos, moveDir, Color.magenta);

        var nextPos = selfPos + moveDir * (_isGrounded ? Speed : mag);
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
                TossPotion(potion, false);
                break;
            case PotionData.Kind.Plant:
                TossPotion(potion, true);
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
                var ingredient = Ingredient.Make(data.id, _drinkPos.position);
                ingredient.transform.localScale = Vector3.one * .5f;
                tween.Join(ingredient.Pop(.4f));
            });
            tween.AppendInterval(delay);
        }
    }

    void TossPotion(Potion potion, bool isPlant)
    {
        PlayCast();
        potion.transform.position = _tossPos.position;
        potion.gameObject.SetActive(true);
        PrettyLog.LogEasy(potion.name, potion.gameObject.activeSelf);
        var dir = (isPlant ? plantDir : tossDir).normalized;
        var speed = isPlant ? plantSpeed : tossSpeed;
        potion.Rigid.AddForce(dir * speed, ForceMode2D.Impulse);
    }

    void DrinkPotion(Potion potion)
    {
        _status.SetBuff(potion.data.id, true);
    }

    public void PlayHit()
    {
        if (_status.HasBuff("shield"))
        {
            return;
        }
        _anim.SetTrigger("Hit");
    }

    public void PlayDie()
    {
        _anim.SetTrigger("Die");
    }

    public void PlayCast()
    {
        _anim.SetTrigger("Cast");
    }
}
