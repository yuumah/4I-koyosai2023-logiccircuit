using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public const int MAX_POWER=256;

    private Animator ani;
    public enum PINTYPE{
        PASSIVE,
        INPUT,
        OUTPUT,
        NAN
    }
    public enum TYPE{
        OR,
        AND,
        WIRE,
        NOT,
        SOURCE
    }

    [System.Serializable]
    public class IO{
        //電圧がかかっているか
        //最大値は64
        
        [SerializeField]
        public int Power=0;

        //入出力方向
        [SerializeField]
        internal PINTYPE T=PINTYPE.PASSIVE;
        public PINTYPE getPintype(){
            return T;
        }
    }
    [SerializeField]
    private char Identity;
    [SerializeField]
    private TYPE Type;
    public TYPE getType(){
        return Type;
    }

    [SerializeField]
    public IO[] GPIO=new IO[4];
    public char getIdentity(){
        return this.Identity;
    }

    [SerializeField]
    private int offset=0;
    public int getOffset(){
        return offset;
    }

    void Start()
    {
        if(Type==TYPE.OR){
            GPIO[(0+offset)%4].T=PINTYPE.OUTPUT;
            GPIO[(1+offset)%4].T=PINTYPE.INPUT;
            GPIO[(2+offset)%4].T=PINTYPE.INPUT;
            GPIO[(3+offset)%4].T=PINTYPE.INPUT;
        }
        else if(Type==TYPE.NOT){
            GPIO[(0+offset)%4].T=PINTYPE.OUTPUT;
            GPIO[(1+offset)%4].T=PINTYPE.NAN;
            GPIO[(2+offset)%4].T=PINTYPE.INPUT;
            GPIO[(3+offset)%4].T=PINTYPE.NAN;
        }
        else if(Type==TYPE.WIRE){
            ani=this.GetComponent<Animator>();
        }
    }

    public void UpdateState()
    {
        if(Type==TYPE.OR){
            if(
                0<GPIO[(1+offset)%4].Power||
                0<GPIO[(2+offset)%4].Power||
                0<GPIO[(3+offset)%4].Power
            ){
                GPIO[(0+offset)%4].Power=MAX_POWER;
            }
            else{
                GPIO[(0+offset)%4].Power=0;
            }
        }
        else if(Type==TYPE.NOT){
            if(0<GPIO[(2+offset)%4].Power){
                GPIO[(0+offset)%4].Power=0;
            }
            else{
                GPIO[(0+offset)%4].Power=MAX_POWER;
            }
        }
        else if(Type==TYPE.WIRE){
            int p=0;
            for(int i=0;i<4;i++){
                p=Math.Max(p,GPIO[i].Power);
            }
            for(int i=0;i<4;i++){
                GPIO[i].Power=p;
            }
            if(0<p){
                ani.SetBool("ON",true);
            }
            else{
                ani.SetBool("ON",false);
            }
        }

    }
}
