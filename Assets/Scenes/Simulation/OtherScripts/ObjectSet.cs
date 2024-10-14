
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public interface IReadWriteGroup {
    bool GetReadWriteGroup();
}

/// <summary>
/// ObjectSet manages two sets, one of active objects that can be read from and one that can only be written to.
/// This allows for concurrent reading and concurrent writing as long as the reads do not depend on the writes until they are switched.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectSet<T> : IReadWriteGroup where T : SetObject {
    public int Count => readObjects.Count;

    public HashSet<T> readObjects = new();
    public HashSet<T> writeObjects = new();
    private List<T> toRemove = new();
    private List<T> toAdd = new();
    /// <summary> Determines which objects are readable and are in teh readObjects set, all other objects are writable and in the writeObjects set </summary>
    private bool rwGroup = false;
    
    public event Action<T> onRemove = delegate { };

    public bool IsReadOnly => false;

    /// <summary>
    /// Switches the readObject and writeObjects hashsets.
    /// </summary>
    public void SwitchObjectSets() {
        var temp = readObjects;
        toAdd.ForEach(a => {
            a.SetReadWriteGroup(this);
            writeObjects.Add(a);
        });
        toRemove.ForEach(r => writeObjects.Remove(r));
        readObjects = writeObjects;
        writeObjects = temp;
        rwGroup = !rwGroup;
        toRemove.ForEach(r => writeObjects.Remove(r));
        toRemove.Clear();
        toAdd.ForEach(ApplyAdd);
        toAdd.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Add(T obj) {
        if (obj == null) throw new NullReferenceException("Trying to add a null object!");
        toAdd.Add(obj);
    }

    private void ApplyAdd(T obj) {
        var clone = (T)obj.Clone();
        clone.LinkObject(obj);
        writeObjects.Add(clone);
    }

    public T GetWritable(T obj) {
        if (obj == null) throw new NullReferenceException("Trying to get a null writable object!");
        if (obj.copy != null)
            return (T)obj.GetWritable();
        // The organism might have died already
        return null;
    }

    public void Clear() {
        writeObjects.Clear();
    }

    public void CopyTo(T[] array, int arrayIndex) {
        readObjects.CopyTo(array, arrayIndex);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool Remove(T obj) {
        if (!writeObjects.Contains(obj)) return false;
        toRemove.Add((T)obj.GetWritable());
        onRemove(obj);
        return true;
    }

    public bool Contains(T obj) {
        return readObjects.Contains(obj);
    }

    public bool GetReadWriteGroup() {
        return rwGroup;
    }
}

public abstract class SetObject : ICloneable {

    public ulong id { get; private set; }
    internal IReadWriteGroup rwGroupSet;
    internal SetObject copy;
    internal bool rwGroup;

    public SetObject() {
        id = GetNewId();
        // Objects will always be added to the true rwGroup
        rwGroup = true;
    }

    public void LinkObject(SetObject copy) {
        id = copy.id;
        this.copy = copy;
        copy.copy = this;
        // Copies will always be added to the false rwGroup
        rwGroup = false;
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

    public abstract object Clone();

    public SetObject GetReadable() {
        return rwGroup == rwGroupSet.GetReadWriteGroup() ? this : copy;
    }

    public SetObject GetWritable() {
        return rwGroup == rwGroupSet.GetReadWriteGroup() ? copy : this;
    }

    internal void SetReadWriteGroup(IReadWriteGroup group) {
        rwGroupSet = group;
    }
}