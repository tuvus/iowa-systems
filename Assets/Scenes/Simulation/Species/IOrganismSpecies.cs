using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Species;

public interface IOrganismSpecies {

    /// <summary>
    /// Spawns a new Organism.
    /// </summary>
    /// <returns>The index of the new organism</returns>
    public Organism SpawnOrganism();

    /// <summary>
    /// Spawns a new organism within distance degrees of position
    /// </summary>
    /// <param name="position">The position to be randomised around</param>
    /// <param name="zone">The zone that the position is in</param>
    /// <param name="distance">The distance in degrees from the position</param>
    /// <returns>The index of the new organism or -1 if there is no available organism to spawn</returns>
    public Organism SpawnOrganism(float3 position, int zone, float distance);
}