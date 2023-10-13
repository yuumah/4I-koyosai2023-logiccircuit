using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Block[] Blocks;
    [SerializeField]
    private Block FallBack;
    // Start is called before the first frame update

    private int maxrow=0;
    private int column=0;
    
    [SerializeField]
    private Vector2Int currentposint;
    private Vector2 currentpos;
    
    private MainInput maininput;
    public GameObject Select;
    public GameObject[,] Board;
    public Camera MainCam;


    private Block findBlock(char Identity){
        for(int i=0;i<Blocks.Length;i++){
            if(Identity.Equals(Blocks[i].getIdentity())){
                return Blocks[i];
            }
        }
        return FallBack;
    }

    private void OnDisable()
    {
        maininput.Dispose();
    }
    void Start()
    {
        maininput=new MainInput();
        
        maininput.Enable();
        String txtmap="#*#*#\n*#*#*\n#*#*#\n*#*#*\n#*#*#####";
        String[] map=txtmap.Split("\n");
        column=map.Length;
        for(int i=0;i<column;i++){
            if(maxrow<map[i].Length){
                maxrow=map[i].Length;
            }
        }
        Board=new GameObject[column,maxrow];
        MainCam.transform.position=new Vector3(((float)maxrow-1)/2,((float)column-1)/2,MainCam.transform.position.z);
        MainCam.orthographicSize=((float)column)/2;

        for(int i=0;i<column;i++){
            for(int j=0;j<maxrow;j++){
                Vector3 pos=new Vector3(j,i)+this.transform.position;
                char id='$';
                if(j<map[column-i-1].Length){
                    id=map[column-i-1][j];
                }
                Block b=findBlock(id);
                GameObject gameObject=Instantiate(b.gameObject,pos,this.transform.rotation,this.transform);
                Board[i,j]=gameObject;
            }
        }

        currentpos=new Vector2(0,0);
        currentposint=new Vector2Int(0,0);
    }

    void Update()
    {
        Vector2 vec=maininput.Player.Move.ReadValue<Vector2>();
        float size=vec.x*vec.x+vec.y*vec.y;
        if(0<size){
            Move(vec);
        }
        else{
            currentpos=currentposint;
        }

        if(0.5<=maininput.Player.Select.ReadValue<float>()){
            Destroy(Board[currentposint.y,currentposint.x]);
        }
    }


    private void Move(Vector2 vec)
    {
        currentpos+=vec*0.01f;
        if(Math.Round(currentpos.x)<0){
            currentpos.x=0;
        }
        else if(maxrow<=Math.Round(currentpos.x)){
            currentpos.x=maxrow-1;
        }
        if(Math.Round(currentpos.y)<0){
            currentpos.y=0;
        }
        else if(column<=Math.Round(currentpos.y)){
            currentpos.y=column-1;
        }
        currentposint.x=(int)Math.Round(currentpos.x);
        currentposint.y=(int)Math.Round(currentpos.y);
        Select.transform.position=new Vector3(currentposint.x,currentposint.y,0);
    }

    public Vector2Int getCurrentPos(){
        return currentposint;
    }
}
