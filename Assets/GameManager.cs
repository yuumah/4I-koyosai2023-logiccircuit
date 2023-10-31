using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    private InputActionReference _press,_impact_move,_rotate,_hold;

    private bool isGoal=false;
    private List<string> mapsPath=new List<string>();


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
        _hold.action.performed-=ForceReset;
        _hold.action.Disable();
        _press.action.performed-=OnPress;
        _press.action.Disable();
        _impact_move.action.performed-=Impact_move;
        _impact_move.action.Disable();
        _rotate.action.performed-=RotateSocket;
        _rotate.action.Disable();
        maininput.Dispose();
    }
    void Start()
    {
        Cursor.visible=false;
        Getfiles();
        maininput=new MainInput();
        
        maininput.Enable();
        String txtmap=ReadText(mapsPath[0]);
        //String txtmap="#0#0#0G0#0#0#0\n#0#0#0I0#0#0#0\n#0#0L1*0L2#0#0\n#0#0S0S0S0#0#0";
        //String txtmap="L1I1I1L2L1I1T0L2L1L2#1T0T0I1L2\nI0#1I1T1#1L1L3L0L3L0L2#1I0#1I0\nT3I1L2L0L2T3L2#1T0L2I0L1L3I0I0\nL0#1I0#1I0I0L0L2I0L0L3L0L2L0T1\nL1I1L3I0I0T3#1N0L0L2L1L2T3L2#1\nN0L1L2T3L3#1L1L3#1L0L3I0#1L0L2\nI0I0L0L3L1L2I0#1T3T0#1L0L2L1T1\n#1L0I1I1L3L0L3L0L3L0I1I1T2L3#1";
        String[] map=txtmap.Split("\n");
        column=map.Length;
        for(int i=0;i<column;i++){
            int row=0;
            for(int j=0;j<map[i].Length;j++){
                if((map[i][j]<'0'||'9'<map[i][j])&&('!'<=map[i][j]&&map[i][j]<='~')){
                    row++;
                }
            }
            if(maxrow<row){
                maxrow=row;
            }
        }
        Board=new GameObject[column,maxrow];
        MainCam.transform.position=new Vector3(((float)maxrow-1)/2,((float)column-1)/2,MainCam.transform.position.z);
        MainCam.orthographicSize=((float)column)/2;

        for(int i=0;i<column;i++){
            int offset=0;
            for(int j=0;j<maxrow;j++){
                Vector3 pos=new Vector3(j,i)+this.transform.position;
                char id='$';
                int r=0;
                if(j+offset<map[column-i-1].Length){
                    id=map[column-i-1][j+offset];
                    while(j+offset+1<map[column-i-1].Length){
                        char c=map[column-i-1][j+offset+1];
                        if('0'<=c&&c<='9'){
                            r+=c-'0';
                            offset++;
                        }
                        else{
                            break;
                        }
                    }
                }
                Block b=findBlock(id);
                GameObject gameObject=Instantiate(b.gameObject,pos,Quaternion.Euler(0f,0f,-r*90),this.transform);
                Board[i,j]=gameObject;
                Block B=gameObject.GetComponent<Block>();
                B.setOffset(r);
                B.Init();
            }
        }

        currentpos=new Vector2(0,0);
        currentposint=new Vector2Int(0,0);

        _press.action.performed+=OnPress;
        _press.action.Enable();
        _rotate.action.performed+=RotateSocket;
        _rotate.action.Enable();
        _impact_move.action.performed+=Impact_move;
        _impact_move.action.Enable();
        _hold.action.performed+=ForceReset;
        _hold.action.Enable();


    }
    void Update()
    {
        if(!isGoal){
            Vector2 vec=maininput.Player.Move.ReadValue<Vector2>();
            float size=vec.x*vec.x+vec.y*vec.y;
            if(0<size){
                Move(vec,Time.deltaTime*4);
            }
            else{
                currentpos=currentposint;
            }

            UpdateCircuit();
        }
        else{
            if(0<maininput.Player.ReStart.ReadValue<float>()){
                SceneManager.LoadScene("MainGame");
            }
        }
    }

    public void OnPress(InputAction.CallbackContext context)
    {
        if(isGoal){
            return;
        }
        // 押された瞬間でPerformedとなる
        if (!context.performed) return;

        Block b=Board[currentposint.y,currentposint.x].GetComponent<Block>();
        if(b.getType()==Block.TYPE.SOCKET){
            Socket s=b.GetComponent<Socket>();
            s.SwitchToNext();
        }
    }
    public void RotateSocket(InputAction.CallbackContext context){
        if(isGoal)return;
        if(context.performed){
            Block b=Board[currentposint.y,currentposint.x].GetComponent<Block>();
            if(b.getType()==Block.TYPE.SOCKET){
                Socket s=b.GetComponent<Socket>();
                if(0<context.ReadValue<float>()){
                    s.Turn();
                }
                else{
                    s.Turn(3);
                }
            }
        }
    }
    public void Impact_move(InputAction.CallbackContext context){
        if(isGoal)return;
        if(context.performed){
            Move(maininput.Player.Move.ReadValue<Vector2>(),0.5f);
        }
    }

    public void ForceReset(InputAction.CallbackContext context){
        SceneManager.LoadScene("MainGame");
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
        Select.transform.position=new Vector3(currentposint.x,currentposint.y,Select.transform.position.z);
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

    public void Goal(){
        isGoal=true;
        this.GetComponent<Animator>().SetBool("Clear",true);
    }


    private void Getfiles(){
        String folderpath="maps";
        string[] fs =Directory.GetFiles (folderpath,"*",SearchOption.TopDirectoryOnly);
        for(int i=0;i<fs.Length;i++){
            if(Path.GetExtension(fs[i])==".txt"){
                mapsPath.Add(fs[i]);
            }
        }
        for(int i=0;i<mapsPath.Count;i++){
            Debug.Log(mapsPath[i]);
            Debug.Log(ReadText(mapsPath[i]));
        }
    }
    private static string ReadText(string iPath)
    {
        using var fs = new FileStream(iPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fs, Encoding.UTF8);
        string textContent = reader.ReadToEnd();
        return textContent;
    }
}
