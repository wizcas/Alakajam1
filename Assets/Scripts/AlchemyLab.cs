/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AlchemyLab : MonoBehaviour 
{
    public Formular[] formulars;

    /// <summary>
    /// Combine ingredients into a potion
    /// </summary>
    /// <param name="ingredientIds">ingredient IDs</param>
    /// <returns>potion ID. If combination fails, return NULL</returns>
    public string Combine(params string[] ingredientIds)
    {
        var f = FindFormular(ingredientIds);
        if (Formular.IsValid(f))
            return f.potionId;
        return null;
    }

    Formular FindFormular(string[] ingredientIds)
    {
        if (formulars == null)
            return Formular.Invalid;

        return formulars.FirstOrDefault(f => f.Match(ingredientIds));
    }
}

[Serializable] 
public struct Formular
{
    public static Formular Invalid = new Formular
    {
        ingredientIds = null,
        potionId = null
    };
    public static bool IsValid(Formular f)
    {
        return f.ingredientIds != null && f.potionId != null;
    }

    public string[] ingredientIds;
    public string potionId;

    public bool Match(string[] ids)
    {
        if (ids == null) return false;
        // ingredients passed in is more than formular needs
        if (ids.Length != ingredientIds.Length)
            return false;
        // Sort IDs for same sequence
        Array.Sort(ids);
        Array.Sort(ingredientIds);
        var unmatched = new List<string>(ingredientIds);
        for (int i = 0; i < ids.Length; i++)
        {
            var id = ids[i];
            var expected = ingredientIds[i];
            if (id.ToLower() == expected.ToLower())
            {
                unmatched.Remove(expected);
            }
        }
        return unmatched.Count == 0;
    }
}