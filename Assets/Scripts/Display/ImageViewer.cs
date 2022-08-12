using UnityEngine;
using UnityEngine.UI;

public class ImageViewer : MonoBehaviour
{

	public int displayIndex;
	public TextAsset networkFile;

	[Header("UI")]
	public UnityEngine.UI.RawImage display;
	public TMPro.TMP_Text label;
	public Button prevButton;
	public Button nextButton;
	public Button randomizeButton;

	int lastDisplayIndex;
	ImageLoader loader;
	NeuralNetwork neuralNetwork;

	void Start()
	{
		loader = FindObjectOfType<ImageLoader>();
		lastDisplayIndex = -1;
		if (networkFile)
		{
			neuralNetwork = NetworkSaveData.LoadNetworkFromData(networkFile.text);
		}

		prevButton.onClick.AddListener(() => displayIndex--);
		nextButton.onClick.AddListener(() => displayIndex++);
		randomizeButton.onClick.AddListener(() => displayIndex = Random.Range(0, loader.NumImages));
	}

	public void EvaluateNetwork()
	{
		if (neuralNetwork != null)
		{
			var evaluate = NetworkEvaluator.Evaluate(neuralNetwork, loader.GetAllData());
			Debug.Log(evaluate.GetAccuracyString());
		}
		else
		{
			Debug.Log("No network specified.");
		}
	}

	void Update()
	{
		displayIndex = Mathf.Clamp(displayIndex, 0, loader.NumImages - 1);

		if (lastDisplayIndex != displayIndex)
		{
			lastDisplayIndex = displayIndex;
			var img = loader.GetImage(displayIndex);
			DisplayImage(img);

		}
	}

	void DisplayImage(Image image)
	{
		display.texture = image.ConvertToTexture2D();

		label.text = $"Label: {loader.LabelNames[image.label]}";
		if (neuralNetwork != null)
		{
			label.text += SetTextColour(" | ", "#707070");
			int prediction = neuralNetwork.Classify(image.pixelValues).predictedClass;
			bool correct = prediction == image.label;
			string colString = (correct) ? "#7cf760" : "#db524b";
			label.text += SetTextColour($"Prediction: {loader.LabelNames[prediction]}", colString);
		}
	}

	string SetTextColour(string text, string colString)
	{
		return $"<color={colString}>{text}</color>";
	}

}
