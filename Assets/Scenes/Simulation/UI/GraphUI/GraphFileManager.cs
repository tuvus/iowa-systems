using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphFileManager : MonoBehaviour {
    private GraphFile populationFile;

    public void SetupFileManager(int speciesCount, Color32[] speciesColors) {
        populationFile = new GraphFile("Population", speciesCount, speciesColors);
    }

    public void AddPointsToFile(GraphFile file, int[] points) {
        if (points.Length <= 0)
            return;
        file.AddPoints(points);
    }

    public GraphFile GetPopulationFile() {
        return populationFile;
    }
}
