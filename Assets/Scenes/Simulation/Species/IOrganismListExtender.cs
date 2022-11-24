/// <summary>
/// Attach to a class that should hold an array of values for an organism.
/// The class can then be an extention of an OrganismList.
/// Resizing, adding more extentions, and deallocation will be managed by the OrganismList.
/// </summary>
public interface IOrganismListExtender {
    /// <summary>
    /// Returns the current capacity of the OrganismList
    /// </summary>
    /// <returns>The current capacity of the OrganismList</returns>
    public int GetListCapacity();

    /// <summary>
    /// Adds an OrganismListExtender to the OrganismListChain
    /// </summary>
    /// <param name="listExtender">The new OrganismListExtender to add</param>
    public void AddListExtender(IOrganismListExtender listExtender);

    /// <summary>
    /// Resizes the OrganismList and all of its OrganismListExtender to the new capacity
    /// </summary>
    /// <param name="newCapacity">The new ammount to resize to</param>
    public void IncreaseOrganismListCapacity(int newCapacity);

    /// <summary>
    /// Deallocates all of the Native collections in the OrganismList and all OrganismListExtenders
    /// </summary>
    public void Deallocate();
}