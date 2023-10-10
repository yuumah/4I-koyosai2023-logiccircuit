using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField]
    private char Identity;
    
    public char getIdentity(){
        return this.Identity;
    }
}
