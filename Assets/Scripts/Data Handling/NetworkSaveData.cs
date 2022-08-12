[System.Serializable]
public class NetworkSaveData
{

	public int[] layerSizes;
	public ConnectionSaveData[] connections;
	public Cost.CostType costFunctionType;

	// Load network from saved data
	public NeuralNetwork LoadNetwork()
	{
		NeuralNetwork network = new NeuralNetwork(layerSizes);
		for (int i = 0; i < network.layers.Length; i++)
		{
			ConnectionSaveData loadedConnection = connections[i];

			System.Array.Copy(loadedConnection.weights, network.layers[i].weights, loadedConnection.weights.Length);
			System.Array.Copy(loadedConnection.biases, network.layers[i].biases, loadedConnection.biases.Length);
			network.layers[i].activation = Activation.GetActivationFromType(loadedConnection.activationType);
		}
		network.SetCostFunction(Cost.GetCostFromType((Cost.CostType)costFunctionType));

		return network;
	}

	// Load save data from file
	public static NeuralNetwork LoadNetworkFromFile(string path)
	{
		using (var reader = new System.IO.StreamReader(path))
		{
			string data = reader.ReadToEnd();
			return LoadNetworkFromData(data);
		}
	}

	public static NeuralNetwork LoadNetworkFromData(string loadedData)
	{
		return UnityEngine.JsonUtility.FromJson<NetworkSaveData>(loadedData).LoadNetwork();
	}

	public static string SerializeNetwork(NeuralNetwork network)
	{
		NetworkSaveData saveData = new NetworkSaveData();
		saveData.layerSizes = network.layerSizes;
		saveData.connections = new ConnectionSaveData[network.layers.Length];
		saveData.costFunctionType = (Cost.CostType)network.cost.CostFunctionType();

		for (int i = 0; i < network.layers.Length; i++)
		{
			saveData.connections[i].weights = network.layers[i].weights;
			saveData.connections[i].biases = network.layers[i].biases;
			saveData.connections[i].activationType = network.layers[i].activation.GetActivationType();
		}
		return UnityEngine.JsonUtility.ToJson(saveData);
	}

	public static void SaveToFile(string networkSaveString, string path)
	{
		using (var writer = new System.IO.StreamWriter(path))
		{
			writer.Write(networkSaveString);
		}
	}


	public static void SaveToFile(NeuralNetwork network, string path)
	{
		using (var writer = new System.IO.StreamWriter(path))
		{
			writer.Write(SerializeNetwork(network));
		}
	}


	[System.Serializable]
	public struct ConnectionSaveData
	{
		public double[] weights;
		public double[] biases;
		public Activation.ActivationType activationType;
	}
}
