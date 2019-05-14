using System;

namespace 出题小霸王
{
    public class calcTree
    {
        string ret = "";
        struct node
        {
            public int val;
            public char flag;
            public char preFlag;
            public bool withBrack;
            public void Init()
            {
                withBrack = false;
                val = 0;
                flag = 'N';
                preFlag = 'N';//the potential flag; if ==N, not extendable (only without bracker)
            }
        };
        static int maxn;//max value of a number
        int layer;//max length of the equation
        const int treeSize = 50;
        const int queSize = 50;
        node[] tree = new node[treeSize];//1<<layer
        int[] leaf_que = new int[queSize];//1<<(layer-1) store index
        int leaf_ed = -1;//leaf_que pointer
        public bool useBrack = true;
        char[,] flagBox = { { '+', '-' }, { '*', '/' } };
        int[] flagQue = new int[4];
        int FQed = -1;
        int[] highQue = new int[2];// '*' '/'
        int HQed = -1;


        int prePrior = -1;//+-=>0 */=>1
        int preInd = -1;

        public void setFlagBox(bool plu, bool min, bool mul, bool div)//choose optional flags
        {
            if (plu) flagQue[++FQed] = 0;
            if (min) flagQue[++FQed] = 1;
            if (mul) { flagQue[++FQed] = 2; highQue[++HQed] = 2; }
            if (div) { flagQue[++FQed] = 3; highQue[++HQed] = 3; }
        }

        public calcTree()
        {
            for (int i = 0; i < treeSize; ++i)
            {
                tree[i].Init();
            }
        }
        public calcTree(int _max, int _layer)
        {
            for (int i = 0; i < treeSize; ++i)
            {
                tree[i].Init();
            }
            maxn = _max;
            layer = _layer;
        }

