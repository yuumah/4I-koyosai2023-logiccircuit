using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : Block
{
    [SerializeField]
    private GameObject[] SelectAble;
    private Block[] blocks;
    [SerializeField]
    private int selected=0;
    private int inneroffset=0;
    void Start()
    {
        this.setOffset(0);
        this.Type=TYPE.SOCKET;
        blocks=new Block[SelectAble.Length];
        for(int i=0;i<SelectAble.Length;i++){
            if(SelectAble[i]!=null)blocks[i]=SelectAble[i].GetComponent<Block>();
        }
    }

    public void SwitchToNext(){
        if(SelectAble[selected]!=null){
            SelectAble[selected].SetActive(false);
        }

        selected=(selected+1)%SelectAble.Length;

        if(SelectAble[selected]!=null){
            SelectAble[selected].SetActive(true);
            blocks[selected].setOffset(inneroffset);
            blocks[selected].Init();
        }
    }

    public void Turn(){
        inneroffset=(inneroffset+1)%4;
        if(SelectAble[selected]!=null){
            Debug.Log("Turn:"+inneroffset.ToString());
            SelectAble[selected].transform.rotation=Quaternion.Euler(0,0,-90*inneroffset);
            blocks[selected].setOffset(inneroffset);
            blocks[selected].Init();
        }
    }

    private void Update(){
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
    }
}
