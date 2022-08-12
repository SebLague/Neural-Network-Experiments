using static System.Math;

public readonly struct Activation
{

	public enum ActivationType
	{
		Sigmoid,
		TanH,
		ReLU,
		SiLU,
		Softmax
	}

	public static IActivation GetActivationFromType(ActivationType type)
	{
		switch (type)
		{
			case ActivationType.Sigmoid:
				return new Sigmoid();
			case ActivationType.TanH:
				return new TanH();
			case ActivationType.ReLU:
				return new ReLU();
			case ActivationType.SiLU:
				return new SiLU();
			case ActivationType.Softmax:
				return new Softmax();
			default:
				UnityEngine.Debug.LogError("Unhandled activation type");
				return new Sigmoid();
		}
	}

	public readonly struct Sigmoid : IActivation
	{
		public double Activate(double[] inputs, int index)
		{
			return 1.0 / (1 + Exp(-inputs[index]));
		}

		public double Derivative(double[] inputs, int index)
		{
			double a = Activate(inputs, index);
			return a * (1 - a);
		}

		public ActivationType GetActivationType()
		{
			return ActivationType.Sigmoid;
		}
	}

	public readonly struct TanH : IActivation
	{
		public double Activate(double[] inputs, int index)
		{
			double e2 = Exp(2 * inputs[index]);
			return (e2 - 1) / (e2 + 1);
		}

		public double Derivative(double[] inputs, int index)
		{
			double e2 = Exp(2 * inputs[index]);
			double t = (e2 - 1) / (e2 + 1);
			return 1 - t * t;
		}

		public ActivationType GetActivationType()
		{
			return ActivationType.TanH;
		}
	}


	public readonly struct ReLU : IActivation
	{
		public double Activate(double[] inputs, int index)
		{
			return Max(0, inputs[index]);
		}

		public double Derivative(double[] inputs, int index)
		{
			return (inputs[index] > 0) ? 1 : 0;
		}

		public ActivationType GetActivationType()
		{
			return ActivationType.ReLU;
		}
	}

	public readonly struct SiLU : IActivation
	{
		public double Activate(double[] inputs, int index)
		{
			return inputs[index] / (1 + Exp(-inputs[index]));
		}

		public double Derivative(double[] inputs, int index)
		{
			double sig = 1 / (1 + Exp(-inputs[index]));
			return inputs[index] * sig * (1 - sig) + sig;
		}

		public ActivationType GetActivationType()
		{
			return ActivationType.SiLU;
		}
	}


	public readonly struct Softmax : IActivation
	{
		public double Activate(double[] inputs, int index)
		{
			double expSum = 0;
			for (int i = 0; i < inputs.Length; i++)
			{
				expSum += Exp(inputs[i]);
			}

			double res = Exp(inputs[index]) / expSum;

			return res;
		}

		public double Derivative(double[] inputs, int index)
		{
			double expSum = 0;
			for (int i = 0; i < inputs.Length; i++)
			{
				expSum += Exp(inputs[i]);
			}

			double ex = Exp(inputs[index]);

			return (ex * expSum - ex * ex) / (expSum * expSum);
		}

		public ActivationType GetActivationType()
		{
			return ActivationType.Softmax;
		}
	}

}
