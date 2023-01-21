using UnityEngine;
using UnityEngine.UI;
using YG;

public class UIManager : MonoBehaviour 
{
	public Image pauseScreen;
	public Image gameOverPanel;
	public Button pauseButton;
	public Text infoText;
	public Text counterText;
	public Button soundOnButton;
	public Button soundOffButton;

	void OnEnable()
	{
		GameManager.onGameReadyEvent += onGameReadyEvent;
		GameManager.onGameStartedEvent += onGameStartedEvent;
		GameManager.onGameRunningEvent += onGameRunningEvent;
		GameManager.onGamePausedEvent += onGamePausedEvent;
		GameManager.onGameResumedEvent += onGameResumedEvent;
		GameManager.onGameOverEvent += onGameOverEvent;
		GameManager.onTapEvent += onTapEvent;
		GameManager.onScoreEvent += onScoreEvent;
		GameManager.onPerfectScoreEvent += onPerfectScoreEvent;
		GameManager.onBestScoreEvent += onBestScoreEvent;

		YandexGame.GetDataEvent += updateSoundUI;
    }

	void OnDisable()
	{
		GameManager.onGameReadyEvent -= onGameReadyEvent;
		GameManager.onGameStartedEvent -= onGameStartedEvent;
		GameManager.onGameRunningEvent -= onGameRunningEvent;
		GameManager.onGamePausedEvent -= onGamePausedEvent;
		GameManager.onGameResumedEvent -= onGameResumedEvent;
		GameManager.onGameOverEvent -= onGameOverEvent;
		GameManager.onTapEvent -= onTapEvent;
		GameManager.onScoreEvent -= onScoreEvent;
		GameManager.onPerfectScoreEvent -= onPerfectScoreEvent;
		GameManager.onBestScoreEvent -= onBestScoreEvent;
		YandexGame.GetDataEvent -= updateSoundUI;
    }

	void Start () 
	{
		initialize ();

		if (YandexGame.SDKEnabled == true)
		{
            updateSoundUI();
        }
	}

	void initialize()
	{
		counterText.gameObject.SetActive (false);
		pauseButton.gameObject.SetActive (false);
		pauseScreen.gameObject.SetActive (false);
		gameOverPanel.gameObject.SetActive (false);
		infoText.gameObject.SetActive (true);
		showBestScoreText (false);

		updateScoreUI ();
	}

	public void updateScoreUI ()
	{
		counterText.text = GameManager.Instance.totalScore.ToString ();

		Text hightScoreText = gameOverPanel.GetComponentsInChildren<Text> () [2];
		hightScoreText.text = GameManager.Instance.totalScore.ToString ();
	}

	public void updateBestScoreUI()
	{
		Text bestScoreText = gameOverPanel.GetComponentsInChildren<Text> () [4];
		bestScoreText.text = GameManager.Instance.bestScore.ToString ();
	}

	public void updateSoundUI()
	{
		bool sounds = YandexGame.savesData.soundsIsON;

		if (sounds)
		{
			soundOnButton.gameObject.SetActive (false);
			soundOffButton.gameObject.SetActive (true);
			AudioListener.volume = 1;
		} 
		else
		{
			soundOnButton.gameObject.SetActive (true);
			soundOffButton.gameObject.SetActive (false);
			AudioListener.volume = 0;
		}
	}

	public void onClickSoundOnButton()
	{
		soundOnButton.gameObject.SetActive(false);
		soundOffButton.gameObject.SetActive(true);
		YandexGame.savesData.soundsIsON = true;
		AudioListener.volume = 1f;
		YandexGame.SaveProgress();
	}

	public void onClickSoundOffButton()
	{
        soundOffButton.gameObject.SetActive(false);
		soundOnButton.gameObject.SetActive(true);
		YandexGame.savesData.soundsIsON = false;
		AudioListener.volume = 0;
		YandexGame.SaveProgress();
    }

	public void showBestScoreText( bool visibility )
	{
		Text newhighestText = gameOverPanel.GetComponentsInChildren<Text> () [5];
		newhighestText.enabled = visibility;
	}


	/* Events */

	public void onHomeEvent()
	{
	}

	public void onGameReadyEvent()
	{
		initialize ();
	}

	public void onGameStartedEvent ()
	{
		infoText.gameObject.SetActive (false);

		updateScoreUI ();

		if (!pauseButton.gameObject.activeSelf)
		{
			pauseButton.gameObject.SetActive (true);
			counterText.gameObject.SetActive (true);
		}
	}
		
	public void onGameRunningEvent ()
	{
	}

	public void onGamePausedEvent()
	{
		pauseScreen.gameObject.SetActive (true);
		pauseButton.gameObject.SetActive (false);
	}

	public void onGameResumedEvent()
	{
		pauseScreen.gameObject.SetActive (false);
		pauseButton.gameObject.SetActive (true);
	}

	public void onGameOverEvent()
	{
		updateBestScoreUI ();
		pauseButton.gameObject.SetActive (false);
		counterText.gameObject.SetActive (false);
		gameOverPanel.gameObject.SetActive (true);
		YandexGame.FullscreenShow();
	}

	public void onTapEvent()
	{
	}

	public void onScoreEvent()
	{
		updateScoreUI ();
	}

	public void onPerfectScoreEvent()
	{
		updateScoreUI ();
	}

	public void onBestScoreEvent()
	{
		showBestScoreText (true);
	}
}