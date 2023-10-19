using System;
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

    [SerializeField]
    private InputActionReference _press,_impact_move;


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
        _press.action.performed-=OnPress;
        _press.action.Disable();
        _impact_move.action.performed-=Impact_move;
        _impact_move.action.Disable();
        maininput.Dispose();
    }
    void Start()
    {
        maininput=new MainInput();
        
        maininput.Enable();
        String txtmap="#+#\n#+#\n#+#\n+O+\n#+#";
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
                int r=b.getOffset();
                GameObject gameObject=Instantiate(b.gameObject,pos,Quaternion.Euler(0f,0f,-r*90),this.transform);
                Board[i,j]=gameObject;
                gameObject.GetComponent<Block>().Init();
            }
        }

        currentpos=new Vector2(0,0);
        currentposint=new Vector2Int(0,0);

        _press.action.performed+=OnPress;
        _press.action.Enable();
        _impact_move.action.performed+=Impact_move;
        _impact_move.action.Enable();

    }
    void Update()
    {
        Vector2 vec=maininput.Player.Move.ReadValue<Vector2>();
        float size=vec.x*vec.x+vec.y*vec.y;
        if(0<size){
            Move(vec,0.005f);
        }
        else{
            currentpos=currentposint;
        }

        UpdateCircuit();
    }

    public void OnPress(InputAction.CallbackContext context)
    {
        // 押された瞬間でPerformedとなる
        if (!context.performed) return;

        Debug.Log("Press");
    }
    public void Impact_move(InputAction.CallbackContext context){
        if(context.performed){
            Move(maininput.Player.Move.ReadValue<Vector2>(),0.5f);
        }
    }


    private void Move(Vector2 vec,float amp)
    {
        currentpos+=vec*amp;
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

    Vector2Int[] way={
        new Vector2Int(1,0),
        new Vector2Int(0,1),
        new Vector2Int(-1,0),
        new Vector2Int(0,-1)
    };
    void UpdateCircuit(){
        for(int i=0;i<column;i++){
            for(int j=0;j<maxrow;j++){
                Block b=Board[i,j].GetComponent<Block>();
                for(int p=0;p<4;p++){
                    Vector2Int v=way[p];

                    if(b.GPIO[p].getPintype()==Block.PINTYPE.INPUT||b.GPIO[p].getPintype()==Block.PINTYPE.PASSIVE){
                        if((0<=i+v.x&&i+v.x<column)&&(0<=j+v.y&&j+v.y<maxrow)){
                            Block next=Board[i+v.x,j+v.y].GetComponent<Block>();
                            //next.UpdateState();
                            int nextpin=(p+2)%4;
                            if(next.GPIO[nextpin].getPintype()==Block.PINTYPE.OUTPUT||next.GPIO[nextpin].getPintype()==Block.PINTYPE.PASSIVE){
                                b.GPIO[p].Power=Math.Max(next.GPIO[nextpin].Power-1,0);
                            }
                            else{
                                b.GPIO[p].Power=0;
                            }
                        }
                        else{
                            b.GPIO[p].Power=0;
                        }
                    }
                }
                b.UpdateState();
            }
        }
    }
}
