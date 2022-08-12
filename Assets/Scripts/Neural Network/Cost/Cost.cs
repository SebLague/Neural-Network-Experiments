public class Cost
{

	public enum CostType
	{
		MeanSquareError,
		CrossEntropy
	}

	public static ICost GetCostFromType(CostType type)
	{
		switch (type)
		{
			case CostType.MeanSquareError:
				return new MeanSquaredError();
			case CostType.CrossEntropy:
				return new CrossEntropy();
			default:
				UnityEngine.Debug.LogError("Unhandled cost type");
				return new MeanSquaredError();
		}
	}

	public class MeanSquaredError : ICost
	{
		public double CostFunction(double[] predictedOutputs, double[] expectedOutputs)
		{
			// cost is sum (for all x,y pairs) of: 0.5 * (x-y)^2
			double cost = 0;
			for (int i = 0; i < predictedOutputs.Length; i++)
			{
				double error = predictedOutputs[i] - expectedOutputs[i];
				cost += error * error;
			}
			return 0.5 * cost;
		}

		public double CostDerivative(double predictedOutput, double expectedOutput)
		{
			return predictedOutput - expectedOutput;
		}

		public CostType CostFunctionType()
		{
			return CostType.MeanSquareError;
		}
	}

	public class CrossEntropy : ICost
	{
		// Note: expected outputs are expected to all be either 0 or 1
		public double CostFunction(double[] predictedOutputs, double[] expectedOutputs)
		{
			// cost is sum (for all x,y pairs) of: 0.5 * (x-y)^2
			double cost = 0;
			for (int i = 0; i < predictedOutputs.Length; i++)
			{
				double x = predictedOutputs[i];
				double y = expectedOutputs[i];
				double v = (y == 1) ? -System.Math.Log(x) : -System.Math.Log(1 - x);
				cost += double.IsNaN(v) ? 0 : v;
			}
			return cost;
		}

		public double CostDerivative(double predictedOutput, double expectedOutput)
		{
			double x = predictedOutput;
			double y = expectedOutput;
			if (x == 0 || x == 1)
			{
				return 0;
			}
			return (-x + y) / (x * (x - 1));
		}

		public CostType CostFunctionType()
		{
			return CostType.CrossEntropy;
		}
	}

}