using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;

public class OrganismAtribute<T> : IOrganismListExtender where T : struct {
    [NativeDisableContainerSafetyRestriction] public NativeArray<T> organismAttributes;
    IOrganismListExtender listExtender;

    public OrganismAtribute(IOrganismListExtender extensionFrom) {
        extensionFrom.AddListExtender(this);
        organismAttributes = new NativeArray<T>(extensionFrom.GetListCapacity(), Allocator.Persistent);
    }

    public void AddListExtender(IOrganismListExtender listExtender) {
        if (this.listExtender == null)
            this.listExtender = listExtender;
        else
            this.listExtender.AddListExtender(listExtender);
    }

    public int GetListCapacity() {
        return organismAttributes.Length;
    }

    public void IncreaseOrganismListCapacity(int newCapacity) {
        NativeArray<T> oldOrganismAttributes = organismAttributes;
        organismAttributes = new NativeArray<T>(newCapacity, Allocator.Persistent);
        NativeArray<T>.Copy(oldOrganismAttributes, 0, organismAttributes, 0, oldOrganismAttributes.Length);
        oldOrganismAttributes.Dispose();
        if (listExtender != null)
            listExtender.IncreaseOrganismListCapacity(newCapacity);
    }

    public void Deallocate() {
        organismAttributes.Dispose();
        if (listExtender != null)
            listExtender.Deallocate();
    }
}