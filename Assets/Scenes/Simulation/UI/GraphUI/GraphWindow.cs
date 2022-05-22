using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphWindow : MonoBehaviour {
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
    GraphFile graphFile;
    Color32[] speciesColors;
    int lastColumnText;

    public int rowCount;
    public int columnDisplayCount;
    public float textDistance;
    private float defaultColumnWidth;

    float distanceFromTopRight = 30;
    float distanceFromBottomLeft = 35;

    #region GraphControls

    /// <summary>
    /// Sets up the graph without any points on it
    /// </summary>
    public void SetupGraph(int refreshHours) {
        defaultColumnWidth = verticalColumn.GetComponent<RectTransform>().rect.width;
        graphFile = null;
        speciesColors = null;
        graphContainer = GetGraphTransform().GetChild(0).GetChild(0).GetComponent<RectTransform>();
        SetPopulationMax(10);
        SetXAxisLabel(refreshHours);
        GetColumnTransform().localPosition = new Vector2(0, distanceFromBottomLeft + GetColumnTransform().localPosition.y);
        rows = new List<RectTransform>(rowCount);
        columns = new List<Column>(20);
        GetYAxisSlider().minValue = (int)(GetGraphWidth() / defaultColumnWidth);
        columnDisplayCount = (int)GetYAxisSlider().minValue * 5;
        GetYAxisSlider().value = columnDisplayCount;
        GetYAxisSlider().maxValue = columnDisplayCount * 10;
    }

    /// <summary>
    /// Setups up the points from the file and loads a new file
    /// </summary>
    public void DisplayGraph(GraphFile graphFile) {
        this.graphFile = graphFile;
        if (graphFile == null)
            return;
        SetPopulationMax(graphFile.GetMax());
        speciesColors = graphFile.GetSpeciesColors();
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

    /// <summary>
    /// Refreshes the points from the file that is already loaded
    /// </summary>
    public void RefreshGraph() {
        if (graphFile == null)
            return;
        RefreshGraphPoints();
        RefreshColumnWidth();
        RefreshRows(rowCount, 1);
        RefreshColumnPointLines();
        RefreshRowLines();
        RefreshColumnText();
        RefreshGraphXSlider();
    }

    /// <summary>
    /// Refreshes only the new points that were added to the file if possible
    /// </summary>
    public void RefreshGraphFromTime(int pastRefreshCount) {
        if (graphFile.GetMax() > yMaximum) {
            RefreshGraph();
        } else {
            RefreshGraphPoints(pastRefreshCount);
            RefreshColumnWidth();
            RefreshColumnPointLines(pastRefreshCount);
            RefreshRowLines();
            RefreshColumnText(lastColumnText);
            RefreshGraphXSlider();
        }
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
    /// Sets the yMaximum to the population value rounded to the highest divisble number by 10, whichever is greater.
    /// Refreshes the yMaximum label
    /// </summary>
    /// <param name="population"></param>
    public void SetPopulationMax(int population) {
        yMaximum = Mathf.Max(Mathf.CeilToInt(population / 10.0f) * 10, 10);
        SetYAxisLabel(yMaximum);
    }
    #endregion

    #region GraphUtility
    /// <summary>
    /// Sets the population max to the file max
    /// Get the sets of points from the file and sets thier position
    /// Hides the unused columns and points
    /// Makes sure there is a column for each set of points
    /// </summary>
    void RefreshGraphPoints() {
        RefreshGraphPoints(0);
    }

    /// <summary>
    /// Sets the population max to the file max
    /// Get the sets of points from the file and sets thier position
    /// Hides the unused columns and points
    /// Makes sure there is a column for each set of points
    /// </summary>
    /// <param name="startingIndex">The starting index of the points</param>
    void RefreshGraphPoints(int startingIndex) {
        SetPopulationMax(graphFile.GetMax());
        int graphSize = graphFile.GetGraphSize();
        for (int i = startingIndex; i < graphSize; i++) {
            int[] points = new int[graphFile.GetPointSize()];
            graphFile.GetPoints(points, i);
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
            columns[column].SetupColumn(this);
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
    public Color32 GetSpeciesColor(int speciesIndex) {
        return speciesColors[speciesIndex];
    }

    #region Columns
    /// <summary>
    /// Refreshes all column text to only be displayed on each column 
    /// if the distance to the last dispayed column is greater than textDistance
    /// column 0 is always displayed
    /// </summary>
    void RefreshColumnText() {
        RefreshColumnText(0);
    }

    /// <summary>
    /// Refreshes the column text to only be displayed on each column 
    /// if the distance to the last dispayed column is greater than textDistance
    /// column 0 is always displayed
    /// </summary>
    /// <param name="startingIndex">The starting index of the columns</param>
    void RefreshColumnText(int startingIndex) {
        float columnWidth = GetColumnWidth();
        int count = int.MaxValue;
        for (int i = startingIndex; i < columns.Count; i++) {
            if (count * columnWidth >= textDistance) {
                GetColumn(i).GetText().enabled = true;
                count = 0;
                count++;
                lastColumnText = i;
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
    }

    /// <summary>
    /// Refreshed all the lines in between each point to account for thier new graph positions
    /// </summary>
    void RefreshColumnPointLines() {
        RefreshColumnPointLines(0);
    }


    /// <summary>
    /// Refreshed the lines in between each point to account for thier new graph positions
    /// </summary>
    /// 
    /// <param name="startingIndex">The starting index to refresh</param>
    void RefreshColumnPointLines(int startingIndex) {
        for (int i = startingIndex; i < columns.Count; i++) {
            for (int j = 0; j < GetColumn(i).GetLinesCount(); j++) {
                RectTransform line = GetColumn(i).GetLine(j);
                if (i == 0 || i >= graphFile.GetGraphSize() || j >= graphFile.GetPointSize()) {
                    line.gameObject.SetActive(false);
                } else {
                    line.GetComponent<Image>().color = GetSpeciesColor(j);
                    Vector2 fromPos = GetColumn(i - 1).GetPoint(j).position;
                    Vector2 toPos = GetColumn(i).GetPoint(j).position;
                    line.sizeDelta = new Vector2(Vector2.Distance(fromPos, toPos), line.sizeDelta.y);
                    line.eulerAngles = new Vector3(0, 0, (Mathf.Atan2(toPos.y - fromPos.y, toPos.x - fromPos.x) * 180 / Mathf.PI));
                    line.position = new Vector2((fromPos.x + toPos.x) / 2, (fromPos.y + toPos.y) / 2);
                    line.gameObject.SetActive(true);
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
            column.GetPoint(i).GetComponent<Image>().color = GetSpeciesColor(i);
            column.GetPoint(i).localPosition = new Vector2(0, FindGraphPosition(points[i]));
            column.GetPoint(i).gameObject.SetActive(true);
            column.GetLine(i).gameObject.SetActive(true);
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
            int rowValue = Mathf.RoundToInt(yMaximum * i / (rowCount - 1) / roundTo) * roundTo;
            float rowPosition = distanceFromBottomLeft;
            if (i != 0)
                rowPosition += FindGraphPosition(rowValue);
            GetRow(i).localPosition = new Vector2(10, rowPosition);
            GetRow(i).GetChild(0).GetComponent<Text>().text = rowValue.ToString();
            GetRow(i).gameObject.SetActive(true);
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