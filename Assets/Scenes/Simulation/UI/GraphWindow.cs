using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphWindow : MonoBehaviour {
	[SerializeField]
	private GameObject labelX;
	[SerializeField]
	private GameObject labelY;
	[SerializeField]
	private GameObject horizontalLine;
	[SerializeField]
	private Sprite circleSprite;
	private RectTransform graphContainer;
	public float yMaximum;
	public int xLineCount;

	private void Awake() {
		graphContainer = GameObject.Find("GraphNodeContainer").GetComponent<RectTransform>();
	}
	private void CreateCircle(Vector2 _anchorposition, Color _color) {
		GameObject newCircle = new GameObject("circle", typeof(Image));
		newCircle.transform.SetParent(graphContainer, false);
		newCircle.GetComponent<Image>().sprite = circleSprite;
		newCircle.GetComponent<Image>().color = new Color (_color.r, _color.g, _color.b, 1);
		RectTransform rectTransform = newCircle.GetComponent<RectTransform>();
		rectTransform.anchoredPosition = _anchorposition;
		rectTransform.sizeDelta = new Vector2(5, 5);
		rectTransform.anchorMin = new Vector2(0, 0);
		rectTransform.anchorMax = new Vector2(0, 0);
	}
	public void ShowGeneralGraph(int _index) {
		float graphHeight = transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.y;
		float xSize = 10;
		float xOffset = 40;

		for (int i = 0; i < _index; i++) {
			float xPosition = xOffset + (i * xSize);
			float yPosition = (_index / yMaximum) * graphHeight;
			GameObject tempX = Instantiate(labelX, graphContainer.transform).gameObject;
			tempX.name = "labelX";
			RectTransform labelTempX = tempX.GetComponent<RectTransform>();
			labelTempX.anchorMin = new Vector2(0, 0);
			labelTempX.anchorMax = new Vector2(0, 0);
			labelTempX.position = new Vector2(xPosition, 7);
			labelTempX.GetComponent<Text>().text = i.ToString();
		}
	}

	public void ShowGraph (List<int> _values, Color _speciesColor) {
		float graphHeight = transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.y;
		float xSize = 10;
		float xOffset = 40;

		for (int i = 0; i < _values.Count; i++) {
			float xPosition = xOffset + (i * xSize);
			float yPosition = (_values[i] / yMaximum) * graphHeight;
			CreateCircle(new Vector2(xPosition, yPosition), _speciesColor);

		}

	}
	public void ClearGraph() {
		for (int i = 0; i < graphContainer.childCount; i++) {
			if (graphContainer.transform.GetChild(i).name == "circle") {
				Destroy(graphContainer.transform.GetChild(i).gameObject);
			}
			if (graphContainer.transform.GetChild(i).name == "labelX") {
				Destroy(graphContainer.transform.GetChild(i).gameObject);
			}

		}
	}
}
