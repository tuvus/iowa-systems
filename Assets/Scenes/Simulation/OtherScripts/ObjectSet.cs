
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ObjectSet manages two sets, one of active objects that can be read from and one that can only be written to.
/// This allows for concurrent reading and concurrent writing as long as the reads do not depend on the writes until they are switched.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectSet<T> where T : SetObject{
    public int Count => readObjects.Count;

    public HashSet<T> readObjects = new();
    public HashSet<T> writeObjects = new();
    private List<T> toRemove = new();
    private List<(T obj, T obj2)> toAdd = new();
    
    public event Action<T> onRemove = delegate { };

    public bool IsReadOnly => false;

    /// <summary>
    /// Switches the readObject and writeObjects hashsets.
    /// </summary>
    public void SwitchObjectSets() {
        var temp = readObjects;
        readObjects = writeObjects;
        writeObjects = temp;
        toRemove.ForEach(r => writeObjects.Remove(r));
        toRemove.Clear();
        toAdd.ForEach(a => writeObjects.Add(a.obj2));
        toAdd.Clear();
    }

    public void Add(T obj, T obj2) {
        if (obj == null || obj2 == null) throw new NullReferenceException("Trying to add a null object!"); 
        obj2.LinkObject(obj);
        writeObjects.Add(obj);
        toAdd.Add((obj, obj2));
    }

    public T GetWritable(T obj) {
        if (obj.copy != null && writeObjects.Contains((T)obj.copy))
            return (T)obj.copy;
        // The organism might have died already
        return null;
    }

    public void Clear() {
        writeObjects.Clear();
    }

    public void CopyTo(T[] array, int arrayIndex) {
        readObjects.CopyTo(array, arrayIndex);
    }
    
    public bool Remove(T obj) {
        if (!writeObjects.Contains(obj)) return false;
        writeObjects.Remove(obj);
        toRemove.Add(obj);
        onRemove(obj);
        return true;
    }

    public bool Contains(T obj) {
        return readObjects.Contains(obj);
    }
}

public abstract class SetObject {

    public ulong id { get; private set; }
    internal SetObject copy;

    public SetObject() {
        id = GetNewId();
    }

    public void LinkObject(SetObject copy) {
        id = copy.id;
        this.copy = copy;
        copy.copy = this;
    }
    
    private static ulong nextId = 0;

    private static ulong GetNewId() {
        nextId++;
        return nextId;
    }

    public override int GetHashCode() {
        return (int)id;
    }

    public override bool Equals(object obj) {
        if (obj == null) return false;
        if (obj is not SetObject) return false;
        return id == ((SetObject)obj).id;
    }
}