using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using static Species;

public class AnimalSpeciesCarcass : AnimalSpeciesOrgan, IOrganismSpecies {
    [Tooltip("The time it takes a corpse to deteriorate in days")]
    public float deteriationTime;

    public OrganismList<Organism> carcassList;
    public NativeArray<Organism> carcasses;

    public AnimalSpeciesCarcass() {

    }

    public override void SetupSpeciesOrganArrays(IOrganismListExtender listExtender) {
        carcassList = new OrganismList<Organism>(listExtender.GetListCapacity(), this);
        carcasses = carcassList.organisms;
    }

    public int SpawnOrganism() {
        int? organism = carcassList.ActivateOrganismParallel();
        if (!organism.HasValue)
            return -1;
        carcasses[organism.Value] = new Organism(0, 0, float3.zero, 0);
        //TODO: Add position and rotation
        return organism.Value;
    }

    public int SpawnOrganism(float3 position, int zone, float distance) {
        int? organism = carcassList.ActivateOrganismParallel();
        if (!organism.HasValue)
            return -1;
        carcasses[organism.Value] = new Organism(0, zone, position, 0);
        //TODO: Add position and rotation
        return organism.Value;
    }

    public override void OnListUpdate() {
        carcasses = carcassList.organisms;
    }

    public void ReproduceOrganismParallel(OrganismAction action) {
        int organismsToReproduce = action.amount;
        for (; organismsToReproduce > 0; organismsToReproduce--) {
            if (SpawnOrganism(action.position, action.amount, action.floatValue) == -1) {
                organismsToReproduce--;
                break;
            }
        }
        if (organismsToReproduce > 0) {
            //Doesen't have to deal with spawning more than the capacity
            //organismActions.Enqueue(new OrganismAction(action, organismsToReproduce));
            Debug.LogError("There where leftover organisms to reproduce. This should not have occured");
        }
    }


    public virtual void KillOrganismParallel(OrganismAction action) {
        carcassList.DeactivateActiveOrganismParallel(action.organism);
    }

    public override void Deallocate() {
        carcassList.Deallocate();
    }
}