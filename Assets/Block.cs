using System;
using UnityEngine;

public class Block : MonoBehaviour
{
    public const int MAX_POWER=128;

    private const int GoalCount=512;
    private int goalcount=0;

    protected Animator ani;
    public enum PINTYPE{
        PASSIVE,
        INPUT,
        OUTPUT,
        NAN
    }
    public enum TYPE{
        NONE,
        OR,
        AND,
        WIRE,
        NOT,
        SOURCE,
        XOR,
        SOCKET,
        GOAL,
        CROSS
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
    protected char Identity;
    [SerializeField]
    protected TYPE Type;
    public TYPE getType(){
        return Type;
    }

    [SerializeField]
    public IO[] GPIO=new IO[4];
    public char getIdentity(){
        return this.Identity;
    }
    [SerializeField]
    protected int offset=0;
    public void setOffset(int offset){
        this.offset=offset;
    }

    private GameManager gameManager;

    void Awake()
    {
        gameManager=GameObject.Find("GameManager").GetComponent<GameManager>();
        if(Type!=TYPE.NONE){
            ani=this.GetComponent<Animator>();
        }
    }

    public void Init()
    {
        Awake();
        for(int i=0;i<4;i++){
            GPIO[i].Power=0;
        }
        if(Type==TYPE.OR){
            GPIO[(0+offset)%4].T=PINTYPE.OUTPUT;
            GPIO[(1+offset)%4].T=PINTYPE.INPUT;
            GPIO[(2+offset)%4].T=PINTYPE.NAN;
            GPIO[(3+offset)%4].T=PINTYPE.INPUT;
        }
        else if(Type==TYPE.NOT){
            GPIO[(0+offset)%4].T=PINTYPE.OUTPUT;
            GPIO[(1+offset)%4].T=PINTYPE.NAN;
            GPIO[(2+offset)%4].T=PINTYPE.INPUT;
            GPIO[(3+offset)%4].T=PINTYPE.NAN;
        }
        else if(Type==TYPE.WIRE){
            PINTYPE[] ps=new PINTYPE[4];
            for(int i=0;i<4;i++){
                ps[(i+offset)%4]=GPIO[i].T;
            }
            for(int i=0;i<4;i++){
                GPIO[i].T=ps[i];
            }
        }
        else if(Type==TYPE.AND){
            GPIO[(0+offset)%4].T=PINTYPE.OUTPUT;
            GPIO[(1+offset)%4].T=PINTYPE.INPUT;
            GPIO[(2+offset)%4].T=PINTYPE.NAN;
            GPIO[(3+offset)%4].T=PINTYPE.INPUT;
        }
        else if(Type==TYPE.XOR){
            GPIO[(0+offset)%4].T=PINTYPE.OUTPUT;
            GPIO[(1+offset)%4].T=PINTYPE.INPUT;
            GPIO[(2+offset)%4].T=PINTYPE.NAN;
            GPIO[(3+offset)%4].T=PINTYPE.INPUT;
        }
        else if(Type==TYPE.SOCKET){
            for(int i=0;i<4;i++){
                GPIO[i].T=PINTYPE.PASSIVE;
            }
        }
        else if(Type==TYPE.SOURCE){
            GPIO[(0+offset)%4].T=PINTYPE.OUTPUT;
            GPIO[(0+offset)%4].Power=MAX_POWER;
        }
        else if(Type==TYPE.GOAL){
            for(int i=0;i<4;i++){
                GPIO[i].T=PINTYPE.INPUT;
            }
        }
        //UpdateState();
    }

    public virtual void UpdateState()
    {
        if(Type==TYPE.OR){
            if(
                0<GPIO[(1+offset)%4].Power||
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
                if(GPIO[i].T==PINTYPE.PASSIVE){
                    GPIO[i].Power=p;
                }
            }
        }
        else if(Type==TYPE.CROSS){
            int p1=Math.Max(GPIO[0].Power,GPIO[2].Power);
            int p2=Math.Max(GPIO[1].Power,GPIO[3].Power);
            GPIO[0].Power=p1;
            GPIO[2].Power=p1;
            GPIO[1].Power=p2;
            GPIO[3].Power=p2;
        }
        else if(Type==TYPE.AND){
            if(
                0<GPIO[(1+offset)%4].Power&&
                0<GPIO[(3+offset)%4].Power
            ){
                GPIO[(0+offset)%4].Power=MAX_POWER;
            }
            else{
                GPIO[(0+offset)%4].Power=0;
            }
        }
        else if(Type==TYPE.XOR){
            if(
                0<GPIO[(1+offset)%4].Power!=
                0<GPIO[(3+offset)%4].Power
            ){
                GPIO[(0+offset)%4].Power=MAX_POWER;
            }
            else{
                GPIO[(0+offset)%4].Power=0;
            }
        }
        else if(Type==TYPE.GOAL){
            int p=0;
            for(int i=0;i<4;i++){
                p+=GPIO[i].Power;
            }
            if(0<p){
                goalcount++;
                if(GoalCount<goalcount){
                    Debug.Log("Goal");
                    gameManager.Goal();
                }
            }
            else{
                goalcount=0;
            }
        }

        if(Type!=TYPE.NONE){
            string[] para={"N","E","S","W"};
            for(int i=0;i<4;i++){
                if(0<GPIO[(i+offset)%4].Power){
                    ani.SetBool(para[i],true);
                }
                else{
                    ani.SetBool(para[i],false);
                }
            }
        }

    }
}
