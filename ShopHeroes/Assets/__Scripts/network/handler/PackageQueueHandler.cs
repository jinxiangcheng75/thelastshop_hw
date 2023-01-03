using UnityEngine;
using System.Collections;
//using System.Threading;
using System;
using System.Threading;

public class LoopQueue<T>
{
    T[] mQueue;
    int mCursor;
    int mCapacity;
    int mHead;
    int mTail;

    public LoopQueue(int capacity)
    {
        mCapacity = capacity;
        mQueue = new T[mCapacity];
        mHead = 0;
        mTail = 0;
        Count = 0;
    }

    public int Count { get; private set; }

    public void clear()
    {
        mHead = 0;
        mTail = 0;
        Count = 0;
        for (int i = 0; i < mQueue.Length; i++)
        {
            mQueue[i] = default;
        }
    }

    public void enqueue(T t)
    {
        if (Count >= mCapacity)
            return;
        mQueue[mTail++] = t;
        mTail %= mCapacity;
        //if(mTail >= mCapacity) 
        //    mTail = 0;
        Count++;
    }

    public void enqueueHead(T t)
    {
        if (Count >= mCapacity)
            return;
        if (mHead < 1)
            mHead = mCapacity - 1;
        else
            mHead--;
        mQueue[mHead] = t;
        Count++;
    }

    public T dequeue()
    {
        if (Count == 0)
            return default;
        int old = mHead;
        T t = mQueue[mHead++];
        mQueue[old] = default;
        mHead %= mCapacity;
        //if(mHead >= mCapacity)
        //    mHead = 0;
        Count--;
        return t;
    }

    public T dequeueTail()
    {
        if (Count == 0)
            return default;
        int old = mTail;
        if (mTail < 1)
            mTail = mCapacity - 1;
        else
            mTail--;
        T t = mQueue[old];
        mQueue[old] = default;
        Count--;
        return t;
    }

    public T peekHead()
    {
        return mQueue[mHead];
    }
}

public class PackageQueueHandler<T>
{
    ReaderWriterLockSlim mRWLock;
    int mSleepTime = 100;
    Thread mSendThread;
    System.Object thisLock = new System.Object();
    LoopQueue<T> mSendQueue;
    LoopQueue<T> mReceiveQueue;
    LoopQueue<T> mReceiveHotfixQueue;
    System.Action<T> mMessageProcess;

    public PackageQueueHandler(System.Action<T> messageProcess)
    {
        if (messageProcess == null)
        {
            throw new System.ArgumentNullException();
        }
        mRWLock = new ReaderWriterLockSlim();
        mMessageProcess = messageProcess;
        int queueSize = 64;
        mSendQueue = new LoopQueue<T>(queueSize);
        mReceiveQueue = new LoopQueue<T>(queueSize);
        mReceiveHotfixQueue = new LoopQueue<T>(queueSize);
        // ThreadStart ts = new ThreadStart(threadLoop);
        mSendThread = new Thread(threadLoop);
        mSendThread.Name = "PackageSendThread";
    }

    public void start()
    {
        mSendThread.Start();
    }

    void threadLoop()
    {
        while (!isExit)
        {
            if (!mSendThread.IsAlive) continue;
            try
            {
                Thread.Sleep(mSleepTime);
            }
            catch (System.Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError(e);
                //throw e;
#endif
            }
            T t = popSendQueue();
            if (t != null)
            {
                try
                {
                    mMessageProcess(t);
                }
                catch (System.Exception e)
                {
#if UNITY_EDITOR
                    Debug.LogError(e);
                    //throw e;
#endif
                }
            }
        }
    }

    public void pushSendQueue(T t)
    {
        lock (mSendQueue)
        {
            mSendQueue.enqueue(t);
        }
    }

    public T popSendQueue()
    {
        T t = default(T);
        lock (mSendQueue)
        {
            //int cnt = mSendQueue.Count;
            t = mSendQueue.dequeue();
        }
        return t;
    }

    public void pushReceivedQueue(T t)
    {
        lock (mReceiveQueue)
        {
            mReceiveQueue.enqueue(t);
        }
    }

    public T popReceivedQueue()
    {
        T t = default(T);
        lock (mReceiveQueue)
        {
            t = mReceiveQueue.dequeue();
        }
        return t;
    }

    bool isExit = false;
    public void abort()
    {
        isExit = true;
        // try
        // {
        //     mSendThread.Abort();
        // }
        // catch (Exception e)
        // {
        //     Debug.LogError(e);
        // }
        mSendQueue.clear();
        mReceiveQueue.clear();
        mReceiveHotfixQueue.clear();
    }

}