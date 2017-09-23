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

public class AlchemyLab : Singleton<AlchemyLab>
{
    public int slotCount = 3;
    public List<IngredientData> addedIngredients;
    public PotionData currentPotion;

    IEnumerable<Formular> Formulars
    {
        get
        {
            return AlchemyLibrary.Instance.Formulars;
        }
    }

    private void Awake()
    {
        addedIngredients = new List<IngredientData>(slotCount);
    }

    public void ClearIngredients()
    {
        addedIngredients.Clear();
        currentPotion = null;
        NotifyLabDataChanged();
    }

    public void AddIngredient(IngredientData ingredient)
    {
        if (addedIngredients.Count >= slotCount) return;
        addedIngredients.Add(ingredient);
        currentPotion = TryCombination(Formulars, addedIngredients.Select(i => i.id).ToArray());
        NotifyLabDataChanged();
    }

    public void UseCurrentPotion()
    {
        foreach (var ingredient in addedIngredients)
        {
            ingredient.Use();
        }
        Messenger.Broadcast(Messages.UsePotion, MakeLabData());
        ClearIngredients();
    }

    /// <summary>
    /// Combine ingredients into a potion
    /// </summary>
    /// <param name="ingredientIds">ingredient IDs</param>
    /// <returns>potion ID. If combination fails, return NULL</returns>
    public PotionData TryCombination(IEnumerable<Formular> formulars, params string[] ingredientIds)
    {
        var f = FindFormular(ingredientIds, formulars);
        if (Formular.IsValid(f))
        {
            var pid = f.potionId;
            return AlchemyLibrary.Instance.FindPotion(pid);
        }
        return null;
    }

    public Formular FindFormular(string[] ingredientIds, IEnumerable<Formular> formulars)
    {
        if (formulars == null)
            return Formular.Invalid;

        return formulars.FirstOrDefault(f => f.Match(ingredientIds));
    }

    void NotifyLabDataChanged()
    {
        Messenger.Broadcast(Messages.LabChanged, MakeLabData());
    }

    LabData MakeLabData()
    {
        return new LabData
        {
            ingredients = addedIngredients,
            outcomePotion = currentPotion
        };
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

public struct LabData
{
    public IEnumerable<IngredientData> ingredients;
    public PotionData outcomePotion;
}