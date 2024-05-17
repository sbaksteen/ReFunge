using System.Collections;
using ReFunge.Data.Values;

namespace ReFunge.Data
{
    internal class FungeStack
    {
        private Stack<int> stack = new Stack<int>();

        public int Size => stack.Count;

        public FungeStack() { }

        public FungeStack(Stack<int> stack)
        {
            this.stack = stack;
        }

        public int this[int index]  
        { 
            get
            {
                if (index >= stack.Count)
                {
                    return 0;
                }
                return stack.ElementAt(index);
            }
        }

        public FungeInt Pop() { 
            if (stack.Count == 0)
            {
                return 0;
            }
            return stack.Pop(); 
        }

        public void Push(FungeInt value) 
        {
            stack.Push(value);
        }

        public FungeVector PopVector(int dim)
        {
            int[] ints = new int[dim];
            for (int i = dim - 1; i >= 0; i--)
            {
                ints[i] = Pop();
            }
            return new FungeVector(ints);
        }

        public void PushVector(FungeVector vector, int dim)
        {
            for (int i = 0; i < dim; i++)
            {
                Push(vector[i]);
            }
        }

        public FungeString PopString()
        {
            string str = "";
            char c = (char)Pop();
            while (c != 0)
            {
                str += c;
                c = (char)Pop();
            }
            return str;
        }

        public void PushString(FungeString str)
        {
            string s = str;
            Push(0);
            for (int i = s.Length - 1; i >= 0; i--)
            {
                Push(s[i]);
            }
        }

        internal void Clear()
        {
            stack.Clear();
        }

        internal FungeStack Clone()
        {
            return new FungeStack(new(stack));
        }
    }

    internal class FungeStackStack : IEnumerable<FungeStack>
    {
        private Stack<FungeStack> stack = new Stack<FungeStack>();

        public int Size => stack.Count;

        public FungeStack TOSS => stack.Peek();
        public FungeStack? SOSS => stack.Count > 1 ? stack.ElementAt(1) : null;

        public FungeStack this[int index] => stack.ElementAt(index);

        public FungeStackStack() {
            stack.Push(new FungeStack());
        }

        public FungeStackStack Clone()
        {
            FungeStackStack newStack = new();
            foreach (FungeStack s in stack)
            {
                newStack.stack.Push(s.Clone());
            }
            return newStack;
        }

        public void NewStack(int n)
        {
            List<int> newStack = [];
            for (int i = 0; i < n; i++)
            {
                newStack.Add(TOSS.Pop());
            }
            if (n < 0)
            {
                for (int i = 0; i > n; i--)
                {
                    TOSS.Push(0);
                }
            }
            stack.Push(new FungeStack(new Stack<int>(newStack)));
        }

        public void RemoveStack(int n)
        {
            List<int> transfer = [];
            for (int i = 0; i < n; i++)
            {
                transfer.Add(TOSS.Pop());
            }
            stack.Pop();
            transfer.Reverse();
            foreach (int i in transfer)
            {
                TOSS.Push(i);
            }
            if (n < 0)
            {
                for (int i = 0; i > n; i--)
                {
                    TOSS.Pop();
                }
            }
        }

        public IEnumerator<FungeStack> GetEnumerator()
        {
            return ((IEnumerable<FungeStack>)stack).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)stack).GetEnumerator();
        }
    }
}
