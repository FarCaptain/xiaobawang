/*
"Automated generation of customized algebraic expressions using binary expression trees"
(A substantiation of my Writing Sample)
By YuanQu
*/
using System;

namespace ExpressionGenerator
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
                preFlag = 'N';//the potential flag; if ==N, not extendable (only without bracket)
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
        multi multiple = new multi();

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
                    if (HQed == -1) tree[rig].preFlag = 'N';
                    else
                    {
                        tmp = highQue[ran.Next() % (HQed + 1)]; //'*'||'/'
                        tree[rig].preFlag = flagBox[tmp / 2, tmp % 2];
                    }
                }
            }
            else if (tree[cur].flag == '*')
            {
                tree[lft].val = multiple.randFact(tree[cur].val);
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
            leaf_que[ind] = lft;//push two son into queue
            if(useBrack||tree[rig].preFlag!='N') leaf_que[++leaf_ed] = (cur << 1) + 2;
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
	
    public class PrimePool
    {

        public static bool[] isprime = new bool[90005];
        public static int[] prime = new int[8720];//8713
        public static int totPrime = 0;

        public static void prePrime()//sieve of Eratosthenes
        {
            isprime[0] = isprime[1] = true;
            for (int i = 2; i <= 300; ++i)//sqrt(90k)
            {
                if (!isprime[i])
                    for (int j = i * i; j <= 90000; j += i)
                    {
                        isprime[j] = true;
                    }
            }
            for (int i = 0; i <= 90000; ++i)
            {
                if (!isprime[i])
                {
                    prime[totPrime++] = i;
                }
            }
        }
    }

    public class multi
    {
        struct breakPrime
        {
            internal int p, q;
        };

        private int quickPow(int p, int q)
        {
            int ret = 1;
            while (q != 0)
            {
                if ((q & 1) == 1)
                {
                    ret *= p;
                }
                q >>= 1;
                p = p * p;
            }
            return ret;
        }

        public int randFact(int num)//generate a factor of num
        {
            if (num <= 0) return 1;
            int totBreak = 0;
            breakPrime[] bp = new breakPrime[100];
            for (int i = 0; i < PrimePool.totPrime && PrimePool.prime[i] * PrimePool.prime[i] <= num; ++i)
            {
                if (num % PrimePool.prime[i] == 0)
                {
                    int cnt = 0;
                    bp[totBreak].p = PrimePool.prime[i];
                    while (num % PrimePool.prime[i] == 0)
                    {
                        num /= PrimePool.prime[i];
                        cnt++;
                    }
                    bp[totBreak].q = cnt;
                    totBreak++;
                }
            }
            if (num != 1)
            {
                bp[totBreak].p = num;
                bp[totBreak].q = 1;
                totBreak++;
            }

            int fact = 1;
            var seed = Guid.NewGuid().GetHashCode();
            Random ran = new Random(seed);
            for (int i = 0; i < totBreak; ++i)
            {
                if(i == 0) fact *= bp[i].p;
                else fact *= quickPow(bp[i].p, ran.Next() % (bp[i].q + 1));
            }
            return fact;
        }
    }
}

