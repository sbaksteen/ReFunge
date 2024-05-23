using ReFunge.Data.Values;

namespace ReFunge.Semantics;

public abstract class FungeFunc
{
    public abstract void Execute(FungeIP ip);
}

internal class FungeFunc<TResult>(Func<FungeIP, TResult> func) : FungeFunc 
    where TResult : IFungeValue<TResult>
{
    public override void Execute(FungeIP ip)
    {
            var result = func(ip);
            result.PushToStack(ip);
        }
}

internal class FungeAction(Action<FungeIP> action) : FungeFunc
{
    public override void Execute(FungeIP ip)
    {
            action(ip);
        }
}

internal class FungeFunc<T1, TResult>(Func<FungeIP, T1, TResult> func) : FungeFunc 
    where T1 : IFungeValue<T1> 
    where TResult : IFungeValue<TResult>
{
    public override void Execute(FungeIP ip)
    {
            var arg1 = T1.PopFromStack(ip);
            var result = func(ip, arg1);
            result.PushToStack(ip);
        }
}

internal class FungeAction<T1>(Action<FungeIP, T1> action) : FungeFunc 
    where T1 : IFungeValue<T1>
{
    public override void Execute(FungeIP ip)
    {
            var arg1 = T1.PopFromStack(ip);
            action(ip, arg1);
        }
}

internal class FungeFunc<T1, T2, TResult>(Func<FungeIP, T1, T2, TResult> func) : FungeFunc 
    where T1 : IFungeValue<T1> where T2 : IFungeValue<T2> 
    where TResult : IFungeValue<TResult>
{
    public override void Execute(FungeIP ip)
    {
            var arg2 = T2.PopFromStack(ip);
            var arg1 = T1.PopFromStack(ip);
            var result = func(ip, arg1, arg2);
            result.PushToStack(ip);
        }
}

internal class FungeAction<T1, T2>(Action<FungeIP, T1, T2> action) : FungeFunc 
    where T1 : IFungeValue<T1> where T2 : IFungeValue<T2>
{
    public override void Execute(FungeIP ip)
    {
            var arg2 = T2.PopFromStack(ip);
            var arg1 = T1.PopFromStack(ip);
            action(ip, arg1, arg2);
        }
}

internal class FungeFunc<T1, T2, T3, TResult>(Func<FungeIP, T1, T2, T3, TResult> func) : FungeFunc 
    where T1 : IFungeValue<T1> where T2 : IFungeValue<T2> where T3 : IFungeValue<T3> 
    where TResult : IFungeValue<TResult>
{
    public override void Execute(FungeIP ip)
    {
            var arg3 = T3.PopFromStack(ip);
            var arg2 = T2.PopFromStack(ip);
            var arg1 = T1.PopFromStack(ip);
            var result = func(ip, arg1, arg2, arg3);
            result.PushToStack(ip);
        }
}

internal class FungeAction<T1, T2, T3>(Action<FungeIP, T1, T2, T3> action) : FungeFunc 
    where T1 : IFungeValue<T1> where T2 : IFungeValue<T2> where T3 : IFungeValue<T3>
{
    public override void Execute(FungeIP ip)
    {
            var arg3 = T3.PopFromStack(ip);
            var arg2 = T2.PopFromStack(ip);
            var arg1 = T1.PopFromStack(ip);
            action(ip, arg1, arg2, arg3);
        }
}

internal class FungeFunc<T1, T2, T3, T4, TResult>(Func<FungeIP, T1, T2, T3, T4, TResult> func) : FungeFunc 
    where T1 : IFungeValue<T1> where T2 : IFungeValue<T2> where T3 : IFungeValue<T3> where T4 : IFungeValue<T4> 
    where TResult : IFungeValue<TResult>
{
    public override void Execute(FungeIP ip)
    {
            var arg4 = T4.PopFromStack(ip);
            var arg3 = T3.PopFromStack(ip);
            var arg2 = T2.PopFromStack(ip);
            var arg1 = T1.PopFromStack(ip);
            var result = func(ip, arg1, arg2, arg3, arg4);
            result.PushToStack(ip);
        }
}

internal class FungeAction<T1, T2, T3, T4>(Action<FungeIP, T1, T2, T3, T4> action) : FungeFunc 
    where T1 : IFungeValue<T1> where T2 : IFungeValue<T2> where T3 : IFungeValue<T3> where T4 : IFungeValue<T4>
{

    public override void Execute(FungeIP ip)
    {
            var arg4 = T4.PopFromStack(ip);
            var arg3 = T3.PopFromStack(ip);
            var arg2 = T2.PopFromStack(ip);
            var arg1 = T1.PopFromStack(ip);
            action(ip, arg1, arg2, arg3, arg4);
        }
}

internal class FungeFunc<T1, T2, T3, T4, T5, TResult>(Func<FungeIP, T1, T2, T3, T4, T5, TResult> func) : FungeFunc 
    where T1 : IFungeValue<T1> where T2 : IFungeValue<T2> where T3 : IFungeValue<T3> where T4 : IFungeValue<T4> where T5 : IFungeValue<T5> 
    where TResult : IFungeValue<TResult>
{

    public override void Execute(FungeIP ip)
    {
            var arg5 = T5.PopFromStack(ip);
            var arg4 = T4.PopFromStack(ip);
            var arg3 = T3.PopFromStack(ip);
            var arg2 = T2.PopFromStack(ip);
            var arg1 = T1.PopFromStack(ip);
            var result = func(ip, arg1, arg2, arg3, arg4, arg5);
            result.PushToStack(ip);
        }
}

internal class FungeAction<T1, T2, T3, T4, T5>(Action<FungeIP, T1, T2, T3, T4, T5> action) : FungeFunc 
    where T1 : IFungeValue<T1> where T2 : IFungeValue<T2> where T3 : IFungeValue<T3> where T4 : IFungeValue<T4> where T5 : IFungeValue<T5>
{
    public override void Execute(FungeIP ip)
    {
            var arg5 = T5.PopFromStack(ip);
            var arg4 = T4.PopFromStack(ip);
            var arg3 = T3.PopFromStack(ip);
            var arg2 = T2.PopFromStack(ip);
            var arg1 = T1.PopFromStack(ip);
            action(ip, arg1, arg2, arg3, arg4, arg5);
        }
}