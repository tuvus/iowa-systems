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

    public AnimalSpeciesCarcass(int capacity) {
        carcassList = new OrganismList<Organism>(capacity, this);
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
            //organismActions.Enqueue(new OrganismAction(action, organismsToReproduce));
        }
    }


    public virtual void KillOrganismParallel(OrganismAction action) {
        carcassList.DeactivateActiveOrganismParallel(action.organism);
    }
}