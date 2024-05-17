﻿namespace ReFunge.Data.Values
{
    internal interface IFungeValue<T> where T : IFungeValue<T>
    {
        public static abstract T PopFromStack(FungeIP ip);
        public void PushToStack(FungeIP ip);
    }

}