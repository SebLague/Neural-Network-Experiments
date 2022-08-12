using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NetworkTrainer))]
public class TrainerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		using (new EditorGUI.DisabledScope(!Application.isPlaying))
		{
			if (GUILayout.Button("Save Network"))
			{
				NetworkTrainer trainer = (NetworkTrainer)target;
				trainer.Save();
			}
		}
	}
}
