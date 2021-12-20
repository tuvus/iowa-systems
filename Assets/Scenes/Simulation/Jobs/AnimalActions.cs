

public struct AnimalActions {
    public enum ActionType {
        RunFromPredator = 1,
        EatFood = 2,
        GoToFood = 3,
        AttemptReproduction = 4,
        AttemptToMate = 5,
        Explore = 6,
        Idle = 7,
    }
    public AnimalActions(ActionType actionType, int index = -1) {
        this.actionType = actionType;
        this.index = index;
    }

    public ActionType actionType;
    public int index;
}
