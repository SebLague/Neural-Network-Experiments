public interface ICost
{
	double CostFunction(double[] predictedOutputs, double[] expectedOutputs);

	double CostDerivative(double predictedOutput, double expectedOutput);

	Cost.CostType CostFunctionType();
}
