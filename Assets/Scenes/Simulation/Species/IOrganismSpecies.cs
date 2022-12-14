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
    public int SpawnOrganism();

    /// <summary>
    /// Spawns a new organism within distance degrees of position
    /// </summary>
    /// <param name="position">The position to be randomised around</param>
    /// <param name="zone">The zone that the position is in</param>
    /// <param name="distance">The distance in degrees from the position</param>
    /// <returns>The index of the new organism or -1 if there is no available organism to spawn</returns>
    public int SpawnOrganism(float3 position, int zone, float distance);

    /// <summary>
    /// Spawns up to action.ammount new organisms with with action.floatvalue distance 
    /// around action.position.
    /// If the organisms can't be spawned it adds the remaining organisms to organismActions
    /// </summary>
    /// <param name="action">The place, ammount and distance from place to reproduce</param>
    public void ReproduceOrganismParallel(OrganismAction action);

    /// <summary>
    /// Kills the organism
    /// </summary>
    /// <param name="action">The organism to kill</param>
    public void KillOrganismParallel(OrganismAction action);
}