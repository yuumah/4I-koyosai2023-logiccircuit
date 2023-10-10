using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Block[] Blocks;
    [SerializeField]
    private Block FallBack;
    // Start is called before the first frame update

    private int maxrow=0;
    private int column=0;

    private Block findBlock(char Identity){
        for(int i=0;i<Blocks.Length;i++){
            if(Identity.Equals(Blocks[i].getIdentity())){
                return Blocks[i];
            }
        }
        return FallBack;
    }
    void Start()
    {
        String txtmap="#*#####***#*#*#**#*#**#**#*#*#*#*#*#**#**#*#**#*#*#\n*##\n##a";
        String[] map=txtmap.Split("\n");
        column=map.Length;
        for(int i=0;i<column;i++){
            if(maxrow<map[i].Length){
                maxrow=map[i].Length;
            }
        }

        for(int i=0;i<column;i++){
            for(int j=0;j<maxrow;j++){
                Vector3 pos=new Vector3(j,-i)+this.transform.position;
                char id='$';
                if(j<map[i].Length){
                    id=map[i][j];
                }
                Block b=findBlock(id);
                Instantiate(b.gameObject,pos,this.transform.rotation,this.transform);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
