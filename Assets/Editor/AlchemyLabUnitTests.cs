/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyLabUnitTests
{
    AlchemyLab lab;
    IEnumerable<Formular> formulars;
    [SetUp]
    public void Setup()
    {
        lab = AlchemyLab.Instance;
        formulars= new[]
        {
            new Formular
            {
                ingredientIds = new []{"a", "b", "c"},
                potionId = "pABC"
            },
            new Formular
            {
                ingredientIds = new []{"d", "e", "f"},
                potionId = "pDEF"
            },
            new Formular
            {
                ingredientIds = new []{"a", "e", "d"},
                potionId = "pADE"
            }
        };
    }

	[Test]
    public void TestFormularMatch()
    {
        var f = new Formular
        {
            ingredientIds = new[]
            {
                "aaa", "Bbb", "CcC"
            }
        };
        Assert.True(f.Match(new[] { "AAA", "bbb", "ccc" }));
        Assert.True(f.Match(new[] { "ccc", "aaa", "bbB" }));
        Assert.False(f.Match(new string[0]));
        Assert.False(f.Match(null));
        Assert.False(f.Match(new[] { "a", "b", "c", "d" }));
        Assert.False(f.Match(new[] { "aaa", "bbb", "ccc", "ddd" }));
        Assert.False(f.Match(new[] { "x" }));

        f = new Formular
        {
            ingredientIds = new[]
            {
                "aAa", "Bbb", "CcC", "aaa", "AAA"
            }
        };
        Assert.True(f.Match(new[] { "AAA", "bbb", "aaa", "ccc", "aaA" }));
        Assert.False (f.Match(new[] { "AAA", "bbb", "aaa", "ccc", "ddd" }));
    }
    
    [Test]
    public void TestCombine()
    {
        //todo refactor
        //Assert.AreEqual("pABC", lab.TryCombination(formulars, "a", "c", "B"));
        //Assert.AreEqual("pDEF", lab.TryCombination(formulars, "f", "d", "e"));
        //Assert.Null(lab.TryCombination(formulars, null));
        //Assert.Null(lab.TryCombination(formulars, "x"));
        //Assert.Null(lab.TryCombination(formulars, "pADE", "pABC", "pDEF", "p", "p"));
    }
}
