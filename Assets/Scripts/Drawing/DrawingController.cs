using UnityEngine;
using System.Collections.Generic;

public class DrawingController : MonoBehaviour
{

	public int drawResolution = 1024;
	public int outputResolution = 28;

	public BoxCollider2D canvasCollider;
	public ComputeShader drawCompute;
	public float brushRadius;
	[Range(0, 1)]
	public float smoothing;
	RenderTexture canvas;
	RenderTexture outputCanvas;
	RenderTexture croppedCanvas;

	Camera cam;
	Vector2Int brushCentreOld;
	ComputeBuffer boundsBuffer;

	void Start()
	{
		cam = Camera.main;
		ComputeHelper.CreateRenderTexture(ref canvas, drawResolution, drawResolution, FilterMode.Bilinear, ComputeHelper.defaultGraphicsFormat, "Draw Canvas");
		canvasCollider.gameObject.GetComponent<MeshRenderer>().material.mainTexture = canvas;

		ComputeHelper.CreateStructuredBuffer<uint>(ref boundsBuffer, 4);
		boundsBuffer.SetData(new int[] { drawResolution - 1, 0, drawResolution - 1, 0 });
		drawCompute.SetBuffer(0, "bounds", boundsBuffer);
		drawCompute.SetTexture(0, "Canvas", canvas);
	}

	public RenderTexture Canvas => canvas;

	public RenderTexture RenderOutputTexture()
	{

		ComputeHelper.CreateRenderTexture(ref outputCanvas, outputResolution, outputResolution, FilterMode.Point, ComputeHelper.defaultGraphicsFormat, "Draw Output");
		RenderTexture source = canvas;
		RenderTexture downscaleSource = RenderTexture.GetTemporary(source.width, source.height);
		Graphics.Blit(source, downscaleSource);
		int currWidth = source.width / 2;
		while (currWidth > outputResolution * 2)
		{
			RenderTexture temp = RenderTexture.GetTemporary(currWidth, currWidth);
			Graphics.Blit(downscaleSource, temp);
			currWidth /= 2;
			RenderTexture.ReleaseTemporary(downscaleSource);
			downscaleSource = temp;
		}
		Graphics.Blit(downscaleSource, outputCanvas);
		RenderTexture.ReleaseTemporary(downscaleSource);
		return outputCanvas;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			Clear();
		}

		Vector2 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);

		Bounds canvasBounds = canvasCollider.bounds;

		float tx = Mathf.InverseLerp(canvasBounds.min.x, canvasBounds.max.x, mouseWorld.x);
		float ty = Mathf.InverseLerp(canvasBounds.min.y, canvasBounds.max.y, mouseWorld.y);

		Vector2Int brushCentre = new Vector2Int((int)(tx * drawResolution), (int)(ty * drawResolution));

		drawCompute.SetInts("brushCentre", brushCentre.x, brushCentre.y);
		drawCompute.SetInts("brushCentreOld", brushCentreOld.x, brushCentreOld.y);
		drawCompute.SetFloat("brushRadius", brushRadius);
		drawCompute.SetFloat("smoothing", smoothing);
		drawCompute.SetInt("resolution", drawResolution);
		drawCompute.SetInt("mode", (Input.GetMouseButton(0)) ? 0 : 1);

		if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
		{
			ComputeHelper.Dispatch(drawCompute, drawResolution, drawResolution);
		}

		brushCentreOld = brushCentre;
	}

	void Clear()
	{
		boundsBuffer.SetData(new int[] { drawResolution - 1, 0, drawResolution - 1, 0 });
		ComputeHelper.ClearRenderTexture(canvas);
	}

	void OnDestroy()
	{
		ComputeHelper.Release(boundsBuffer);
		ComputeHelper.Release(canvas, outputCanvas, croppedCanvas);
	}
}
