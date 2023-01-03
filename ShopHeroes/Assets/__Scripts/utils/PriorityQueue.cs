using System;
using System.Collections;
using System.Collections.Generic;

public abstract class PriorityQueue<T> where T : IComparable<T>
{
    T[] mHeapArr;
    int mSize;
    int mCapacity;
    const int MAX_CAPACITY = 1 << 15;
    public int Count { get { return mSize; } }

    public PriorityQueue() : this(8) { }

    public PriorityQueue(int Capacity)
    {
        if (Capacity > MAX_CAPACITY)
            throw new System.Exception("capacity exceed 32768 !");
        mHeapArr = new T[Capacity];
        mCapacity = Capacity;
        mSize = 0;
    }
    public void enqueue(T t)
    {
        if (mSize >= mCapacity)
        {
            grow();
        }
        mHeapArr[mSize++] = t;
        floatUp();
    }

    public T dequeue()
    {
        T res = mHeapArr[0];
        mHeapArr[0] = mHeapArr[--mSize];
        mHeapArr[mSize] = default;
        floatDown();
        return res;
    }

    public T peek()
    {
        if (mSize > 0)
            return mHeapArr[0];
        else
            throw new InvalidOperationException("PriorityQueue is empty!!!");
    }

    abstract public bool compare(T a, T b);

    void floatUp()
    {
        int childIdx = mSize - 1;
        int parentIdx = (childIdx - 1) / 2;
        T newVal = mHeapArr[childIdx];
        while (childIdx > 0 && compare(newVal, mHeapArr[parentIdx]))
        {
            mHeapArr[childIdx] = mHeapArr[parentIdx];
            childIdx = parentIdx;
            parentIdx = (childIdx - 1) / 2;
        }
        mHeapArr[childIdx] = newVal;
    }

    void floatDown()
    {
        int parentIdx = 0;
        T val = mHeapArr[parentIdx];
        int maxIndex = mSize - 1;
        int childIdx = 2 * parentIdx + 1;//left child
        while (childIdx <= maxIndex)
        {
            if (childIdx + 1 < maxIndex && compare(mHeapArr[childIdx + 1], mHeapArr[childIdx]))
                childIdx++;
            if (compare(val, mHeapArr[childIdx]))
            {
                break;
            }
            var childVal = mHeapArr[childIdx];
            //mHeapArr[childIdx] = val;
            mHeapArr[parentIdx] = childVal;
            parentIdx = childIdx;
            childIdx = 2 * parentIdx + 1;
        }
        mHeapArr[parentIdx] = val;
    }

    void grow()
    {
        int size = mCapacity * 2;
        var newArr = new T[size];
        Array.Copy(mHeapArr, newArr, mCapacity);
        mHeapArr = newArr;
        mCapacity = size;
    }

    public void clear()
    {
        mSize = 0;
        for (int i = 0; i < mHeapArr.Length; i++)
        {
            mHeapArr[i] = default;
        }
    }
}

public class MinPriorityQueue<T> : PriorityQueue<T> where T : IComparable<T>
{
    public override bool compare(T a, T b) { return a.CompareTo(b) < 0; }
}

public class MaxPriorityQueue<T> : PriorityQueue<T> where T : IComparable<T>
{
    public override bool compare(T a, T b) { return a.CompareTo(b) > 0; }
}

public class MinPriorityQueueInt : PriorityQueue<int>
{
    public override bool compare(int a, int b)
    {
        return a < b;
    }
}
public class MaxPriorityQueueInt : PriorityQueue<int>
{
    public override bool compare(int a, int b)
    {
        return a > b;
    }
}

#if UNITY_EDITOR
public static class PriorityQueueTest
{
    public static void Test()
    {
        int num = 200;
        int[] vals = genArray(num);
        string enss = string.Empty;
        var minQ = new MinPriorityQueue<int>();
        for (int i = 0; i < num; i++)
        {
            minQ.enqueue(vals[i]);
            enss += vals[i] + ", ";
        }
        UnityEngine.Debug.Log("enqueue list : " + enss);

        int min = minQ.dequeue();
        string ss = min + "";
        for (int i = 0; i < num - 1; i++)
        {
            int de = minQ.dequeue();
            if (de < min)
            {
                throw new Exception("dequeue not the miniest!!!");
            }
            ss += "," + de;
        }
        UnityEngine.Debug.Log("test complete ss:" + ss);
    }

    static int[] genArray(int num)
    {
        List<int> candidates = new List<int>(num);
        for (int i = 1; i <= num; i++)
        {
            candidates.Add(i);
        }
        int[] res = new int[num];
        int ri = 0;
        while (candidates.Count > 0)
        {
            int idx = UnityEngine.Random.Range(0, candidates.Count);
            res[ri++] = candidates[idx];
            candidates.RemoveAt(idx);
        }
        return res;
    }
}

#endif