using System.Reflection;
using ReFunge.Data.Values;

namespace ReFunge.Semantics;

public abstract class FungeFunc
{
    public abstract void Execute(FungeIP ip);

    public static FungeFunc Create(MethodInfo method, object? instance)
    {
        var returnType = method.ReturnType;
        var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
        var returnTypeValid = returnType == typeof(void) ||
                              returnType.GetInterfaces().Any(i =>
                                  i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFungeValue<>));
        var parameterTypesValid = parameterTypes.Length > 0 &&
                                  parameterTypes[0] == typeof(FungeIP) &&
                                  parameterTypes.Skip(1).All(t =>
                                      t.GetInterfaces().Any(i =>
                                          i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFungeValue<>)
                                      )
                                  );
        if (!returnTypeValid || !parameterTypesValid)
            throw new InvalidOperationException(
                "Method must return an IFungeValue and take only IFungeValues as parameters");

        if (method.ReturnType == typeof(void) && method.GetParameters().Length == 1)
            return CreateHelperAction(method, instance);

        foreach (var helperMethod in typeof(FungeFunc).GetMethods(BindingFlags.Static | BindingFlags.Public))
        {
            if (method.ReturnType == typeof(void) &&
                helperMethod is { Name: "CreateHelperAction", IsGenericMethod: true } &&
                helperMethod.GetGenericArguments().Length == method.GetParameters().Length - 1)
            {
                var genericArguments = method.GetParameters().Skip(1).Select(p => p.ParameterType).ToList();
                var genericHelperMethod = helperMethod.MakeGenericMethod(genericArguments.ToArray());
                return (FungeFunc)genericHelperMethod.Invoke(null, [method, instance])!;
            }

            if (method.ReturnType != typeof(void) &&
                helperMethod is { Name: "CreateHelperFunc", IsGenericMethod: true } &&
                helperMethod.GetGenericArguments().Length == method.GetParameters().Length)
            {
                var genericArguments = method.GetParameters().Skip(1).Select(p => p.ParameterType)
                    .Append(method.ReturnType).ToList();
                var genericHelperMethod = helperMethod.MakeGenericMethod(genericArguments.ToArray());
                return (FungeFunc)genericHelperMethod.Invoke(null, [method, instance])!;
            }
        }

        throw new ArgumentException("Method has invalid signature");
    }

    public static FungeFunc CreateHelperAction(MethodInfo method, object instance)
    {
        var dlg = (Action<FungeIP>)Delegate.CreateDelegate(typeof(Action<FungeIP>), instance, method);
        return new FungeAction(dlg);
    }

    public static FungeFunc CreateHelperFunc<TResult>(MethodInfo method, object instance)
        where TResult : IFungeValue<TResult>
    {
        var dlg = (Func<FungeIP, TResult>)Delegate.CreateDelegate(typeof(Func<FungeIP, TResult>), instance, method);
        return new FungeFunc<TResult>(dlg);
    }

    public static FungeFunc CreateHelperAction<T1>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1>
    {
        var dlg = (Action<FungeIP, T1>)Delegate.CreateDelegate(typeof(Action<FungeIP, T1>), instance, method);
        return new FungeAction<T1>(dlg);
    }

    public static FungeFunc CreateHelperFunc<T1, TResult>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1> where TResult : IFungeValue<TResult>
    {
        var dlg = (Func<FungeIP, T1, TResult>)Delegate.CreateDelegate(typeof(Func<FungeIP, T1, TResult>), instance,
            method);
        return new FungeFunc<T1, TResult>(dlg);
    }

    public static FungeFunc CreateHelperAction<T1, T2>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1> where T2 : IFungeValue<T2>
    {
        var dlg = (Action<FungeIP, T1, T2>)Delegate.CreateDelegate(typeof(Action<FungeIP, T1, T2>), instance, method);
        return new FungeAction<T1, T2>(dlg);
    }

    public static FungeFunc CreateHelperFunc<T1, T2, TResult>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1> where T2 : IFungeValue<T2> where TResult : IFungeValue<TResult>
    {
        var dlg = (Func<FungeIP, T1, T2, TResult>)Delegate.CreateDelegate(typeof(Func<FungeIP, T1, T2, TResult>),
            instance, method);
        return new FungeFunc<T1, T2, TResult>(dlg);
    }

    public static FungeFunc CreateHelperAction<T1, T2, T3>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1> where T2 : IFungeValue<T2> where T3 : IFungeValue<T3>
    {
        var dlg = (Action<FungeIP, T1, T2, T3>)Delegate.CreateDelegate(typeof(Action<FungeIP, T1, T2, T3>), instance,
            method);
        return new FungeAction<T1, T2, T3>(dlg);
    }

    public static FungeFunc CreateHelperFunc<T1, T2, T3, TResult>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1>
        where T2 : IFungeValue<T2>
        where T3 : IFungeValue<T3>
        where TResult : IFungeValue<TResult>
    {
        var dlg = (Func<FungeIP, T1, T2, T3, TResult>)Delegate.CreateDelegate(
            typeof(Func<FungeIP, T1, T2, T3, TResult>), instance, method);
        return new FungeFunc<T1, T2, T3, TResult>(dlg);
    }

    public static FungeFunc CreateHelperAction<T1, T2, T3, T4>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1> where T2 : IFungeValue<T2> where T3 : IFungeValue<T3> where T4 : IFungeValue<T4>
    {
        var dlg = (Action<FungeIP, T1, T2, T3, T4>)Delegate.CreateDelegate(typeof(Action<FungeIP, T1, T2, T3, T4>),
            instance, method);
        return new FungeAction<T1, T2, T3, T4>(dlg);
    }

    public static FungeFunc CreateHelperFunc<T1, T2, T3, T4, TResult>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1>
        where T2 : IFungeValue<T2>
        where T3 : IFungeValue<T3>
        where T4 : IFungeValue<T4>
        where TResult : IFungeValue<TResult>
    {
        var dlg = (Func<FungeIP, T1, T2, T3, T4, TResult>)Delegate.CreateDelegate(
            typeof(Func<FungeIP, T1, T2, T3, T4, TResult>), instance, method);
        return new FungeFunc<T1, T2, T3, T4, TResult>(dlg);
    }

    public static FungeFunc CreateHelperAction<T1, T2, T3, T4, T5>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1>
        where T2 : IFungeValue<T2>
        where T3 : IFungeValue<T3>
        where T4 : IFungeValue<T4>
        where T5 : IFungeValue<T5>
    {
        var dlg = (Action<FungeIP, T1, T2, T3, T4, T5>)Delegate.CreateDelegate(
            typeof(Action<FungeIP, T1, T2, T3, T4, T5>), instance, method);
        return new FungeAction<T1, T2, T3, T4, T5>(dlg);
    }

    public static FungeFunc CreateHelperFunc<T1, T2, T3, T4, T5, TResult>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1>
        where T2 : IFungeValue<T2>
        where T3 : IFungeValue<T3>
        where T4 : IFungeValue<T4>
        where T5 : IFungeValue<T5>
        where TResult : IFungeValue<TResult>
    {
        var dlg = (Func<FungeIP, T1, T2, T3, T4, T5, TResult>)Delegate.CreateDelegate(
            typeof(Func<FungeIP, T1, T2, T3, T4, T5, TResult>), instance, method);
        return new FungeFunc<T1, T2, T3, T4, T5, TResult>(dlg);
    }
}

