using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerData 
{
	public int highestPosition;
	public int totalScore;
	public float soundVolume = 1;

	private int currentLevelIndex;
	public int CurrentLevelIndex
	{
		get
		{
			return currentLevelIndex;
		}

		set
		{
			if (value != currentLevelIndex)
			{
				currentLevelIndex = value;
				currentChallengeIndex = 0;
			}
		}
	}

	private int currentChallengeIndex;
	public int CurrentChallengeIndex
	{
		get
		{
			return currentChallengeIndex;
		}

		set
		{
			if (value != currentLevelIndex)
			{
				currentChallengeIndex = value;
			}
		}
	}
}