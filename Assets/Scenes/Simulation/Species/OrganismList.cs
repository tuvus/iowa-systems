using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
public class OrganismList<T> where T : struct {
    public struct OrganismStatus {
        [Tooltip("Is the organism spawned or not")]
        public bool spawned;
        [Tooltip("The index of the organism in the array")]
        public int arrayIndex;
        [Tooltip("The index of the organism in the activeSpeciesArray")]
        public int activeOrganismIndex;

        public OrganismStatus(bool spawned, int arrayIndex, int activeOrganismIndex) {
            this.spawned = spawned;
            this.arrayIndex = arrayIndex;
            this.activeOrganismIndex = activeOrganismIndex;
        }
    }

    [NativeDisableContainerSafetyRestriction] public NativeArray<T> organisms;
    [NativeDisableContainerSafetyRestriction] public NativeArray<OrganismStatus> organismStatuses;
    public NativeArray<int> activeOrganisms;
    //During update newly deactivated organisms may still have an activeOrganismIndex value
    //because they may be reactivated again in the same update.
    public int activeOrganismCount;
    private OrganismActionQueue<int> removeActiveOrganismQueue;
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
        // removeActiveOrganismQueue = new OrganismActionQueue<int>(this);
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
        int activeOrganismIndex = organismStatuses[organismIndex].activeOrganismIndex;
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
    /// Can safely be used parallel
    /// </summary>
    /// <returns>A new active organism or null if there is no free organism</returns>
    public int? ActivateOrganismParallel() {
        //Make sure that there are free inactive organisms to get
        //I left this in here as a duplicate check because it occures in a non parallel method
        if (inactiveOrganismCount <= 0)
            return null;
        //Get the first inactive organism and remove it from the inactiveOrganisms list
        int newOrganismIndex = 0;
        int? activeOrganismIndex = ManageActivateOrDeactivate(true, ref newOrganismIndex);
        //The inactiveOrganismCount may still be null, it must be checked in the parallel method as 
        if (!activeOrganismIndex.HasValue)
            return null;
        //newOrganismIndex could still be negative, if it is, leave it be and return null
        if (newOrganismIndex < 0)
            return null;
        //Add the organism to the activeOrganismsList
        activeOrganisms[activeOrganismIndex.Value] = newOrganismIndex;
        organismStatuses[newOrganismIndex] = new OrganismStatus(true, newOrganismIndex, activeOrganismIndex.Value);
        return newOrganismIndex;
    }

    /// <summary>
    /// Adds the organism to the inactive list and schedules it to be removed from the active list.
    /// The organism can be reactivated 
    /// </summary>
    /// <param name="organismIndex">The index of the organism</param>
    public void DeactivateActiveOrganismParallel(int organismIndex) {
        //Check if the organism is still active
        if (!organismStatuses[organismIndex].spawned)
            return;
        int activeOrganismIndex = organismStatuses[organismIndex].activeOrganismIndex;
        //Despawn the organism but maintain the activeOrganismIndex
        organismStatuses[organismIndex] = new OrganismStatus(false, organismIndex, activeOrganismIndex);
        ManageActivateOrDeactivate(false, ref organismIndex);
        //Schedule removeal the organism from the active list
        removeActiveOrganismQueue.Enqueue(activeOrganismIndex);
    }

    /// <summary>
    /// This method has two functions depending on the value of activateOrDeactivate.
    /// The method manages two functions in order to apply the Syncronized property correctly.
    /// 
    /// If activateOrDeactivate is true, this method removes the last inactiveOrganism from the inactiveOrganismIndex
    /// returns the new organism index. This method does NOT add the organism to the activeOrganism, 
    /// it only reserves a spot for it.
    /// 
    /// If activateOrDeactivate is false, this method adds the organism to the inactiveOrganism list.
    /// This method does Not remove it from the activeOrganism list or change any other values
    /// </summary>
    /// <param name="activateOrDeactivate"></param>
    /// <param name="activeOrDeactiveOrganismIndex"></param>
    /// <returns>
    ///  </returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    private int? ManageActivateOrDeactivate(bool activateOrDeactivate, ref int activeOrganismIndexOrOrganismIndex) {
        if (activateOrDeactivate) {
            //There may be no inactiveOrganisms to use, if so return null
            if (inactiveOrganismCount <= 0)
                return null;
            int organismIndex = inactiveOrganisms[Interlocked.Decrement(ref inactiveOrganismCount)];
            //If the organism has not yet been removed from the activeOrganism index, use the previous activeOrganism index.
            int tempActiveOrganismIndex = organismStatuses[organismIndex].activeOrganismIndex;
            if (tempActiveOrganismIndex == -1) {
                activeOrganismIndexOrOrganismIndex = Interlocked.Increment(ref activeOrganismCount) - 1;
            } else {
                activeOrganismIndexOrOrganismIndex = tempActiveOrganismIndex;
            }
            return organismIndex;
        } else {
            int inactiveOrganismIndex = Interlocked.Increment(ref inactiveOrganismCount);
            inactiveOrganisms[inactiveOrganismIndex] = activeOrganismIndexOrOrganismIndex;
            return 0;
        }
    }

    /// <summary>
    /// Deques the removeActiveOrganismQueue one by one.
    /// Removes all activeOrganisms that are no longer being used on the end.
    /// If the activeOrgnismIndex from the removeActiveOrganismQueue was not removed,
    /// replaces the value at activeOrganismIndex with the value at the end of the list.
    /// Otherwise it has already been removed from the end.
    /// </summary>
    public void CleanActiveOrganismList() {
        while (!removeActiveOrganismQueue.Empty()) {
            int activeOrganismIndex = removeActiveOrganismQueue.Dequeue();
            //Check if the activeOrganism was removed when previously removing an activeOrganism
            if (activeOrganismIndex >= activeOrganismCount)
                return;
            //Remove activeOrganisms until there is a valid one to move in the middle of the array.
            while (!organismStatuses[activeOrganisms[activeOrganismCount - 1]].spawned) {
                organismStatuses[activeOrganisms[activeOrganismCount - 1]] = new OrganismStatus(false, organismStatuses[activeOrganisms[activeOrganismCount - 1]].arrayIndex,-1);
                activeOrganismCount--;
            }
            //Check if the activeOrganism was removed when removing the unused activeOrganisms
            if (activeOrganismIndex >= activeOrganismCount)
                return;
            //Replace this activeOrganism index with the one at the end
            activeOrganisms[activeOrganismIndex] = activeOrganisms[activeOrganismCount - 1];
            activeOrganismCount--;
        }
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