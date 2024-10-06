using System;
using System.Collections;
using System.Collections.Generic;

public class ObjectMap<O,T> where O : SetObject where T : MapObject<O> {
    public Dictionary<O,T> readObjects = new();
    public Dictionary<O,T> writeObjects = new();
    private List<O> toRemove = new();
    private List<(T obj, T obj2)> toAdd = new();

    public bool IsReadOnly => false;

    public ObjectMap(ObjectSet<O> parentSet) {
        parentSet.onRemove += r => Remove(r);
    }

    /// <summary>
    /// Switches the readObject and writeObjects hashsets.
    /// </summary>
    public void SwitchObjectSets() {
        var temp = readObjects;
        readObjects = writeObjects;
        writeObjects = temp;
        toRemove.ForEach(r => writeObjects.Remove(r));
        toRemove.Clear();
        toAdd.ForEach(a => writeObjects.Add(a.obj2.setObject,a.obj2));
        toAdd.Clear();
    }

    public void Add(T obj, T obj2) {
        if (obj == null || obj2 == null) throw new NullReferenceException("Trying to add a null object!"); 
        obj2.LinkObject(obj);
        writeObjects.Add(obj.setObject, obj);
        toAdd.Add((obj, obj2));
        if (!Equals(obj, GetWritable(obj.setObject))) throw new Exception("asfasjfdsaf");
    }

    public T GetReadable(O obj) {
        if (obj == null) return null;
        return readObjects[obj];
    }

    public T GetWritable(O obj) {
        if (obj == null) return null;
        return writeObjects[obj];
    }

    public void Clear() {
        writeObjects.Clear();
    }

    public void CopyTo(T[] array, int arrayIndex) {
        readObjects.Values.CopyTo(array, arrayIndex);
    }

    public bool Remove(O obj) {
        if (obj == null) return false;
        if (!writeObjects.ContainsKey(obj)) return false;
        writeObjects.Remove(obj);
        toRemove.Add(obj);
        return true;
    }
    
    public bool Remove(T obj) {
        return obj != null && Remove(obj.setObject);
    }

    public bool Contains(T obj) {
        return obj != null && readObjects.ContainsKey(obj.setObject);
    }
}

public class MapObject<O> {
    public readonly O setObject;
    internal MapObject<O> copy;

    public void LinkObject(MapObject<O> copy) {
        this.copy = copy;
        copy.copy = this;
    }

    public MapObject(O setObject) {
        this.setObject = setObject;
    }
}