public class FungeReflectException(Exception? e = null) : Exception(e?.Message, e);

internal class FungeFunc<TResult>(Func<FungeIP, TResult> func) : FungeFunc
    where TResult : IFungeValue<TResult>
{
    public override void Execute(FungeIP ip)
    {
        try
        {
            var result = func(ip);
            result.PushToStack(ip);
        }
        catch (FungeReflectException)
        {
            ip.Reflect();
        }
    }
}

internal class FungeAction(Action<FungeIP> action) : FungeFunc
{
    public override void Execute(FungeIP ip)
    {
        try
        {
            action(ip);
        }
        catch (FungeReflectException)
        {
            ip.Reflect();
        }
    }
}

internal class FungeFunc<T1, TResult>(Func<FungeIP, T1, TResult> func) : FungeFunc
    where T1 : IFungeValue<T1>
    where TResult : IFungeValue<TResult>
{
    public override void Execute(FungeIP ip)
    {
        var arg1 = T1.PopFromStack(ip);
        try
        {
            var result = func(ip, arg1);
            result.PushToStack(ip);
        }
        catch (FungeReflectException)
        {
            ip.Reflect();
        }
    }
}

internal class FungeAction<T1>(Action<FungeIP, T1> action) : FungeFunc
    where T1 : IFungeValue<T1>
{
    public override void Execute(FungeIP ip)
    {
        var arg1 = T1.PopFromStack(ip);
        try
        {
            action(ip, arg1);
        }
        catch (FungeReflectException)
        {
            ip.Reflect();
        }
    }
}