        void gen(int ind)//generate two son & select a flag for father
        {//ind is leaf_que index

            var seed = Guid.NewGuid().GetHashCode();
            Random ran = new Random(seed);

            int cur = leaf_que[ind]; //get tree index

            int lft = (cur << 1) + 1;
            int rig = (cur << 1) + 2;
            

            //get a curFlag
            if (useBrack || cur == 0)
            {
                int tmp = flagQue[ran.Next()%(FQed+1)];
                tree[cur].flag = flagBox[tmp/2,tmp%2];///set flag
            }
            else
            {
                tree[cur].flag = tree[cur].preFlag;
            }

            //extend 2 sons && set potential flag
            if (tree[cur].flag == '+')
            {
                tree[lft].val = ran.Next() % (tree[cur].val + 1);//[0,tree[cur].val]
                tree[rig].val = tree[cur].val - tree[lft].val;
                if (useBrack)
                {
                    if (cur != 0)
                    {
                        int fa = (cur - 1) / 2;
                        if (tree[fa].flag == '*' || tree[fa].flag == '/')
                        {
                            tree[cur].withBrack = true;
                        }
                        else if (tree[fa].flag == '-' && cur % 2 == 0)//cur is a right son 
                        {
                            tree[cur].withBrack = true;
                        }
                    }
                }
                else//set preFlag
                {
                    int tmp = flagQue[ran.Next() % (FQed + 1)];
                    tree[lft].preFlag = flagBox[tmp / 2,tmp % 2];
                    tmp = flagQue[ran.Next() % (FQed + 1)];
                    tree[rig].preFlag = flagBox[tmp / 2,tmp % 2];
                }
            }
            else if (tree[cur].flag == '-')
            {
                tree[lft].val = ran.Next() % (maxn - tree[cur].val + 1) + tree[cur].val;//[tree[cur].val,maxn]=>worst:tree[cur].val==maxn
                tree[rig].val = tree[lft].val - tree[cur].val;
                if (useBrack)
                {
                    if (cur != 0)
                    {
                        int fa = (cur - 1) / 2;
                        if (tree[fa].flag == '*' || tree[fa].flag == '/')
                        {
                            tree[cur].withBrack = true;
                        }
                        else if (tree[fa].flag == '-' && cur % 2 == 0)//cur is a right son 
                        {
                            tree[cur].withBrack = true;
                        }
                    }
                }
                else//set preFlag
                {
                    int tmp = flagQue[ran.Next() % (FQed + 1)];
                    tree[lft].preFlag = flagBox[tmp / 2,tmp % 2];
                    tmp = highQue[ran.Next() % (HQed + 1)]; //'*'||'/'
                    tree[rig].preFlag = flagBox[tmp / 2,tmp % 2];
                }
            }
            else if (tree[cur].flag == '*')
            {
                tree[lft].val = multi.randFact(tree[cur].val);
                tree[rig].val = tree[cur].val / tree[lft].val;
                if (useBrack)
                {
                    if (cur != 0)
                    {
                        int fa = (cur - 1) / 2;
                        if (tree[fa].flag == '/' && cur % 2 == 0)//cur is a right son 
                        {
                            tree[cur].withBrack = true;
                        }
                    }
                }
                else//set preFlag
                {
                    int tmp = highQue[ran.Next() % (HQed + 1)];//'*'||'/'
                    tree[lft].preFlag = flagBox[tmp / 2,tmp % 2];
                    tmp = highQue[ran.Next() % (HQed + 1)]; //'*'||'/'
                    tree[rig].preFlag = flagBox[tmp / 2,tmp % 2];
                }
            }
            else if (tree[cur].flag == '/')
            {
                if (tree[cur].val == 0) tree[rig].val = ran.Next() % maxn + 1;
                else tree[rig].val = ran.Next() % (maxn / tree[cur].val) + 1;//(0,maxn/tree[cur].val] =>worst tree[cur].val==maxn
                tree[lft].val = tree[cur].val * tree[rig].val;
                if (useBrack)
                {
                    if (cur != 0)
                    {
                        int fa = (cur - 1) / 2;
                        if (tree[fa].flag == '/' && cur % 2 == 0)//cur is a right son 
                        {
                            tree[cur].withBrack = true;
                        }
                    }
                }
                else//set preFlag
                {
                    int tmp = highQue[ran.Next() % (HQed + 1)];//'*'||'/'
                    tree[lft].preFlag = flagBox[tmp / 2,tmp % 2];
                    tree[rig].preFlag = 'N';
                }
            }
            leaf_que[ind] = (cur << 1) + 1;//push two son into queue
            if(useBrack||tree[cur].flag!='/') leaf_que[++leaf_ed] = (cur << 1) + 2;
        }//without 'meaningless' brackers


        void midTraver(int rt)//mid Traversal
        {

            if (rt != 0 && tree[(rt - 1) / 2].flag == 'N') return;

            int lft = (rt << 1) + 1;
            int rig = (rt << 1) + 2;
            node cur = tree[rt];
            if (cur.withBrack)
            {
                ret += "(";
            }
            midTraver(lft);
            if (cur.flag == 'N')
            {
                ret += Convert.ToString(cur.val);
            }
            else
            {
                ret += Convert.ToString(cur.flag);
            }
            midTraver(rig);
            if (cur.withBrack)
            {
                ret += ')';
            }
        }
        public void build()
        {
            var seed = Guid.NewGuid().GetHashCode();
            Random ran = new Random(seed);
            tree[0].val = ran.Next() % (maxn + 1);
            leaf_que[++leaf_ed] = 0;
            gen(0);
            for (int i = 0; i < layer - 2; i++)
            {
                int pick = ran.Next() % (leaf_ed + 1);
                gen(pick);
            }
            //tree[0].withBrack = false;
            midTraver(0);

        }
        public string equation()
        {
            return ret;
        }
        public string getAns()
        {
            return Convert.ToString(tree[0].val);
        }

    }
}

/*
 * 加减乘除
 * 括号
 */
