﻿using ReFunge.Data;
using ReFunge.Data.Values;

namespace ReFunge
{
    internal class Interpreter
    {
        internal List<FungeIP> IPList = [];

        // New IPs should always be inserted before the parent
        // If the new IP does not have a parent, insert it at the start
        internal List<(FungeIP ip, FungeIP? parent)> NewIPList = [];

        internal int IPID;

        internal FungeSpace PrimarySpace;

        internal int ReturnValue = 0;

        internal bool Quit;

        internal long Tick;
        
        internal TextWriter Output;
        
        internal TextReader Input;

        public Interpreter(TextReader? input = null, TextWriter? output = null, int dim = 2)
        {
            if (input is null)
            {
                input = Console.In;
            }
            if (output is null)
            {
                output = Console.Out;
            }
            Output = output;
            Input = input;
            PrimarySpace = new FungeSpace(dim);
            IPList.Add(new FungeIP(IPID++, PrimarySpace, this));
        }
        
        public void AddNewIP(FungeIP ip, FungeIP? parent = null)
        {
            NewIPList.Add((ip, parent));
        }

        public void WriteCharacter(char c)
        {
            Output.Write(c);
        }

        public void WriteInteger(int i)
        {
            Output.Write($"{i} ");
        }
        
        public bool EndOfInput()
        {
            return Input.Peek() == -1;
        }

        public int ReadCharacter()
        {
            return Input.Read();
        }
        
        public int ReadInteger()
        {
            if (Input.Peek() != '-' && (Input.Peek() < '0' || Input.Peek() > '9'))
            {
                return 0;
            }
            int c = Input.Read();
            if (c == '-')
            {
                return -ReadInteger();
            }
            int r = 0;
            while (c >= '0' && c <= '9')
            {
                r = r * 10 + c - '0';
                c = Input.Read();
            }
            return r;
        }

        public void DoStep()
        {
            if (Quit) { return; }
            List<FungeIP> toRemove = [];
            foreach (var ip in IPList)
            {
                ip.Step();
                if (ip.RequestQuit)
                {
                    Quit = true;
                    return;
                }
                if (!ip.Alive)
                {
                    toRemove.Add(ip);
                }
            }
            foreach (var (ip, parent) in NewIPList)
            {
                if (parent is null || !IPList.Contains(parent))
                {
                    IPList.Insert(0, ip);
                    continue;
                }
                IPList.Insert(IPList.IndexOf(parent), ip);
            }
            foreach (var ip in toRemove)
            {
                IPList.Remove(ip);
            }
            Tick++;
        }

        public int Run()
        {
            while (!Quit && IPList.Count > 0)
            {
                DoStep();
            }
            return ReturnValue;
        }

        public void Load(string filename)
        {
            PrimarySpace.LoadFile(new FungeVector(), filename);
        }
    }
}
