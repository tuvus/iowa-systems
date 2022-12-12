using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

/// <summary>
/// A dynamic list that manages the active and inactive state of the organisms inside of it.
/// Also manages resizing when nessesary.
/// Can be extended by adding an OrganismListExtender, which will also be managed by the OrganismList.
/// </summary>
/// <typeparam name="T">Value to be stored in the list extension</typeparam>
public class OrganismList<T> : IOrganismListExtender where T : struct {
    public struct OrganismStatus {
        [Tooltip("Is the organism spawned or not")]
        public bool spawned;
        [Tooltip("The index of the organism in the array")]
        public int arrayIndex;
        [Tooltip("The max index of the organism in the activeSpeciesArray")]
        public int maxActiveOrganismIndex;

        public OrganismStatus(bool spawned, int arrayIndex, int maxActiveOrganismIndex) {
            this.spawned = spawned;
            this.arrayIndex = arrayIndex;
            this.maxActiveOrganismIndex = maxActiveOrganismIndex;
        }
    }

    [NativeDisableContainerSafetyRestriction] public NativeArray<T> organisms;
    [NativeDisableContainerSafetyRestriction] public NativeArray<OrganismStatus> organismStatuses;
    public NativeArray<int> activeOrganisms;
    public int activeOrganismCount;
    public NativeArray<int> inactiveOrganisms;
    public int inactiveOrganismCount;
    //An extention of values for the organism without having to add new active and inactive organism holders
    private IOrganismListExtender listExtender;
    private IOrganismListCapacityChange organismListHolder;

    /// <summary>
    /// Creates the OrganismList with a capacity of initialCapacity
    /// </summary>
    /// <param name="initialCapacity">The starting capacity of the list</param>
    public OrganismList(int initialCapacity, IOrganismListCapacityChange organismListHolder) {
        this.organismListHolder = organismListHolder;
        organisms = new NativeArray<T>(initialCapacity, Allocator.Persistent);
        organismStatuses = new NativeArray<OrganismStatus>(initialCapacity, Allocator.Persistent);
        activeOrganisms = new NativeArray<int>(initialCapacity, Allocator.Persistent);
        activeOrganismCount = 0;
        inactiveOrganisms = new NativeArray<int>(initialCapacity, Allocator.Persistent);
        inactiveOrganismCount = initialCapacity;
        for (int i = 0; i < organisms.Length; i++) {
            inactiveOrganisms[i] = i;
        }
    }

    /// <summary>
    /// Creates the OrganismList with a capacity of listExtenders capacity.
    /// Links listExtender to this OrganismList.
    /// </summary>
    /// <param name="listExtender">The parent list that should govern the size of this list</param>
    public OrganismList(IOrganismListExtender listExtender) : this(listExtender.GetListCapacity(), null) {
        listExtender.AddListExtender(this);
    }

    public void AddListExtender(IOrganismListExtender listExtender) {
        if (this.listExtender == null)
            this.listExtender = listExtender;
        else
            this.listExtender.AddListExtender(listExtender);
    }

    public int GetListCapacity() {
        return organisms.Length;
    }

    /// <summary>
    /// Gets an inactive organism, activates it and returns it.
    /// May change the capacity of the organism arrays
    /// </summary>
    /// <returns>A new active organism</returns>
    public int ActivateOrganism() {
        //Make sure that there are free inactive organisms to get
        if (inactiveOrganismCount == 0) {
            IncreaseOrganismListCapacity(organisms.Length * 2);
            organismListHolder.OnListUpdate();
        }
        //Get the first inactive organism and remove it from the inactiveOrganisms list
        int newOrganismIndex = inactiveOrganisms[inactiveOrganismCount - 1];
        inactiveOrganismCount--;
        //Add the organism to the activeOrganismsList
        activeOrganisms[activeOrganismCount] = newOrganismIndex;
        organismStatuses[newOrganismIndex] = new OrganismStatus(true, newOrganismIndex, activeOrganismCount);
        activeOrganismCount++;
        return newOrganismIndex;
    }

    /// <summary>
    /// Removes the organism from the active list and adds it to the inactive list.
    /// Resets the organism's data and sets it to not spawned.
    /// If the organism's index does not match the value of activeOrganisms at the organim's activeOrganismIndex
    /// then the deactivation has already occured and nothing will be done.
    /// </summary>
    /// <param name="organismIndex">The index of the organism</param>
    public void DeactivateActiveOrganism(int organismIndex) {
        //Check if the organism is still active
        if (!organismStatuses[organismIndex].spawned)
            return;
        //Finds the activeOrganismIndex starting at maxActiveOrganismIndex and works it's way to the begining.
        //Because of the way activeOrganisms are removed the index must be equal to or less than maxActiveOrganismIndex.
        int activeOrganismIndex = organismStatuses[organismIndex].maxActiveOrganismIndex;
        for (; activeOrganismIndex >= -1; activeOrganismIndex--) {
            if (activeOrganisms[activeOrganismIndex] == organismIndex)
                break;
        }
        //Remove the organism from the active list
        for (int i = activeOrganismIndex; i < activeOrganismCount - 1; i++) {
            activeOrganisms[i] = activeOrganisms[i + 1];
        }
        activeOrganismCount--;
        //Add the organism to the inactive list
        inactiveOrganisms[inactiveOrganismCount] = organismIndex;
        inactiveOrganismCount++;
        organismStatuses[organismIndex] = new OrganismStatus(false, organismIndex, -2);
    }

