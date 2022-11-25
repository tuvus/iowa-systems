using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// An action queue that resizes when the extended OrganismList resizes.
/// </summary>
/// <typeparam name="T">The action data in struct form</typeparam>
public class OrganismActionQueue<T> : IOrganismListExtender where T : struct {
    private NativeArray<T> actionArray;
    private int queueStartIndex;
    private int queueLength;

    IOrganismListExtender listExtender;

    public OrganismActionQueue(IOrganismListExtender extensionFrom) {
        this.actionArray = new NativeArray<T>(extensionFrom.GetListCapacity(), Allocator.Persistent);
        queueStartIndex = 0;
        queueLength = 0;
        extensionFrom.AddListExtender(this);
    }

    public void AddListExtender(IOrganismListExtender listExtender) {
        if (this.listExtender == null)
            this.listExtender = listExtender;
        else
            this.listExtender.AddListExtender(listExtender);
    }

    public int GetListCapacity() {
        return actionArray.Length;
    }

    public void Enqueue(T data) {
        int actionIndex = GetNewIndex();
        actionArray[actionIndex] = data;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    private int GetNewIndex() {
        if (queueStartIndex + queueLength >= GetListCapacity()) {
            queueLength++;
            return queueStartIndex + queueLength - 1 - GetListCapacity();
        } else {
            queueLength++;
            return queueStartIndex + queueLength - 1;
        }
    }

    public T Dequeue() {
        if (queueStartIndex == actionArray.Length - 1) {
            queueStartIndex = 0;
            queueLength--;
            return actionArray[actionArray.Length - 1];
        } else {
            queueLength--;
            queueStartIndex++;
            return actionArray[queueStartIndex - 1];
        }
    }

    public T Peek() {
        if (queueStartIndex == actionArray.Length - 1) {
            return actionArray[actionArray.Length - 1];
        } else {
            return actionArray[queueStartIndex];
        }
    }

    public bool Empty() {
        return queueLength == 0;
    }

    public void IncreaseOrganismListCapacity(int newCapacity) {
        NativeArray<T> oldActionArray = actionArray;
        actionArray = new NativeArray<T>(newCapacity, Allocator.Persistent);
        //Copy the remaining queue to the new array
        //The queue may have looped around the array so it might need to be copied in two parts.
        if (queueLength != 0) {
            int firstHalfLength = math.min(oldActionArray.Length - queueStartIndex, queueLength);
            NativeArray<T>.Copy(oldActionArray, queueStartIndex, actionArray, 0, firstHalfLength);
            if (firstHalfLength < queueLength) {
                NativeArray<T>.Copy(oldActionArray, 0, actionArray, firstHalfLength, oldActionArray.Length - firstHalfLength);
            }
        }
        oldActionArray.Dispose();
        if (listExtender != null)
            listExtender.IncreaseOrganismListCapacity(newCapacity);
    }

    public void Deallocate() {
        actionArray.Dispose();
        if (listExtender != null)
            listExtender.Deallocate();
    }
}