internal class FungeFunc<T1, T2, TResult>(Func<FungeIP, T1, T2, TResult> func) : FungeFunc
    where T1 : IFungeValue<T1>
    where T2 : IFungeValue<T2>
    where TResult : IFungeValue<TResult>
{
    public override void Execute(FungeIP ip)
    {
        var arg2 = T2.PopFromStack(ip);
        var arg1 = T1.PopFromStack(ip);
        try
        {
            var result = func(ip, arg1, arg2);
            result.PushToStack(ip);
        }
        catch (FungeReflectException)
        {
            ip.Reflect();
        }
    }
}

internal class FungeAction<T1, T2>(Action<FungeIP, T1, T2> action) : FungeFunc
    where T1 : IFungeValue<T1> where T2 : IFungeValue<T2>
{
    public override void Execute(FungeIP ip)
    {
        var arg2 = T2.PopFromStack(ip);
        var arg1 = T1.PopFromStack(ip);
        try
        {
            action(ip, arg1, arg2);
        }
        catch (FungeReflectException)
        {
            ip.Reflect();
        }
    }
}

internal class FungeFunc<T1, T2, T3, TResult>(Func<FungeIP, T1, T2, T3, TResult> func) : FungeFunc
    where T1 : IFungeValue<T1>
    where T2 : IFungeValue<T2>
    where T3 : IFungeValue<T3>
    where TResult : IFungeValue<TResult>
{
    public override void Execute(FungeIP ip)
    {
        var arg3 = T3.PopFromStack(ip);
        var arg2 = T2.PopFromStack(ip);
        var arg1 = T1.PopFromStack(ip);
        try
        {
            var result = func(ip, arg1, arg2, arg3);
            result.PushToStack(ip);
        }
        catch (FungeReflectException)
        {
            ip.Reflect();
        }
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
        try
        {
            action(ip, arg1, arg2, arg3);
        }
        catch (FungeReflectException)
        {
            ip.Reflect();
        }
    }
}

internal class FungeFunc<T1, T2, T3, T4, TResult>(Func<FungeIP, T1, T2, T3, T4, TResult> func) : FungeFunc
    where T1 : IFungeValue<T1>
    where T2 : IFungeValue<T2>
    where T3 : IFungeValue<T3>
    where T4 : IFungeValue<T4>
    where TResult : IFungeValue<TResult>
{
    public override void Execute(FungeIP ip)
    {
        var arg4 = T4.PopFromStack(ip);
        var arg3 = T3.PopFromStack(ip);
        var arg2 = T2.PopFromStack(ip);
        var arg1 = T1.PopFromStack(ip);
        try
        {
            var result = func(ip, arg1, arg2, arg3, arg4);
            result.PushToStack(ip);
        }
        catch (FungeReflectException)
        {
            ip.Reflect();
        }
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
        try
        {
            action(ip, arg1, arg2, arg3, arg4);
        }
        catch (FungeReflectException)
        {
            ip.Reflect();
        }
    }
}

internal class FungeFunc<T1, T2, T3, T4, T5, TResult>(Func<FungeIP, T1, T2, T3, T4, T5, TResult> func) : FungeFunc
    where T1 : IFungeValue<T1>
    where T2 : IFungeValue<T2>
    where T3 : IFungeValue<T3>
    where T4 : IFungeValue<T4>
    where T5 : IFungeValue<T5>
    where TResult : IFungeValue<TResult>
{
    public override void Execute(FungeIP ip)
    {
        var arg5 = T5.PopFromStack(ip);
        var arg4 = T4.PopFromStack(ip);
        var arg3 = T3.PopFromStack(ip);
        var arg2 = T2.PopFromStack(ip);
        var arg1 = T1.PopFromStack(ip);
        try
        {
            var result = func(ip, arg1, arg2, arg3, arg4, arg5);
            result.PushToStack(ip);
        }
        catch (FungeReflectException)
        {
            ip.Reflect();
        }
    }
}

internal class FungeAction<T1, T2, T3, T4, T5>(Action<FungeIP, T1, T2, T3, T4, T5> action) : FungeFunc
    where T1 : IFungeValue<T1>
    where T2 : IFungeValue<T2>
    where T3 : IFungeValue<T3>
    where T4 : IFungeValue<T4>
    where T5 : IFungeValue<T5>
{
    public override void Execute(FungeIP ip)
    {
        var arg5 = T5.PopFromStack(ip);
        var arg4 = T4.PopFromStack(ip);
        var arg3 = T3.PopFromStack(ip);
        var arg2 = T2.PopFromStack(ip);
        var arg1 = T1.PopFromStack(ip);
        try
        {
            action(ip, arg1, arg2, arg3, arg4, arg5);
        }
        catch (FungeReflectException)
        {
            ip.Reflect();
        }
    }
}