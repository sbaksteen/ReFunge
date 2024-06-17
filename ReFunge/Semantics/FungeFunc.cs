using System.Reflection;
using ReFunge.Data.Values;

namespace ReFunge.Semantics;

/// <summary>
///     Base class for all Funge functions.
///     A <see cref="FungeFunc" /> wraps a function that takes a <see cref="FungeIP" /> and zero or more
///     <see cref="IFungeValue{T}" />s
///     and returns either void or an <see cref="IFungeValue{T}" />. The function is executed by calling
///     <see cref="Execute" />.
///     When the function is executed, the IP is passed as the first argument, and any additional arguments are popped from
///     the
///     IP's stack. The result of the function is pushed back to the IP's stack.
///     <br />
///     With the help of reflection, a <see cref="FungeFunc" /> can be created from a method that has the correct
///     signature.
///     <br />
///     If the function throws a <see cref="FungeReflectException" />, the IP is reflected.
/// </summary>
public abstract class FungeFunc
{
    /// <summary>
    ///     Execute the function.
    /// </summary>
    /// <param name="ip">The IP executing the function.</param>
    public abstract void Execute(FungeIP ip);

    /// <summary>
    ///     Create a <see cref="FungeFunc" /> from a method that has the correct signature.
    ///     The method must return either void or an <see cref="IFungeValue{T}" />, take a <see cref="FungeIP" /> as the first
    ///     parameter, and zero or more <see cref="IFungeValue{T}" />s as the remaining parameters.
    /// </summary>
    /// <param name="method">The <see cref="MethodInfo" /> of the method to create the <see cref="FungeFunc" /> from.</param>
    /// <param name="instance">The fingerprint instance to call the method on, or null if the method is static.</param>
    /// <returns>The created <see cref="FungeFunc" />.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the method has an invalid signature.</exception>
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

        foreach (var helperMethod in typeof(FungeFunc).GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
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

        throw new InvalidOperationException("Method has invalid signature");
    }

    private static FungeFunc CreateHelperAction(MethodInfo method, object instance)
    {
        var dlg = (Action<FungeIP>)Delegate.CreateDelegate(typeof(Action<FungeIP>), instance, method);
        return new FungeAction(dlg);
    }

    private static FungeFunc CreateHelperFunc<TResult>(MethodInfo method, object instance)
        where TResult : IFungeValue<TResult>
    {
        var dlg = (Func<FungeIP, TResult>)Delegate.CreateDelegate(typeof(Func<FungeIP, TResult>), instance, method);
        return new FungeFunc<TResult>(dlg);
    }

    private static FungeFunc CreateHelperAction<T1>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1>
    {
        var dlg = (Action<FungeIP, T1>)Delegate.CreateDelegate(typeof(Action<FungeIP, T1>), instance, method);
        return new FungeAction<T1>(dlg);
    }

    private static FungeFunc CreateHelperFunc<T1, TResult>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1> where TResult : IFungeValue<TResult>
    {
        var dlg = (Func<FungeIP, T1, TResult>)Delegate.CreateDelegate(typeof(Func<FungeIP, T1, TResult>), instance,
            method);
        return new FungeFunc<T1, TResult>(dlg);
    }

    private static FungeFunc CreateHelperAction<T1, T2>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1> where T2 : IFungeValue<T2>
    {
        var dlg = (Action<FungeIP, T1, T2>)Delegate.CreateDelegate(typeof(Action<FungeIP, T1, T2>), instance, method);
        return new FungeAction<T1, T2>(dlg);
    }

    private static FungeFunc CreateHelperFunc<T1, T2, TResult>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1> where T2 : IFungeValue<T2> where TResult : IFungeValue<TResult>
    {
        var dlg = (Func<FungeIP, T1, T2, TResult>)Delegate.CreateDelegate(typeof(Func<FungeIP, T1, T2, TResult>),
            instance, method);
        return new FungeFunc<T1, T2, TResult>(dlg);
    }

    private static FungeFunc CreateHelperAction<T1, T2, T3>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1> where T2 : IFungeValue<T2> where T3 : IFungeValue<T3>
    {
        var dlg = (Action<FungeIP, T1, T2, T3>)Delegate.CreateDelegate(typeof(Action<FungeIP, T1, T2, T3>), instance,
            method);
        return new FungeAction<T1, T2, T3>(dlg);
    }

    private static FungeFunc CreateHelperFunc<T1, T2, T3, TResult>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1>
        where T2 : IFungeValue<T2>
        where T3 : IFungeValue<T3>
        where TResult : IFungeValue<TResult>
    {
        var dlg = (Func<FungeIP, T1, T2, T3, TResult>)Delegate.CreateDelegate(
            typeof(Func<FungeIP, T1, T2, T3, TResult>), instance, method);
        return new FungeFunc<T1, T2, T3, TResult>(dlg);
    }

    private static FungeFunc CreateHelperAction<T1, T2, T3, T4>(MethodInfo method, object instance)
        where T1 : IFungeValue<T1> where T2 : IFungeValue<T2> where T3 : IFungeValue<T3> where T4 : IFungeValue<T4>
    {
        var dlg = (Action<FungeIP, T1, T2, T3, T4>)Delegate.CreateDelegate(typeof(Action<FungeIP, T1, T2, T3, T4>),
            instance, method);
        return new FungeAction<T1, T2, T3, T4>(dlg);
    }

    private static FungeFunc CreateHelperFunc<T1, T2, T3, T4, TResult>(MethodInfo method, object instance)
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

    private static FungeFunc CreateHelperAction<T1, T2, T3, T4, T5>(MethodInfo method, object instance)
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

    private static FungeFunc CreateHelperFunc<T1, T2, T3, T4, T5, TResult>(MethodInfo method, object instance)
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

/// <summary>
///     Exception thrown when a function should reflect the IP. This is used to break out of the function and reflect the
///     IP
///     when a non-fatal error occurs. During execution, the exception is caught by the function wrapper in
///     <see cref="FungeFunc.Execute" /> and the IP is reflected.
/// </summary>
/// <param name="e">The inner exception that caused the reflection.</param>
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