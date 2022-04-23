using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Column : MonoBehaviour {
    private GraphWindow graph;
    private List<RectTransform> points;
    private List<RectTransform> lines;
    private Text text;
    private RectTransform verticalLine;
    private RectTransform pointsTransform;
    private RectTransform linesTransform;

    public void SetupColumn(GraphWindow graph) {
        this.graph = graph;
        points = new List<RectTransform>(10);
        lines = new List<RectTransform>(10);
        text = transform.GetChild(0).GetComponent<Text>();
        verticalLine = transform.GetChild(1).GetComponent<RectTransform>();
        pointsTransform = transform.GetChild(2).GetComponent<RectTransform>();
        linesTransform = transform.GetChild(3).GetComponent<RectTransform>();
    }

    public RectTransform GetPoint(int index) {
        if (index >= points.Count) {
            points.Add(Instantiate(graph.GetPopulationCirclePrefab(), pointsTransform).GetComponent<RectTransform>());
            points[index].anchoredPosition = new Vector2(2.5f, 0);
            points[index].sizeDelta = new Vector2(5, 5);
        }
        return points[index];
    }

    public RectTransform GetLine(int index) {
        if (index >= lines.Count) {
            lines.Add(Instantiate(graph.GetPopulationLinePrefab(), linesTransform).GetComponent<RectTransform>());
        }
        return lines[index];
    }

    public Text GetText() {
        return text;
    }

    public RectTransform GetVerticalLine() {
        return verticalLine;
    }

    public int GetPointsCount() {
        return points.Count;
    }

    public int GetLinesCount() {
        return lines.Count;
    }

}
