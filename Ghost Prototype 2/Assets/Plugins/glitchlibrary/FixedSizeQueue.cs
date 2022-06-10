using System.Collections.Generic;

public class FixedSizedQueue<T> : Queue<T>
{
    private readonly object privateLockObject = new object();
    public int Size { get; private set; }

    public FixedSizedQueue(int size)
    {
        Size = size;
    }

    public new void Enqueue(T obj)
    {
        base.Enqueue(obj);
        lock (privateLockObject)
        {
            while (Count > Size)
            {
                Dequeue();
            }
        }
    }
}