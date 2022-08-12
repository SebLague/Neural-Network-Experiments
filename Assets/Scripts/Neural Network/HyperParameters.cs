[System.Serializable]
public class HyperParameters
{
	//[Header("Network achitecture")]
	public int[] layerSizes;
	public Activation.ActivationType activationType;
	public Activation.ActivationType outputActivationType;
	public Cost.CostType costType;

	//[Header("Learning parameters")]
	public double initialLearningRate;
	public double learnRateDecay;
	public int minibatchSize;
	public double momentum;
	public double regularization;

	public HyperParameters()
	{
		activationType = Activation.ActivationType.ReLU;
		outputActivationType = Activation.ActivationType.Softmax;
		costType = Cost.CostType.CrossEntropy;
		initialLearningRate = 0.05;
		learnRateDecay = 0.075;
		minibatchSize = 32;
		momentum = 0.9;
		regularization = 0.1;
	}

}
