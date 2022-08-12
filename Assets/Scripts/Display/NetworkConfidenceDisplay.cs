using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkConfidenceDisplay : MonoBehaviour
{
	public TextAsset networkFile;
	public string predictedLabel;

	public MeshRenderer display;
	public TMPro.TMP_Text labelsUI;
	public TMPro.TMP_Text confidenceUI;

	ImageLoader loader;
	DrawingController drawingController;
	NeuralNetwork network;

	void Start()
	{
		drawingController = FindObjectOfType<DrawingController>();
		network = NetworkSaveData.LoadNetworkFromData(networkFile.text);
		loader = FindObjectOfType<ImageLoader>();
	}


	void Update()
	{
		RenderTexture digitRenderTexture = drawingController.RenderOutputTexture();
		Image image = ImageHelper.TextureToImage(digitRenderTexture, 0);
		(int prediction, double[] outputs) = network.Classify(image.pixelValues);

		UpdateDisplay(image, outputs, prediction);
	}

	void UpdateDisplay(Image image, double[] outputs, int prediction)
	{
		predictedLabel = loader.LabelNames[prediction];

		var rankedLabels = new List<RankedLabel>();
		double s = 0;
		for (int i = 0; i < outputs.Length; i++)
		{
			var r = new RankedLabel() { name = loader.LabelNames[i], score = (float)outputs[i] };
			rankedLabels.Add(r);
			s += outputs[i];
		}

		rankedLabels.Sort((a, b) => b.score.CompareTo(a.score));
		labelsUI.text = "<color=#ffffff>";
		confidenceUI.text = "<color=#ffffff>";
		for (int i = 0; i < outputs.Length; i++)
		{
			labelsUI.text += rankedLabels[i].name + "\n" + ((i == 0) ? "</color>" : "");
			confidenceUI.text += rankedLabels[i].Text + "\n" + ((i == 0) ? "</color>" : ""); ;
		}

		display.material.mainTexture = image.ConvertToTexture2D();
	}

	public struct RankedLabel
	{
		public string name;
		public float score;

		public string Text
		{
			get
			{
				return $"{score * 100:0.00}%";
			}
		}
	}
}
