using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LFUCache<T,U>
{

    Dictionary<T, LFUNode<T,U>> cache;
    LFUNode<T,U> head;
    LFUNode<T,U> tail;
    int capacity;//上限
    int size; //当前容量
    Action<T> onRemoveLastHandler; //移除最后一个时的处理

    public LFUCache(int capacity,Action<T> onRemoveLastHandler = null)
    {
        cache = new Dictionary<T, LFUNode<T,U>>(capacity);
        this.capacity = capacity;
        this.onRemoveLastHandler = onRemoveLastHandler;
        head = new LFUNode<T,U>();
        tail = new LFUNode<T,U>();
        head.post = tail;
        tail.pre = head;
    }

    public U Get(T key)
    {
        if (cache.TryGetValue(key, out LFUNode<T,U> node))
        {
            node.freq++;
            moveToNewPosition(node);
            return node.value;
        }

        return default;
    }

    public void Put(T key, U value)
    {
        if (capacity == 0)
        {
            return;
        }

        if (cache.TryGetValue(key,out LFUNode<T,U> node))
        {
            node.value = value;
            node.freq++;
            moveToNewPosition(node);
        }
        else
        {
            if (size == capacity)
            {
                cache.Remove(head.post.key);
                removeNode(head.post);
                size--;
                onRemoveLastHandler?.Invoke(head.post.key);
            }
            LFUNode<T,U> newNode = new LFUNode<T,U>(key, value);
            addNode(newNode);
            cache.Add(key, newNode);
            size++;
        }
    }

    private void moveToNewPosition(LFUNode<T,U> node)
    {
        LFUNode<T,U> nextNode = node.post;
        removeNode(node);
        while (nextNode.freq <= node.freq && nextNode != tail)
        {
            nextNode = nextNode.post;
        }
        nextNode.pre.post = node;
        node.pre = nextNode.pre;
        node.post = nextNode;
        nextNode.pre = node;
    }

    private void addNode(LFUNode<T,U> node)
    {
        node.post = head.post;
        node.pre = head;
        head.post.pre = node;
        head.post = node;
        moveToNewPosition(node);
    }

    private void removeNode(LFUNode<T,U> node)
    {
        node.pre.post = node.post;
        node.post.pre = node.pre;
    }
}

class LFUNode<T,U>
{
    public T key;
    public U value;
    public int freq = 1; //频率
    public LFUNode<T,U> pre; //上一个
    public LFUNode<T,U> post; //下一个

    public LFUNode() { }
    public LFUNode(T key, U value)
    {
        this.key = key;
        this.value = value;
    }
}