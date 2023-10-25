import random

size=(8,15)

class Block:
    W=[False,False,False,False]
    def __turn(self,n)->list:
        return [self.W[(0+n)%4],self.W[(1+n)%4],self.W[(2+n)%4],self.W[(3+n)%4]]
    def getChar(self)->str:
        if self.W==[True,True,True,True]:
            return "+0"
        for i in range(4):
            if self.__turn(i)==[True,False,True,False]:
                return "I"+str(i)
            elif self.__turn(i)==[False,True,True,True]:
                return "T"+str(i)
            elif self.__turn(i)==[True,True,False,False]:
                return "L"+str(i)
            elif self.__turn(i)==[True,False,False,False]:
                return "#1"
        return "#0"

Board=[[Block() for i in range(size[1])] for j in range(size[0])]
for i in range(size[0]):
    for j in range(size[1]):
        Board[i][j].W=[False,False,False,False]

stack=[(0,0)]
way=((1,0),(0,1),(-1,0),(0,-1))

while 0<len(stack):
    current:tuple=stack[-1]
    v=random.randint(0,3)
    vec=way[v]
    print(current,end=",")
    next=(vec[0]+current[0],vec[1]+current[1])
    if((0<=next[0] and next[0]<size[0]) and (0<=next[1] and next[1]<size[1])) and Board[next[0]][next[1]].getChar()=="#0":
        Board[current[0]][current[1]].W[v]=True
        Board[next[0]][next[1]].W[(v+2)%4]=True
        stack.append(next)
    else:
        f=True
        for i in range(4):
            vec=way[i]
            next=(vec[0]+current[0],vec[1]+current[1])
            if((0<=next[0] and next[0]<size[0]) and (0<=next[1] and next[1]<size[1])) and Board[next[0]][next[1]].getChar()=="#0":
                Board[current[0]][current[1]].W[i]=True
                Board[next[0]][next[1]].W[(i+2)%4]=True
                stack.append(next)
                f=False
                break
    
        if f:
            stack.pop()

    print(vec)

for i in range(size[0]-1,-1,-1):
    for j in range(size[1]):
        print(Board[i][j].getChar(),end="")
    print("\\n",end="")