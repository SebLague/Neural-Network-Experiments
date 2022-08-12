using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageProcessor : MonoBehaviour
{
	public int imageIndex;
	public int numVersionsPerImg = 1;
	public MeshRenderer originalDisplay;
	public MeshRenderer transformedDisplay;
	public string saveFileName;
	public TransformationSettings settings;

	ImageLoader loader;
	int prevImageIndex;
	Image currentImage;
	Texture2D transformedTexture;

	void Start()
	{
		loader = FindObjectOfType<ImageLoader>();
		prevImageIndex = -1;
	}

	void Update()
	{
		imageIndex = Mathf.Clamp(imageIndex, 0, loader.NumImages - 1);

		if (prevImageIndex != imageIndex)
		{
			prevImageIndex = imageIndex;
			currentImage = loader.GetImage(imageIndex);
			originalDisplay.material.mainTexture = currentImage.ConvertToTexture2D();

		}


		var transformedImage = TransformImage(currentImage, settings, currentImage.size);

		transformedImage.ConvertToTexture2D(ref transformedTexture);
		transformedDisplay.material.mainTexture = transformedTexture;
	}

	
	public void RandomizeSettings()
	{
		settings = CreateRandomSettings(new System.Random(), currentImage);
	}

	
	public void ProcessAndSaveAll()
	{
		List<byte> allBytes = new List<byte>();
		List<byte> allLabelBytes = new List<byte>();
		System.Random rng = new System.Random();


		for (int i = 0; i < loader.NumImages; i++)
		{
			for (int j = 0; j < numVersionsPerImg; j++)
			{
				Image image = loader.GetImage(i);
				var settings = CreateRandomSettings(rng, image);
				var transformedImage = TransformImage(image, settings, currentImage.size);
				byte[] bytes = ImageHelper.ImageToBytes(transformedImage);
				allBytes.AddRange(bytes);
				allLabelBytes.Add((byte)image.label);
			}
		}

		FileHelper.SaveBytesToFile(FileHelper.MakePath("Assets"), saveFileName, allBytes.ToArray(), true);
		FileHelper.SaveBytesToFile(FileHelper.MakePath("Assets"), saveFileName + "_labels", allLabelBytes.ToArray(), true);
	}

	public static Image TransformImage(Image original, TransformationSettings settings, int size)
	{
		System.Random rng = new System.Random(settings.noiseSeed);

		Image transformedImage = new Image(size, original.greyscale, original.label);
		if (settings.scale != 0)
		{

			Vector2 iHat = new Vector2(Mathf.Cos(settings.angle), Mathf.Sin(settings.angle)) / settings.scale;
			Vector2 jHat = new Vector2(-iHat.y, iHat.x);
			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					double u = x / (size - 1.0);
					double v = y / (size - 1.0);

					double uTransformed = iHat.x * (u - 0.5) + jHat.x * (v - 0.5) + 0.5 - settings.offset.x;
					double vTransformed = iHat.y * (u - 0.5) + jHat.y * (v - 0.5) + 0.5 - settings.offset.y;
					double pixelValue = original.Sample(uTransformed, vTransformed);
					double noiseValue = 0;
					if (rng.NextDouble() <= settings.noiseProbability)
					{
						noiseValue = (rng.NextDouble() - 0.5) * settings.noiseStrength;
					}
					transformedImage.pixelValues[transformedImage.GetFlatIndex(x, y)] = System.Math.Clamp(pixelValue + noiseValue, 0, 1);
				}
			}
		}
		return transformedImage;
	}

	static double RandomInNormalDistribution(System.Random prng, double mean = 0, double standardDeviation = 1)
	{
		double x1 = 1 - prng.NextDouble();
		double x2 = 1 - prng.NextDouble();

		double y1 = System.Math.Sqrt(-2.0 * System.Math.Log(x1)) * System.Math.Cos(2.0 * System.Math.PI * x2);
		return y1 * standardDeviation + mean;
	}

	public TransformationSettings CreateRandomSettings(System.Random rng, Image image)
	{
		TransformationSettings settings = new TransformationSettings();
		settings.angle = (float)RandomInNormalDistribution(rng) * 0.15f;
		settings.scale = 1 + (float)RandomInNormalDistribution(rng) * 0.1f;

		settings.noiseSeed = rng.Next();
		settings.noiseProbability = (float)System.Math.Min(rng.NextDouble(), rng.NextDouble()) * 0.05f;
		settings.noiseStrength = (float)System.Math.Min(rng.NextDouble(), rng.NextDouble());


		int boundsMinX = image.size;
		int boundsMaxX = 0;
		int boundsMinY = image.size;
		int boundsMaxY = 0;

		for (int y = 0; y < image.size; y++)
		{
			for (int x = 0; x < image.size; x++)
			{
				if (image.pixelValues[image.GetFlatIndex(x, y)] > 0)
				{
					boundsMinX = Mathf.Min(boundsMinX, x);
					boundsMaxX = Mathf.Max(boundsMaxX, x);
					boundsMinY = Mathf.Min(boundsMinY, y);
					boundsMaxY = Mathf.Max(boundsMaxY, y);
				}
			}
		}


		float offsetMinX = -boundsMinX / (float)image.size;
		float offsetMaxX = (image.size - boundsMaxX) / (float)image.size;
		float offsetMinY = -boundsMinY / (float)image.size;
		float offsetMaxY = (image.size - boundsMaxY) / (float)image.size;

		float offsetX = Mathf.Lerp(offsetMinX, offsetMaxX, (float)rng.NextDouble());
		float offsetY = Mathf.Lerp(offsetMinY, offsetMaxY, (float)rng.NextDouble());
		settings.offset = new Vector2(offsetX, offsetY) * 0.8f;


		return settings;
	}

	[System.Serializable]
	public struct TransformationSettings
	{
		public float angle;
		public float scale;
		public Vector2 offset;
		public int noiseSeed;
		[Range(0, 1)] public float noiseProbability;
		[Range(0, 1)] public float noiseStrength;
	}


}
