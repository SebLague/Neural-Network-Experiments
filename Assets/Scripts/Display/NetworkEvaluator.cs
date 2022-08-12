
public static class NetworkEvaluator
{
	public static EvaluationData Evaluate(NeuralNetwork network, DataPoint[] data)
	{
		EvaluationData evalData = new EvaluationData(data[0].expectedOutputs.Length);
		evalData.total = data.Length;

		System.Threading.Tasks.Parallel.ForEach(data, (data) =>
		{
			double[] output = network.CalculateOutputs(data.inputs);
			int predictedLabel = network.MaxValueIndex(output);

			lock (evalData)
			{
				evalData.totalPerClass[data.label]++;
				if (predictedLabel == data.label)
				{
					evalData.numCorrectPerClass[data.label]++;
					evalData.numCorrect++;
				}
				else
				{
					evalData.wronglyPredictedAs[predictedLabel]++;
				}
			}
		});

		return evalData;
	}

}

public class EvaluationData
{
	public int numCorrect;
	public int total;

	public int[] numCorrectPerClass;
	public int[] totalPerClass;
	public int[] wronglyPredictedAs;

	public EvaluationData(int numClasses)
	{
		numCorrectPerClass = new int[numClasses];
		totalPerClass = new int[numClasses];
		wronglyPredictedAs = new int[numClasses];
	}

	public string GetAccuracyString()
	{
		double percentCorrect = (numCorrect / (double)total) * 100;
		return "Num correct: " + numCorrect + " / " + total + " (" + percentCorrect.ToString("F" + 4) + "%)";
	}
}