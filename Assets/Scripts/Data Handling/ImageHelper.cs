using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImageHelper
{


	public static double[] ReadImage(byte[] imageData, int byteOffset, int imageSize, bool flip = false)
	{

		double[] pixelValues = new double[imageSize * imageSize];
		for (int pixelIndex = 0; pixelIndex < pixelValues.Length; pixelIndex++)
		{

			if (flip)
			{
				int x = pixelIndex % imageSize;
				int y = pixelIndex / imageSize;
				int flippedIndex = (imageSize - y - 1) * imageSize + (x);
				pixelValues[pixelIndex] = imageData[byteOffset + flippedIndex] / 255.0;
			}
			else
			{
				pixelValues[pixelIndex] = imageData[byteOffset + pixelIndex] / 255.0;
			}
		}

		return pixelValues;
	}

	public static byte[] ImagesToBytes(Image[] images)
	{
		List<byte> allBytes = new List<byte>();
		foreach (var image in images)
		{
			allBytes.AddRange(ImageToBytes(image));
		}
		return allBytes.ToArray();
	}

	public static byte[] ImageToBytes(Image image)
	{
		byte[] bytes = new byte[image.numPixels];
		for (int i = 0; i < bytes.Length; i++)
		{
			bytes[i] = (byte)(image.pixelValues[i] * 255);
		}
		return bytes;
	}

	public static Image TextureToImage(Texture2D texture, int imageLabel)
	{
		Debug.Assert(texture.width == texture.height, "Texture is not square");
		Color[] pixelCols = texture.GetPixels(0, 0, texture.width, texture.height);
		double[] inputs = new double[pixelCols.Length];

		for (int i = 0; i < inputs.Length; i++)
		{
			inputs[i] = pixelCols[i].r;
		}

		return new Image(texture.width, greyscale: true, inputs, imageLabel);
	}

	public static Image TextureToImage(RenderTexture renderTexture, int imageLabel)
	{
		Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
		var prevActiveRenderTexture = RenderTexture.active;

		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = prevActiveRenderTexture;
		return TextureToImage(texture2D, imageLabel);
	}
}
