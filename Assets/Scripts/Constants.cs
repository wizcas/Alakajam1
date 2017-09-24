/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tags
{
    public const string Player = "Player";
    public const string Potion = "Potion";
}

public enum Essence
{
    Gold,
    Wood,
    Water,
    Fire,
    Earth
}

public static class Messages
{
    public const string LabChanged = "LabChanged";
    public const string PlayerStatusChanged = "PlayerStatusChanged";
    public const string UsePotion = "UsePotion";
    public const string StockChanged = "StockChanged";
    public const string BuffUpdated = "BuffUpdated";
}