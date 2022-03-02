using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphWindow : MonoBehaviour {
	[SerializeField]
	private GameObject horizontalRow;
	[SerializeField]
	private GameObject verticalCollum;
	[SerializeField]
	private GameObject populationCircle;
	[SerializeField]
	private Sprite circleSprite;
	private RectTransform graphContainer;
	public int yMaximum;
	public int xLineCount;

	float distanceFromTop = 60;
	float distanceFromBottom = 35;

	private void Awake() {
		graphContainer = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();
		RefreshHorizontalRows();
		if (SpeciesManager.Instance.GetSpeciesMotor().maxRefreshTime < 24)
			GetXAxisText().text = "Time(" + (int)SpeciesManager.Instance.GetSpeciesMotor().maxRefreshTime + "Hours)";
		else if (SpeciesManager.Instance.GetSpeciesMotor().maxRefreshTime < 168)
			GetXAxisText().text = "Time(" + (int)(SpeciesManager.Instance.GetSpeciesMotor().maxRefreshTime * 10.0f / 24) / 10.0f + "Days)";
		else if (SpeciesManager.Instance.GetSpeciesMotor().maxRefreshTime < 720)
			GetXAxisText().text = "Time(" + (int)(SpeciesManager.Instance.GetSpeciesMotor().maxRefreshTime * 10.0f / 168) / 10.0f + "Weeks)";
		else if (SpeciesManager.Instance.GetSpeciesMotor().maxRefreshTime < 8640)
			GetXAxisText().text = "Time(" + (int)(SpeciesManager.Instance.GetSpeciesMotor().maxRefreshTime * 10.0f / 720) / 10.0f + "Months)";
		else
			GetXAxisText().text = "Time(" + (int)(SpeciesManager.Instance.GetSpeciesMotor().maxRefreshTime * 10.0f / 8640) / 10.0f + "Years)";

	}

	public void AddPopulationList (List<int> _values, Color _speciesColor, BasicSpeciesScript _species) {
		for (int i = 0; i < _values.Count; i++) {
			AddNewDot(i, _values[i], _speciesColor, _species);
		}
	}

	public void AddNewDot(int timePosition, int populationPosition, Color _color, BasicSpeciesScript _species) {
		SetPopulationMax(populationPosition);

		CreateCircle(_color, GetVericalCollum(timePosition),populationPosition, _species);
	}

	GameObject CreateCircle( Color _color, GameObject _verticalCollum, int _populationCount, BasicSpeciesScript _species) {
		GameObject newCircle = Instantiate(populationCircle);
		newCircle.transform.SetParent(_verticalCollum.transform.GetChild(2), false);
		newCircle.GetComponent<Image>().color = new Color (_color.r, _color.g, _color.b, 1);
		PopulationCircle newPopulationCircle = newCircle.GetComponent<PopulationCircle>();
		newPopulationCircle.population = _populationCount;
		newPopulationCircle.species = _species;
		RectTransform rectTransform = newCircle.GetComponent<RectTransform>();
		rectTransform.anchoredPosition = new Vector2(2.5f, 0);
		rectTransform.sizeDelta = new Vector2(5, 5);
		rectTransform.localPosition = new Vector2(rectTransform.localPosition.x,FindCirclePosition(_populationCount));
		return newCircle;
	}

	float FindCirclePosition(int _population) {
		return (graphContainer.sizeDelta.y - distanceFromTop - distanceFromBottom + 30) * ((float)_population / (float)yMaximum) + distanceFromBottom;
    }

	GameObject GetVericalCollum(int _index) {
		if (GetCollumTransform().childCount > _index) {
			return GetCollumTransform().GetChild(_index).gameObject;
        } else {
			return CreateVerticalCollum();
        }
    }

	GameObject CreateVerticalCollum () {
		GameObject newCollum = Instantiate(verticalCollum, GetCollumTransform());
		newCollum.transform.GetChild(0).GetComponent<Text>().text = newCollum.transform.GetSiblingIndex().ToString();
		newCollum.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(1, graphContainer.sizeDelta.y - 40);
		newCollum.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (graphContainer.sizeDelta.y + 10) / 2);
		RefreshCollumHolderPosition();
		GetCollumTransform().sizeDelta += new Vector2(30, 0);
		RefreshHorizontalLines();
		return newCollum;
    }

	void RefreshCollumHolderPosition() {
		GetCollumTransform().GetComponent<RectTransform>().localPosition = new Vector2(GetCollumTransform().GetComponent<RectTransform>().sizeDelta.x / 2 + 40, -630);
		graphContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(GetCollumTransform().GetComponent<RectTransform>().sizeDelta.x + 40, graphContainer.GetComponent<RectTransform>().sizeDelta.y);
	}
	
	public bool SetPopulationMax(int _populaiton) {
		if (_populaiton >= yMaximum) {
			yMaximum = Mathf.RoundToInt(_populaiton / 10 + 1) * 10;
			GetYAxisText().text = "Population(Max:"+ yMaximum + ")";
			return true;
		}
		return false;
	}

	public void RefreshPopulationMax() {
		RefreshPopulationCirclePosition();
		RefreshHorizontalRows();
    }
	
	void RefreshPopulationCirclePosition() {
        for (int i = 0; i < GetCollumTransform().childCount; i++) {
            for (int f = 0; f < GetCollumTransform().GetChild(i).GetChild(2).childCount; f++) {
				PopulationCircle populationCircle = GetCollumTransform().GetChild(i).GetChild(2).GetChild(f).GetComponent<PopulationCircle>();
				populationCircle.GetComponent<RectTransform>().localPosition = new Vector2(populationCircle.GetComponent<RectTransform>().localPosition.x, FindCirclePosition(populationCircle.population));
			}
        }
    }

	void RefreshHorizontalRows () {
		List<float> rowPositions = new List<float>();
		List<int> rowVaues = new List<int>();
		int numberOfRows = 6;

		int roundNumber = 1;
		for (int i = numberOfRows - 1; i >= 0; i--) {
			if (i == 0) {
				rowPositions.Add(0 + distanceFromBottom);
				rowVaues.Add(0);
			} else {
				rowPositions.Add((float)Mathf.RoundToInt(yMaximum * i / (numberOfRows - 1) / roundNumber) * roundNumber / yMaximum * (graphContainer.sizeDelta.y - distanceFromTop + 30 - distanceFromBottom) + distanceFromBottom);
				rowVaues.Add(Mathf.RoundToInt(yMaximum * i / (numberOfRows - 1) / roundNumber) * roundNumber);
			}
		}
        for (int i = 0; i < rowPositions.Count; i++) {
			if (GetRowTransform().childCount - 1 > i) {
				GetRowTransform().GetChild(i).GetComponent<RectTransform>().localPosition = new Vector2(10, rowPositions[i]);
				GetRowTransform().GetChild(i).GetChild(0).GetComponent<Text>().text = rowVaues[i].ToString();
            } else {
				GameObject newRow = Instantiate(horizontalRow, GetRowTransform());
				newRow.GetComponent<RectTransform>().localPosition = new Vector2(10, rowPositions[i]);
				newRow.transform.GetChild(0).GetComponent<Text>().text = rowVaues[i].ToString();
			}
        }

    }

	void RefreshHorizontalLines() {
        for (int i = 0; i < GetRowTransform().childCount; i++) {
			RectTransform row = GetRowTransform().GetChild(i).GetChild(1).GetComponent<RectTransform>();
			row.sizeDelta = new Vector2(GetCollumTransform().sizeDelta.x + 60, 1);
			row.localPosition = new Vector2(row.sizeDelta.x / 2 + 15, 0);
        }
    }

	public RectTransform GetCollumTransform() {
		return graphContainer.GetChild(0).GetComponent<RectTransform>();
	}

	public RectTransform GetRowTransform() {
		return graphContainer.GetChild(1).GetComponent<RectTransform>();
	}

	public Text GetXAxisText() {
		return transform.GetChild(2).GetChild(0).GetComponent<Text>();
    }

	public Text GetYAxisText() {
		return transform.GetChild(3).GetChild(0).GetComponent<Text>();
	}
}
