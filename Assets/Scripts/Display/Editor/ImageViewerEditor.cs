using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ImageViewer))]
public class ImageViewerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		ImageViewer viewer = (ImageViewer)target;

		using (new EditorGUI.DisabledScope(!Application.isPlaying || viewer.networkFile == null))
		{
			if (GUILayout.Button("Evaluate Network"))
			{
				viewer.EvaluateNetwork();
			}
		}
	}
}
