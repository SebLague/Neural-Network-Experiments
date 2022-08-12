public class NeuralNetwork
{
	public readonly Layer[] layers;
	public readonly int[] layerSizes;

	public ICost cost;
	System.Random rng;
	NetworkLearnData[] batchLearnData;

	// Create the neural network
	public NeuralNetwork(params int[] layerSizes)
	{
		this.layerSizes = layerSizes;
		rng = new System.Random();

		layers = new Layer[layerSizes.Length - 1];
		for (int i = 0; i < layers.Length; i++)
		{
			layers[i] = new Layer(layerSizes[i], layerSizes[i + 1], rng);
		}

		cost = new Cost.MeanSquaredError();
	}

	// Run the inputs through the network to predict which class they belong to.
	// Also returns the activations from the output layer.
	public (int predictedClass, double[] outputs) Classify(double[] inputs)
	{
		var outputs = CalculateOutputs(inputs);
		int predictedClass = MaxValueIndex(outputs);
		return (predictedClass, outputs);
	}

	// Run the inputs through the network to calculate the outputs
	public double[] CalculateOutputs(double[] inputs)
	{
		foreach (Layer layer in layers)
		{
			inputs = layer.CalculateOutputs(inputs);
		}
		return inputs;
	}


	public void Learn(DataPoint[] trainingData, double learnRate, double regularization = 0, double momentum = 0)
	{

		if (batchLearnData == null || batchLearnData.Length != trainingData.Length)
		{
			batchLearnData = new NetworkLearnData[trainingData.Length];
			for (int i = 0; i < batchLearnData.Length; i++)
			{
				batchLearnData[i] = new NetworkLearnData(layers);
			}
		}

		System.Threading.Tasks.Parallel.For(0, trainingData.Length, (i) =>
		{
			UpdateGradients(trainingData[i], batchLearnData[i]);
		});


		// Update weights and biases based on the calculated gradients
		for (int i = 0; i < layers.Length; i++)
		{
			layers[i].ApplyGradients(learnRate / trainingData.Length, regularization, momentum);
		}
	}


	void UpdateGradients(DataPoint data, NetworkLearnData learnData)
	{
		// Feed data through the network to calculate outputs.
		// Save all inputs/weightedinputs/activations along the way to use for backpropagation.
		double[] inputsToNextLayer = data.inputs;

		for (int i = 0; i < layers.Length; i++)
		{
			inputsToNextLayer = layers[i].CalculateOutputs(inputsToNextLayer, learnData.layerData[i]);
		}

		// -- Backpropagation --
		int outputLayerIndex = layers.Length - 1;
		Layer outputLayer = layers[outputLayerIndex];
		LayerLearnData outputLearnData = learnData.layerData[outputLayerIndex];

		// Update output layer gradients
		outputLayer.CalculateOutputLayerNodeValues(outputLearnData, data.expectedOutputs, cost);
		outputLayer.UpdateGradients(outputLearnData);

		// Update all hidden layer gradients
		for (int i = outputLayerIndex - 1; i >= 0; i--)
		{
			LayerLearnData layerLearnData = learnData.layerData[i];
			Layer hiddenLayer = layers[i];

			hiddenLayer.CalculateHiddenLayerNodeValues(layerLearnData, layers[i + 1], learnData.layerData[i + 1].nodeValues);
			hiddenLayer.UpdateGradients(layerLearnData);
		}

	}

	public void SetCostFunction(ICost costFunction)
	{
		this.cost = costFunction;
	}

	public void SetActivationFunction(IActivation activation)
	{
		SetActivationFunction(activation, activation);
	}

	public void SetActivationFunction(IActivation activation, IActivation outputLayerActivation)
	{
		for (int i = 0; i < layers.Length - 1; i++)
		{
			layers[i].SetActivationFunction(activation);
		}
		layers[layers.Length - 1].SetActivationFunction(outputLayerActivation);
	}


	public int MaxValueIndex(double[] values)
	{
		double maxValue = double.MinValue;
		int index = 0;
		for (int i = 0; i < values.Length; i++)
		{
			if (values[i] > maxValue)
			{
				maxValue = values[i];
				index = i;
			}
		}

		return index;
	}
}


public class NetworkLearnData
{
	public LayerLearnData[] layerData;

	public NetworkLearnData(Layer[] layers)
	{
		layerData = new LayerLearnData[layers.Length];
		for (int i = 0; i < layers.Length; i++)
		{
			layerData[i] = new LayerLearnData(layers[i]);
		}
	}
}

public class LayerLearnData
{
	public double[] inputs;
	public double[] weightedInputs;
	public double[] activations;
	public double[] nodeValues;

	public LayerLearnData(Layer layer)
	{
		weightedInputs = new double[layer.numNodesOut];
		activations = new double[layer.numNodesOut];
		nodeValues = new double[layer.numNodesOut];
	}
}