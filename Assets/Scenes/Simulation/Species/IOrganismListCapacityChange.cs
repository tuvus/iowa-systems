using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOrganismListCapacityChange {

    /// <summary>
    /// Called after OrganismList resize to update any pointers to the newly made list.
    /// </summary>
    public void OnListUpdate();
}