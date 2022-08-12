using UnityEngine;

public class Image
{
	public readonly int size;
	public readonly int numPixels;
	public readonly bool greyscale;

	public readonly double[] pixelValues;
	public readonly int label;

	public Image(int size, bool greyscale, double[] pixelValues, int label)
	{
		this.size = size;
		this.numPixels = size * size;
		this.greyscale = greyscale;
		this.pixelValues = pixelValues;
		this.label = label;
	}

	public Image(int size, bool greyscale, int label)
	{
		this.size = size;
		this.numPixels = size * size;
		this.greyscale = greyscale;
		this.pixelValues = new double[numPixels];
		this.label = label;
	}

	public int GetFlatIndex(int x, int y)
	{
		return y * size + x;
	}

	public double Sample(double u, double v)
	{
		u = System.Math.Max(System.Math.Min(1, u), 0);
		v = System.Math.Max(System.Math.Min(1, v), 0);

		double texX = u * (size - 1);
		double texY = v * (size - 1);

		int indexLeft = (int)(texX);
		int indexBottom = (int)(texY);
		int indexRight = System.Math.Min(indexLeft + 1, size - 1);
		int indexTop = System.Math.Min(indexBottom + 1, size - 1);

		double blendX = texX - indexLeft;
		double blendY = texY - indexBottom;

		double bottomLeft = pixelValues[GetFlatIndex(indexLeft, indexBottom)];
		double bottomRight = pixelValues[GetFlatIndex(indexRight, indexBottom)];
		double topLeft = pixelValues[GetFlatIndex(indexLeft, indexTop)];
		double topRight = pixelValues[GetFlatIndex(indexRight, indexTop)];

		double valueBottom = bottomLeft + (bottomRight - bottomLeft) * blendX;
		double valueTop = topLeft + (topRight - topLeft) * blendX;
		double interpolatedValue = valueBottom + (valueTop - valueBottom) * blendY;
		return interpolatedValue;
	}

	public Texture2D ConvertToTexture2D()
	{
		Texture2D texture = new Texture2D(size, size);
		ConvertToTexture2D(ref texture);
		return texture;
	}

	public void ConvertToTexture2D(ref Texture2D texture)
	{
		if (texture == null || texture.width != size || texture.height != size)
		{
			texture = new Texture2D(size, size);
		}
		texture.filterMode = FilterMode.Point;

		Color[] colors = new Color[numPixels];
		for (int i = 0; i < numPixels; i++)
		{
			if (greyscale)
			{
				float v = (float)pixelValues[i];
				colors[i] = new Color(v, v, v);
			}
			else
			{
				float r = (float)pixelValues[i * 3 + 0];
				float g = (float)pixelValues[i * 3 + 1];
				float b = (float)pixelValues[i * 3 + 2];
				colors[i] = new Color(r, g, b);
			}
		}
		texture.SetPixels(colors);
		texture.Apply();
	}

}