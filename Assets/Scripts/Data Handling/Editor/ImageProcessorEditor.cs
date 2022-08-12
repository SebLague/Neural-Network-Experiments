using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ImageProcessor))]
public class ImageProcessorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		ImageProcessor imgProcessor = (ImageProcessor)target;

		if (GUILayout.Button("Randomize Settings"))
		{
			imgProcessor.RandomizeSettings();
		}

		using (new EditorGUI.DisabledScope(!Application.isPlaying))
		{
			if (GUILayout.Button("Process & Save All"))
			{

				imgProcessor.ProcessAndSaveAll();
			}
		}
	}
}