    /// <summary>
    /// Gets an inactive organism, activates it and returns it.
    /// Will not change the capacity of the organism arrays
    /// Can safely be used sycronously
    /// </summary>
    /// <returns>A new active organism or null if there is no free organism</returns>
    public int? ActivateOrganismSyncronous() {
        //Make sure that there are free inactive organisms to get
        if (inactiveOrganismCount <= 0)
            return null;
        //Get the first inactive organism and remove it from the inactiveOrganisms list
        int newOrganismIndex;
        int activeOrganismIndex;
        lock (this) {
            newOrganismIndex = inactiveOrganisms[Interlocked.Decrement(ref inactiveOrganismCount)];
            activeOrganismIndex = Interlocked.Increment(ref activeOrganismCount) - 1;
        }
        //newOrganismIndex could still be negative, if it is, leave it be and return null
        if (newOrganismIndex < 0)
            return null;
        //Add the organism to the activeOrganismsList
        activeOrganisms[activeOrganismIndex] = newOrganismIndex;
        organismStatuses[newOrganismIndex] = new OrganismStatus(true, newOrganismIndex, activeOrganismIndex);
        return newOrganismIndex;
    }

    /// <summary>
    /// Removes the organism from the active list and adds it to the inactive list.
    /// Resets the organism's data and sets it to not spawned.
    /// If the organism's index does not match the value of activeOrganisms at the organim's activeOrganismIndex
    /// then the deactivation has already occured and nothing will be done.
    /// </summary>
    /// <param name="organismIndex">The index of the organism</param>
    public void DeactivateActiveOrganismSyncronous(int organismIndex) {
        //Check if the organism is still active
        if (!organismStatuses[organismIndex].spawned)
            return;
        //Finds the activeOrganismIndex starting at maxActiveOrganismIndex and works it's way to the begining.
        //Because of the way activeOrganisms are removed the index must be equal to or less than maxActiveOrganismIndex.
        int activeOrganismIndex = organismStatuses[organismIndex].maxActiveOrganismIndex;
        for (; activeOrganismIndex >= -1; activeOrganismIndex--) {
            if (activeOrganisms[activeOrganismIndex] == organismIndex)
                break;
        }
        //Remove the organism from the active list
        for (int i = activeOrganismIndex; i < activeOrganismCount - 1; i++) {
            activeOrganisms[i] = activeOrganisms[i + 1];
        }
        activeOrganismCount--;
        //Add the organism to the inactive list
        inactiveOrganisms[inactiveOrganismCount] = organismIndex;
        inactiveOrganismCount++;
        organismStatuses[organismIndex] = new OrganismStatus(false, organismIndex, -2);
    }

    /// <summary>
    /// Increases the capacity of the organism arrays, active and inactive arrays.
    /// Also increases the capacity for all of the organs, plants and animals liked with it.
    /// </summary>
    /// <param name="newCapacity">The new capacity of the list</param>
    public void IncreaseOrganismListCapacity(int newCapacity) {
        NativeArray<T> oldOrganisms = organisms;
        organisms = new NativeArray<T>(newCapacity, Allocator.Persistent);
        NativeArray<T>.Copy(oldOrganisms, 0, organisms, 0, oldOrganisms.Length);
        oldOrganisms.Dispose();

        NativeArray<OrganismStatus> oldOrganismsStatus = organismStatuses;
        organismStatuses = new NativeArray<OrganismStatus>(newCapacity, Allocator.Persistent);
        NativeArray<OrganismStatus>.Copy(oldOrganismsStatus, 0, organismStatuses, 0, oldOrganismsStatus.Length);
        oldOrganismsStatus.Dispose();

        NativeArray<int> oldActiveOrganisms = activeOrganisms;
        activeOrganisms = new NativeArray<int>(newCapacity, Allocator.Persistent);
        NativeArray<int>.Copy(oldActiveOrganisms, 0, activeOrganisms, 0, oldActiveOrganisms.Length);
        oldActiveOrganisms.Dispose();

        NativeArray<int> oldInActiveOrganisms = inactiveOrganisms;
        inactiveOrganisms = new NativeArray<int>(newCapacity, Allocator.Persistent);
        NativeArray<int>.Copy(oldInActiveOrganisms, 0, inactiveOrganisms, 0, oldInActiveOrganisms.Length);
        //Add new inactiveOrganisms to the inactiveOrganismList and increment inactiveOrganismCount
        for (int i = oldInActiveOrganisms.Length; i < inactiveOrganisms.Length; i++) {
            inactiveOrganisms[i] = i;
        }
        inactiveOrganismCount += newCapacity - oldInActiveOrganisms.Length;
        oldInActiveOrganisms.Dispose();

        if (listExtender != null)
            listExtender.IncreaseOrganismListCapacity(newCapacity);
    }

    /// <summary>
    /// Deallocates all native collectiona and tells any OrganismListExtenders to do the same
    /// </summary>
    public void Deallocate() {
        organisms.Dispose();
        organismStatuses.Dispose();
        activeOrganisms.Dispose();
        inactiveOrganisms.Dispose();
        if (listExtender != null)
            listExtender.Deallocate();
    }
}