using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum CardColor{Red, Purple, Green};
public enum CardPattern{Empty, Filled, Striped};
public enum CardShape{Oval, Rhombus, Squiggle};

public class Card : MonoBehaviour
{
    public CardColor color;
    public CardPattern pattern;
    public CardShape shape;
    public int num;
    public int position;

    public Card(){
        color = CardColor.Red;
        pattern = CardPattern.Empty;
        shape = CardShape.Oval;
        num = 0;
        position = 0;
    }

    public bool equals(Card ob){
        if (color == ob.color && pattern == ob.pattern && shape == ob.shape && num == ob.num){
            return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
