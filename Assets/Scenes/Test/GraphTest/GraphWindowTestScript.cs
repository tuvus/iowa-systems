using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphWindowTestScript : MonoBehaviour {
    [SerializeField]
    private GameObject horizontalRow;
    [SerializeField]
    private GameObject verticalColumn;
    [SerializeField]
    private GameObject populationCircle;
    [SerializeField]
    private GameObject populationLine;
    private RectTransform graphContainer;
    public int yMaximum;
    List<RectTransform> rows;
    List<Column> columns;
    GraphFileTest graphFileManager;

    public int refreshTime;
    public int rowCount;
    public int columnDisplayCount;
    public float textDistance;
    private float defaultColumnWidth;

    float distanceFromTopRight = 30;
    float distanceFromBottomLeft = 35;

    private void Awake() {
        SetupGraph();
    }

    public void SetupGraph() {
        defaultColumnWidth = verticalColumn.GetComponent<RectTransform>().rect.width;
        graphFileManager = GetComponent<GraphFileTest>();
        graphContainer = GetGraphTransform().GetChild(0).GetChild(0).GetComponent<RectTransform>();
        yMaximum = Mathf.CeilToInt(yMaximum / 10.0f) * 10;
        SetYAxisLabel(yMaximum);
        SetXAxisLabel(refreshTime);
        GetColumnTransform().localPosition = new Vector2(0, distanceFromBottomLeft + GetColumnTransform().localPosition.y);
        rows = new List<RectTransform>(rowCount);
        columns = new List<Column>(20);
        columnDisplayCount = (int)(GetGraphWidth() / defaultColumnWidth);
        GetYAxisSlider().value = columnDisplayCount;
        GetYAxisSlider().minValue = columnDisplayCount;
        GetYAxisSlider().maxValue = columnDisplayCount * 50;
        graphFileManager.SetupGraphFileManager(3);
    }

    private void Start() {
        int[] values = new int[3];
        for (int i = 0; i < values.Length; i++) {
            values[i] = Random.Range(0, 100);
        }
        for (int i = 0; i < Random.Range(100, 300); i++) {
            for (int f = 0; f < values.Length; f++) {
                values[f] = Mathf.Max(0, values[f] + Random.Range(-10, 10));
            }
            graphFileManager.AddPoints(values);
        }
        DisplayGraph();
    }

    private void LateUpdate() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            graphFileManager.AddPoints(new int[] { Random.Range(0, 100), Random.Range(0, 100), Random.Range(0, 100) });
            RefreshGraph();
        }
    }
    #region GraphControls
    public void DisplayGraph() {
        RefreshGraphPoints();
        RefreshColumnWidth();
        RefreshRows(rowCount, 1);
        RefreshColumnPointLines();
        RefreshRowLines();
        RefreshColumnText();
        GetXAxisSlider().maxValue = Mathf.Max(GetColumnTransform().rect.width - GetGraphTransform().rect.width + distanceFromBottomLeft, 0);
        RefreshGraphXSlider();
        RefreshColumnWidth();
    }

    public void RefreshGraph() {
        RefreshGraphPoints();
        RefreshColumnWidth();
        RefreshRows(rowCount, 1);
        RefreshColumnPointLines();
        RefreshRowLines();
        RefreshColumnText();
        RefreshGraphXSlider();
    }

    /// <summary>
    /// Refreshes the xAxisSlider and sets the new position of the slider and graph
    /// If the slider is at the end when the max is changed it will stick to the end of the slider
    /// </summary>
    void RefreshGraphXSlider() {
        bool atEndOfGraph = false;
        if (GetXAxisSlider().value == GetXAxisSlider().maxValue)
            atEndOfGraph = true;
        GetXAxisSlider().maxValue = Mathf.Max(GetColumnTransform().rect.width - GetGraphTransform().rect.width + distanceFromBottomLeft, 0);
        if (atEndOfGraph)
            GetXAxisSlider().SetValueWithoutNotify(GetXAxisSlider().maxValue);
        UpdateGraphXSlider();
    }

    /// <summary>
    /// Updates the size of the graph and refreshes the graph
    /// </summary>
    public void UpdateGraphYSlider() {
        columnDisplayCount = (int)GetYAxisSlider().value;
        RefreshColumnWidth();
        RefreshColumnPointLines();
        RefreshRowLines();
        RefreshColumnText();
        RefreshGraphXSlider();
    }

    /// <summary>
    /// If population is greater than yMaximum refreshes the yMaximum value and sets the YAxisLabel to the new yMaximum 
    /// </summary>
    /// <param name="population"></param>
    public void SetPopulationMax(int population) {
        if (population > yMaximum) {
            yMaximum = Mathf.CeilToInt(population / 10.0f) * 10;
            SetYAxisLabel(yMaximum);
        }
    }
    #endregion

    #region GraphMethods
    /// <summary>
    /// Sets the population max to the file max
    /// Get the sets of points from the file and sets thier position
    /// Hides the unused columns and points
    /// Makes sure there is a column for each set of points
    /// </summary>
    void RefreshGraphPoints() {
        SetPopulationMax(graphFileManager.GetMax());
        int graphSize = graphFileManager.GetGraphSize();
        for (int i = 0; i < graphSize; i++) {
            int[] points = new int[graphFileManager.GetPointSize()];
            graphFileManager.GetPoints(points, i);
            SetGraphColumn(points, GetColumn(i));
        }
    }

    /// <summary>
    /// Gets the graph position on the graph from the population value
    /// </summary>
    /// <param name="population">Population value of the position</param>
    /// <returns>Position that is representitive of the population</returns>
    float FindGraphPosition(int population) {
        return (float)population / yMaximum * GetGraphHight();
    }

    /// <summary>
    /// Returns the column at the specified index,
    /// if the column does not exist creates a new column
    /// </summary>
    /// <param name="column">The index of the column</param>
    /// <returns>Returns the column at the specified index</returns>
    Column GetColumn(int column) {
        if (column >= columns.Count) {
            RectTransform newCollum = Instantiate(verticalColumn, GetColumnTransform()).GetComponent<RectTransform>();
            columns.Add(newCollum.GetComponent<Column>());
            Debug.Log("Test version of graph window, please change column GraphWindow types to GraphWindowTest");
            //columns[column].SetupColumn(this);
            columns[column].GetText().text = newCollum.transform.GetSiblingIndex().ToString();
            columns[column].GetVerticalLine().sizeDelta = new Vector2(columns[column].GetVerticalLine().rect.width, GetGraphHight());
            columns[column].GetVerticalLine().anchoredPosition = new Vector2(0, graphContainer.rect.height / 2 - distanceFromBottomLeft + 2.75f);
            GetColumnTransform().sizeDelta += new Vector2(defaultColumnWidth, 0);
        }
        return columns[column];
    }

    /// <summary>
    /// Returns the species color corespointing to the index
    /// </summary>
    /// <param name="speciesIndex">The index of the species</param>
    /// <returns>Returns the species color corespointing to the index</returns>
    public Color GetSpeciesColor(int speciesIndex) {
        if (speciesIndex == 0)
            return new Color(1, 0, 0, 1);
        if (speciesIndex == 1)
            return new Color(0, 1, 0, 1);
        if (speciesIndex == 2)
            return new Color(0, 0, 1, 1);
        return new Color(1, 1, 1, 1);
    }

    #region Columns
    /// <summary>
    /// Refreshes the column text to only be displayed on each column 
    /// if the distance to the last dispayed column is greater than textDistance
    /// column 0 is always displayed
    /// </summary>
    void RefreshColumnText() {
        float columnWidth = GetColumnWidth();
        int count = int.MaxValue;
        for (int i = 0; i < columns.Count; i++) {
            if (count * columnWidth >= textDistance) {
                GetColumn(i).GetText().enabled = true;
                count = 0;
                count++;
            } else {
                GetColumn(i).GetText().enabled = false;
                count++;
            }
        }
    }

    /// <summary>
    /// Sets the spacing of the horizontalLayoutGroup to account for the new column width
    /// Sets the position of the column transform to account for it's new width
    /// </summary>
    void RefreshColumnWidth() {
        GetColumnTransform().GetComponent<HorizontalLayoutGroup>().spacing = GetColumnWidth() - defaultColumnWidth;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetColumnTransform());
        GetColumnTransform().localPosition = new Vector2(GetColumnTransform().rect.width / 2, GetColumnTransform().localPosition.y);
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetColumnTransform());
    }

    /// <summary>
    /// Refreshed the lines in between each point to account for thier new graph positions
    /// </summary>
    void RefreshColumnPointLines() {
        for (int i = 0; i < columns.Count; i++) {
            for (int j = 0; j < GetColumn(i).GetLinesCount(); j++) {
                RectTransform line = GetColumn(i).GetLine(j);
                if (i == 0 || i >= graphFileManager.GetGraphSize() || j >= graphFileManager.GetPointSize()) {
                    line.gameObject.SetActive(false);
                } else {
                    line.gameObject.SetActive(true);
                    line.GetComponent<Image>().color = GetSpeciesColor(j);
                    Vector2 fromPos = GetColumn(i - 1).GetPoint(j).position;
                    Vector2 toPos = GetColumn(i).GetPoint(j).position;
                    line.sizeDelta = new Vector2(Vector2.Distance(fromPos, toPos), line.sizeDelta.y);
                    line.eulerAngles = new Vector3(0, 0, (Mathf.Atan2(toPos.y - fromPos.y, toPos.x - fromPos.x) * 180 / Mathf.PI));
                    line.position = new Vector2((fromPos.x + toPos.x) / 2, (fromPos.y + toPos.y) / 2);
                }
            }
        }
    }

    /// <summary>
    /// Sets the points in the column to thier color and position
    /// Hides the unused points and lines
    /// Makes sure there is a point and a line for each value
    /// </summary>
    /// <param name="points"></param>
    /// <param name="column"></param>
    void SetGraphColumn(int[] points, Column column) {
        for (int i = 0; i < points.Length; i++) {
            column.GetPoint(i).gameObject.SetActive(true);
            column.GetLine(i).gameObject.SetActive(true);
            column.GetPoint(i).GetComponent<Image>().color = GetSpeciesColor(i);
            column.GetPoint(i).localPosition = new Vector2(0, FindGraphPosition(points[i]));
        }
        for (int i = points.Length + 1; i < column.GetPointsCount(); i++) {
            column.GetPoint(i).gameObject.SetActive(false);
            column.GetLine(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Returns the width of the graph that points and lines use
    /// </summary>
    /// <returns>Returns the width of the graph that points and lines use</returns>
    float GetGraphWidth() {
        return GetGraphTransform().rect.width - distanceFromBottomLeft - distanceFromTopRight;
    }

    /// <summary>
    /// Returns width of each column
    /// </summary>
    /// <returns>Returns width of each column</returns>
    float GetColumnWidth() {
        return GetGraphWidth() / columnDisplayCount;
    }

    /// <summary>
    /// Refreshes the graphs position relative to the slider
    /// </summary>
    public void UpdateGraphXSlider() {
        GetColumnTransform().localPosition = new Vector2(-GetXAxisSlider().value + GetColumnTransform().rect.width / 2, GetColumnTransform().localPosition.y);
    }

    /// <summary>
    /// Sets the yAxis text to the time in hours/days/weeks/months/years
    /// </summary>
    /// <param name="time">The time that each column represents</param>
    private void SetXAxisLabel(int time) {
        if (time < 24)
            GetXAxisText().text = "Time(" + (int)time + "Hours)";
        else if (time < 168)
            GetXAxisText().text = "Time(" + (int)(time * 10.0f / 24) / 10.0f + "Days)";
        else if (time < 720)
            GetXAxisText().text = "Time(" + (int)(time * 10.0f / 168) / 10.0f + "Weeks)";
        else if (time < 8640)
            GetXAxisText().text = "Time(" + (int)(time * 10.0f / 720) / 10.0f + "Months)";
        else
            GetXAxisText().text = "Time(" + (int)(time * 10.0f / 8640) / 10.0f + "Years)";
    }
    #endregion

    #region Rows
    /// <summary>
    /// Refreshes the row count and position reletive to yMaximum
    /// Rowcount must be greater than 1
    /// </summary>
    /// <param name="rowCount"></param>
    /// <param name="roundTo"></param>
    void RefreshRows(int rowCount, int roundTo) {
        for (int i = 0; i < rowCount; i++) {
            GetRow(i).gameObject.SetActive(true);
            int rowValue = Mathf.RoundToInt(yMaximum * i / (rowCount - 1) / roundTo) * roundTo;
            float rowPosition = distanceFromBottomLeft;
            if (i != 0)
                rowPosition += FindGraphPosition(rowValue);
            GetRow(i).localPosition = new Vector2(10, rowPosition);
            GetRow(i).GetChild(0).GetComponent<Text>().text = rowValue.ToString();
        }
        for (int i = rowCount + 1; i < rows.Count; i++) {
            GetRow(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Refreshes the width of each of the row lines to the width of the columns
    /// </summary>
    void RefreshRowLines() {
        for (int i = 0; i < rows.Count; i++) {
            RectTransform lineRect = rows[i].GetChild(1).GetComponent<RectTransform>();
            lineRect.sizeDelta = new Vector2(GetColumnTransform().sizeDelta.x, lineRect.rect.height);
            lineRect.localPosition = new Vector2(lineRect.sizeDelta.x / 2 + 15, 0);
        }
    }

    /// <summary>
    /// Gets the row coresponding to the index
    /// If the row does not exist, creates a new row
    /// </summary>
    /// <param name="index">Index of the row</param>
    /// <returns>The row at the index</returns>
    RectTransform GetRow(int index) {
        if (index >= rows.Count) {
            GameObject newRow = Instantiate(horizontalRow, GetRowTransform());
            rows.Add(newRow.GetComponent<RectTransform>());
        }
        return rows[index];
    }

    /// <summary>
    /// Returns the height of the graph that points and lines use
    /// </summary>
    /// <returns>Returns the height of the graph that points and lines use</returns>
    float GetGraphHight() {
        return graphContainer.rect.height - distanceFromTopRight - distanceFromBottomLeft;
    }

    /// <summary>
    /// Sets the xAxis text to the population max
    /// </summary>
    /// <param name="max"></param>
    public void SetYAxisLabel(int max) {
        GetYAxisText().text = "Population(Max:" + max + ")";
    }
    #endregion
    #endregion

    #region GetObjects
    private RectTransform GetGraphTransform() {
        return transform.GetChild(1).GetComponent<RectTransform>();
    }

    private RectTransform GetColumnTransform() {
        return graphContainer.GetChild(1).GetChild(0).GetComponent<RectTransform>();
    }

    private RectTransform GetRowTransform() {
        return graphContainer.GetChild(0).GetComponent<RectTransform>();
    }

    private Text GetXAxisText() {
        return transform.GetChild(2).GetChild(0).GetComponent<Text>();
    }

    private Slider GetXAxisSlider() {
        return transform.GetChild(2).GetChild(1).GetComponent<Slider>();
    }

    private Slider GetYAxisSlider() {
        return transform.GetChild(3).GetChild(1).GetComponent<Slider>();
    }

    private Text GetYAxisText() {
        return transform.GetChild(3).GetChild(0).GetComponent<Text>();
    }

    public GameObject GetPopulationCirclePrefab() {
        return populationCircle;
    }

    public GameObject GetPopulationLinePrefab() {
        return populationLine;
    }
    #endregion
}
