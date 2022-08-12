using UnityEngine;

[System.Serializable]
public class TrainingSessionInfo
{
	public double epochsCompleted;
	public int batchesCompleted;
	public double elapsedTimeSeconds;
	public double avgTimePerEpochSeconds;
	public long avgTimePerBatchMillis;
	public double currentLearnRate;

	int numBatchesPerEpoch;
	System.Diagnostics.Stopwatch timer;

	public TrainingSessionInfo(int numBatchesPerEpoch)
	{
		this.numBatchesPerEpoch = numBatchesPerEpoch;
		timer = new System.Diagnostics.Stopwatch();
	}

	public void StartTimer()
	{
		timer.Start();
	}

	public void BatchCompleted()
	{
		batchesCompleted++;
		epochsCompleted = LimitDisplayPrecision(batchesCompleted / (double)numBatchesPerEpoch);

		elapsedTimeSeconds = LimitDisplayPrecision(timer.ElapsedMilliseconds / 1000);
		if (epochsCompleted > 0)
		{
			avgTimePerEpochSeconds = LimitDisplayPrecision(timer.ElapsedMilliseconds / epochsCompleted / 1000);
		}
		avgTimePerBatchMillis = timer.ElapsedMilliseconds / batchesCompleted;
	}

	double LimitDisplayPrecision(double value)
	{
		return (int)(value * 1000) / 1000.0;
	}
}
