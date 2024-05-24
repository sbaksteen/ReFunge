namespace ReFunge.Data.Values;

public interface IFungeValue<out T> where T : IFungeValue<T>
{
    public static abstract T PopFromStack(FungeIP ip);
    public void PushToStack(FungeIP ip);
}