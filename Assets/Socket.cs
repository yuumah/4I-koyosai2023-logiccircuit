using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : Block
{
    [SerializeField]
    private GameObject[] SelectAble;
    private Block[] blocks;
    private int selected=0;
    private int inneroffset=0;
    void Awake()
    {
        this.Type=TYPE.SOCKET;
        blocks=new Block[SelectAble.Length];
        for(int i=0;i<SelectAble.Length;i++){
            if(SelectAble[i]!=null){
                blocks[i]=SelectAble[i].GetComponent<Block>();
                SelectAble[i].SetActive(false);
            }
        }
    }

    public void SwitchToNext(){
        setSelect(this.selected+1);
    }
    private void setSelect(int select){
        if(SelectAble[selected]!=null){
            SelectAble[selected].SetActive(false);
        }
        selected=select%SelectAble.Length;
        if(SelectAble[selected]!=null){
            SelectAble[selected].SetActive(true);
            Turn(0);
        }
    }

    public void Turn(int step=1){
        inneroffset=(inneroffset+step)%4;
        if(SelectAble[selected]!=null){
            SelectAble[selected].transform.rotation=Quaternion.Euler(0,0,-90*inneroffset);
            blocks[selected].setOffset(inneroffset);
            blocks[selected].Init();
        }
    }

    public override void setOffset(int offset)
    {
        setSelect(offset/4);
        Turn(offset%4);
    }

    public override void UpdateState(){
        if(SelectAble[selected]!=null){
            for(int i=0;i<4;i++){
                if(blocks[selected].GPIO[i].T==PINTYPE.INPUT){
                    blocks[selected].GPIO[i].Power=this.GPIO[i].Power;
                }
                else if(blocks[selected].GPIO[i].T==PINTYPE.OUTPUT){
                    this.GPIO[i].Power=blocks[selected].GPIO[i].Power;
                }
                else if(blocks[selected].GPIO[i].T==PINTYPE.PASSIVE){
                    blocks[selected].GPIO[i].Power=Mathf.Max(this.GPIO[i].Power-1,blocks[selected].GPIO[i].Power);
                    this.GPIO[i].Power=blocks[selected].GPIO[i].Power;
                }
            }
            blocks[selected].UpdateState();
            
        }
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
