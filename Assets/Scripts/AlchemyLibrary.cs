/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AlchemyLibrary", menuName = "Alchemy Library")]
public class AlchemyLibrary : ScriptableObject
{
    const string ResourcePath = "AlchemyLibrary";
    public static AlchemyLibrary _instance;
    public static AlchemyLibrary Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<AlchemyLibrary>(ResourcePath);
                _instance.BuildRuntimeData();
            }
            return _instance;
        }
    }

    [SerializeField] IngredientData[] ingredients;
    [SerializeField] Formular[] formulars;
    [SerializeField] PotionData[] potions;

    void BuildRuntimeData()
    {
        runtimeIngredients = CopyArray(ingredients);
        runtimePotions = CopyArray(potions);
    }

    T[] CopyArray<T>(T[] src) where T : IRuntimeData
    {
        var ret = new T[src.Length];
        for (int i = 0; i < ret.Length; i++)
        {
            ret[i] = (T)src[i].Copy();
        }
        return ret;
    }

    IngredientData[] runtimeIngredients;
    PotionData[] runtimePotions;

    #region RUNTIME APIs
    public IEnumerable<IngredientData> Ingredients
    {
        get { return runtimeIngredients; }
    }

    public IngredientData GetIngredientDataAt(int index)
    {
        if (runtimeIngredients == null || index < 0 || index >= runtimeIngredients.Length)
        {
            return null;
        }
        return runtimeIngredients[index];
    }
    public IngredientData FindIngredient(string id)
    {
        return runtimeIngredients.FirstOrDefault(data => data.id == id);
    }
    public PotionData FindPotion(string id)
    {
        return runtimePotions.FirstOrDefault(data => data.id == id);
    }
    public IEnumerable<Formular> Formulars
    {
        get { return formulars; }
    }
    #endregion
}

interface IRuntimeData
{
    object Copy();   
}