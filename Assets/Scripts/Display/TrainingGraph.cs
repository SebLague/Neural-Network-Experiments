using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingGraph : MonoBehaviour
{
	public TMPro.TMP_Text text;
	public TMPro.TMP_Text evalText;
	public int percentageIncrement = 5;
	public LineRenderer trainAccuracyGraph;
	public LineRenderer validationAccuracyGraph;
	public float epochSpacing = 0.25f;
	float endY;

	NetworkTrainer trainer;
	double prevEpochTrainAccuracy;
	double prevEpochValidationAccuracy;

	void Awake()
	{
		trainer = FindObjectOfType<NetworkTrainer>();
		trainer.onTrainingStarted += ClearGraphs;
		trainer.onEpochComplete += UpdateGraphs;

		Camera cam = Camera.main;
		endY = cam.transform.position.y + cam.orthographicSize - 0.5f;
		int n = 100 / percentageIncrement;

		for (int i = 1; i <= n; i++)
		{
			float t = i / (float)n;
			int percent = i * percentageIncrement;
			var textInstance = Instantiate(text);
			textInstance.alignment = TMPro.TextAlignmentOptions.MidlineRight;
			textInstance.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);
			textInstance.transform.SetParent(transform);
			textInstance.transform.position = new Vector3(-0.1f, HeightFromAccuracyT(t));
			textInstance.text = percent + "%";
		}

		for (int i = 1; i < 20; i++)
		{
			var textInstance = Instantiate(text);
			textInstance.alignment = TMPro.TextAlignmentOptions.Midline;
			textInstance.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
			textInstance.transform.SetParent(transform);
			textInstance.transform.position = new Vector3(i * epochSpacing, -0.1f);
			textInstance.text = i + "";
		}
		text.gameObject.SetActive(false);
	}

	void Update()
	{
		string evalString = SetTextColour($"Training Accuracy: {prevEpochTrainAccuracy * 100:0.00}%", trainAccuracyGraph.sharedMaterial.color);
		evalString += SetTextColour($"\nTest Accuracy: {prevEpochValidationAccuracy * 100:0.00}%", validationAccuracyGraph.sharedMaterial.color);
		evalString += $"\nEpoch completion: {(trainer.sessionInfo.epochsCompleted % 1) * 100:0.0}%";
		evalText.text = evalString;
	}

	void ClearGraphs()
	{
		trainAccuracyGraph.positionCount = 0;
		validationAccuracyGraph.positionCount = 0;
		UpdateGraphs(-1);
	}

	void UpdateGraphs(int epoch)
	{
		var trainEval = trainer.Evaluate(false);
		var validationEval = trainer.Evaluate(true);

		prevEpochTrainAccuracy = trainEval.numCorrect / (double)trainEval.total;
		prevEpochValidationAccuracy = validationEval.numCorrect / (double)validationEval.total;

		UpdateGraph(epoch + 1, prevEpochTrainAccuracy, trainAccuracyGraph);
		UpdateGraph(epoch + 1, prevEpochValidationAccuracy, validationAccuracyGraph);
	}

	void UpdateGraph(int epoch, double accuracy, LineRenderer graph)
	{
		Vector2 pos = new Vector2(epoch * epochSpacing, HeightFromAccuracyT(accuracy));
		graph.positionCount += 1;
		graph.SetPosition(graph.positionCount - 1, pos);
	}

	float HeightFromAccuracyT(double t)
	{
		return Mathf.Lerp(0, endY, (float)t);
	}

	string SetTextColour(string text, Color col)
	{
		string colString = "#" + ColorUtility.ToHtmlStringRGB(col);
		return $"<color={colString}>{text}</color>";
	}
}
