using UnityEngine;
using UnityEngine.EventSystems;
using YG;

public class GameManager : Singleton<GameManager> 
{
	/* Public Objects & Variables */
	public GameObject blockBasePrefab;
	public GameObject blockPrefab;
	public GameObject perfectFxPrefab;
	public Material blockPrimaryMaterial;
	public Material blockSecondaryMaterial;
	public AudioClip blockDropAudioClip;

			//TODO: Change to public	
	public float blockSnapOffset = 0.1f;
	public float blockSpeedIncrement = 0.2f;
	public static bool isGameOver = false;

	[HideInInspector] public GameObject activeBlock;
	[HideInInspector] public int totalScore = 0;
	[HideInInspector] public int bestScore = 0;

	/* Private Objects & Variables */
	private GameObject oldBlock;
	private GameObject brokenBlock;
	private BlockProperties activeBlockProperties;
	private BlockMovement activeBlockMovement;
	private BlockProperties oldBlockProperties;


	private float imperfectBlockSpeed = 0;
	private float blockBaseHeight;
	private float blockHeight;
	private bool isFirstTap = true;

	/* Events */
	public delegate void Event();
	public static event Event onGameReadyEvent;
	public static event Event onGameStartedEvent;
	public static event Event onGameRunningEvent;
	public static event Event onGamePausedEvent;
	public static event Event onGameResumedEvent;
	public static event Event onGameOverEvent;
	public static event Event onTapEvent;
	public static event Event onScoreEvent;
	public static event Event onPerfectScoreEvent;
	public static event Event onBestScoreEvent;

	/* Actions */
	private void onGameReadyAction()
	{
		CameraManager.downSpeed = 100f;
		resetScene ();
		initialize ();

		onGameReadyEvent ();
	}

	private void onGameStartedAction()
	{
		updateHighestPointer (true);

		onGameStartedEvent ();
	}

	private void onGameRunningAction()
	{
		checkForBestScore ();
		snapToOldBlock ();
		resizeActiveBlock ();
		repositionActiveBlock ();
		createNewBlock (true);
		activeBlockMovement.shouldIncreaseSpeed = true;

		onGameRunningEvent ();
	}

	private void onGamePausedAction()
	{
		onGamePausedEvent ();
	}

	private void onGameResumedAction()
	{
		activeBlockMovement.shouldMove = true;

		onGameResumedEvent ();
	}

	private void onGameOverAction()
	{
		createBrokenBlock (activeBlockProperties.getScale().x);
		updateHighestPointer (true);
		SaveData ();
		Destroy (activeBlock);
		CameraManager.downSpeed = 2f;
		CameraManager.shouldResetPosition = true;

		onGameOverEvent ();
	}

	private void onTapAction()
	{
		onTapEvent ();
	}

	private void onScoreAction()
	{
		totalScore++;
		onScoreEvent ();
	}

	private void onPerfectScoreAction()
	{
		/* Perfect Effect */
		Vector3 fxPosition = activeBlock.transform.position;
		fxPosition.x += activeBlockProperties.getWidth () / 2;
		GameObject perfectFx = Instantiate (perfectFxPrefab, fxPosition, Quaternion.identity) as GameObject;

		onPerfectScoreEvent ();
	}

	private void onBestScoreAction()
	{
		bestScore = totalScore;
		onBestScoreEvent ();
	}

	public void onClickPlayButton()
	{
		onGameReadyAction ();
	}

	public void onClickPauseButton()
	{
		onGamePausedAction ();
	}

	public void onClickResumeButton()
	{
		onGameResumedAction ();
	}

    private void OnEnable()
    {
		YandexGame.GetDataEvent += LoadData;
    }

    /* MonoDevelop Functions */
    void Start () 
	{
		blockBaseHeight = blockBasePrefab.GetComponentInChildren<MeshFilter> ().mesh.bounds.extents.y;
		blockHeight = blockPrefab.GetComponentInChildren<Renderer> ().bounds.size.y;

        if (YandexGame.SDKEnabled == true)
        {
            LoadData();
        }

        initialize ();
		onGameReadyEvent ();
	}


    void Update () 
	{
		bool isTouchEvent = (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId));
		bool isClickEvent = EventSystem.current.IsPointerOverGameObject ();

		if (!isFirstTap && (isTouchEvent || isClickEvent))
		{
			return;
		}
			
		if (Input.GetButtonDown ("Fire1"))
		{
			onTapAction ();

			if (isFirstTap)
			{
				onGameStartedAction ();
				isFirstTap = false;
			}

			if (isGameOver != true)
			{
				if (activeBlockMovement != null)
				{
					activeBlockMovement.shouldMove = false;
				}

				checkForGameOver ();

				if (isGameOver == false)
				{
					onGameRunningAction ();
				} 
				else
				{
					onGameOverAction ();
				}
			}
		}
	}

	/* Game Manager Functions */
	void initialize()
	{
		totalScore = 0;
		imperfectBlockSpeed = 0;
		isGameOver = false;
		isFirstTap = true;

		checkForBestScore ();
		oldBlockInitialize ();
		createNewBlock (false);
		updateHighestPointer (false);
	}

	void oldBlockInitialize()
	{
		oldBlock = blockBasePrefab;
		oldBlockProperties = oldBlock.GetComponent<BlockProperties> ();
	}

	public void checkForBestScore()
	{
		if (totalScore > bestScore)
		{
			onBestScoreAction ();
		}
	}

	private void updateHighestPointer (bool visibility)
	{
		GameObject hightPoint = GameObject.Find ("Highest Point");

		if (visibility == false)
		{
			/* Moving out of screen */
			Vector3 position = hightPoint.transform.position;
			position.y = -10;
			hightPoint.transform.position = position;
		}
		else if (bestScore > 0)
		{
			float newY = blockBaseHeight + blockHeight * (bestScore + 1) - hightPoint.GetComponentInChildren<Renderer> ().bounds.size.y / 2;
			TextMesh hightPointCounter = hightPoint.GetComponentInChildren<TextMesh> ();
			hightPointCounter.text = bestScore.ToString ();

			Vector3 position = hightPoint.transform.position;
			position.y = newY;
			hightPoint.transform.position = position;
		}
	}

	public void snapToOldBlock ()
	{
		float oldBlockX = oldBlock.transform.position.x;
		float activeBlockX = activeBlock.transform.position.x;

		/* Perfect Block Placement */
		if ((oldBlockX - activeBlockX <= blockSnapOffset && oldBlockX - activeBlockX >= 0) || (activeBlockX - oldBlockX <= blockSnapOffset && activeBlockX - oldBlockX >= 0))
		{
			activeBlock.transform.position = new Vector3 (oldBlockX, activeBlock.transform.position.y, activeBlock.transform.position.z);
			onPerfectScoreAction ();
		} 
		/* Imperfect block speed increment */
		else
		{
			imperfectBlockSpeed += blockSpeedIncrement;
		}
	}

	public void repositionActiveBlock()
	{
		float oldBlockX = oldBlock.transform.position.x;
		float oldBlockWidth = oldBlockProperties.getWidth ();

		float activeBlockX = activeBlock.transform.position.x;
		float activeBlockWidth = activeBlockProperties.getWidth ();

		if (activeBlockX < oldBlockX)
		{
			activeBlock.transform.position = new Vector3 (oldBlockX, activeBlock.transform.position.y, activeBlock.transform.position.z);
		}
		else if (activeBlockX > oldBlockX)
		{
			activeBlock.transform.position = new Vector3 (oldBlockX + oldBlockWidth - activeBlockWidth, activeBlock.transform.position.y, activeBlock.transform.position.z);
			if (brokenBlock != null)
			{
				brokenBlock.transform.position = new Vector3 (activeBlock.transform.position.x + activeBlockWidth, activeBlock.transform.position.y, activeBlock.transform.position.z);
			}
		}
	}

	public void resizeActiveBlock()
	{
		float oldBlockX = oldBlock.transform.position.x;

		Vector3 activeBlockScale = activeBlockProperties.getScale();
		float activeBlockWidth = activeBlockProperties.getWidth ();
		float activeBlockX = activeBlock.transform.position.x;

		float offsetX = Mathf.Abs (oldBlockX - activeBlockX);
		float offsetPercent = offsetX / activeBlockWidth;
		float newScaleX = Mathf.Abs(1 - offsetPercent) * activeBlockScale.x;
		float brokenBlockScaleX = Mathf.Abs(offsetPercent) * activeBlockScale.x;

		if (newScaleX < activeBlockScale.x)
		{
			activeBlockProperties.setScale (new Vector3 (newScaleX, activeBlockScale.y, activeBlockScale.z));
			createBrokenBlock (brokenBlockScaleX);
			AudioSource.PlayClipAtPoint (blockDropAudioClip, activeBlock.transform.position, 1f);
		}
	}

	public void checkForGameOver()
	{
		if (oldBlock == null)
		{
			oldBlockInitialize ();
		}

		float oldBlockX = oldBlock.transform.position.x;
		float oldBlockWidth = oldBlockProperties.getWidth ();
		float activeBlockX = activeBlock.transform.position.x;
		float activeBlockWidth = activeBlockProperties.getWidth ();

		if (activeBlockX + activeBlockWidth < oldBlockX || activeBlockX > oldBlockX + oldBlockWidth)
		{
			isGameOver = true;
		} 
		else
		{
			onScoreAction ();
		}
	}

	public void createNewBlock(bool shouldResize)
	{
		if (activeBlock != null)
		{
			oldBlock = activeBlock;
			oldBlockProperties = activeBlockProperties;
		}
		
		activeBlock = Instantiate (blockPrefab) as GameObject;
		activeBlockProperties = activeBlock.GetComponent<BlockProperties> ();
		activeBlockMovement = activeBlock.GetComponent<BlockMovement> ();

		Renderer renderer = activeBlock.GetComponentInChildren<Renderer> ();

		int blockNumber = totalScore + 1;

		if (blockNumber % 2 != 0)
		{
			renderer.material = blockPrimaryMaterial;	
		} else
		{
			renderer.material = blockSecondaryMaterial;
		}

		if (oldBlock != null && shouldResize)
		{
			Vector3 blockScale = activeBlockProperties.getScale ();
			Vector3 oldBlockScale = oldBlockProperties.getScale ();
			activeBlockProperties.setScale (new Vector3 (oldBlockScale.x, blockScale.y, blockScale.z));
		}
			
		float width = activeBlockProperties.getWidth ();
		float originalWidth = activeBlockProperties.getOriginalSize().x;
		activeBlockMovement.maxMovementRight += originalWidth - width;

		Vector3 pos = activeBlock.transform.position;
		activeBlockMovement.direction = Random.Range (0, 2) > 0 ? 1 : -1;
		if (activeBlockMovement.direction > 0)
		{
			pos.x = activeBlockMovement.maxMovementRight;
		}
		else
		{
			pos.x = activeBlockMovement.maxMovementLeft;
		}

		activeBlock.transform.position = new Vector3 (pos.x, blockBaseHeight + pos.y + (activeBlockProperties.getHeight () * blockNumber), pos.z);
				
		activeBlockMovement.speed += imperfectBlockSpeed;
		activeBlockMovement.shouldMove = true;
	}

	public void createBrokenBlock (float newScaleX)
	{
		brokenBlock = Instantiate (blockPrefab) as GameObject;
		Vector3 activeBlockScale = activeBlockProperties.getScale ();

		brokenBlock.transform.position = activeBlock.transform.position;
		BlockProperties brokenBlockProperties = brokenBlock.GetComponent<BlockProperties> ();
		brokenBlockProperties.setScale (new Vector3(newScaleX, activeBlockScale.y, activeBlockScale.z));

		Material material = brokenBlock.GetComponentInChildren<Renderer> ().material;
		material.color = activeBlock.GetComponentInChildren<Renderer> ().material.color;

		/* Set Rendering Mode to Fade */
		material.SetFloat("_Mode", 2);
		material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
		material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		material.SetInt("_ZWrite", 0);
		material.DisableKeyword("_ALPHATEST_ON");
		material.EnableKeyword("_ALPHABLEND_ON");
		material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		material.renderQueue = 3000;

		BrokenBlockMovement brokenBlockMovement = brokenBlock.gameObject.AddComponent <BrokenBlockMovement>() as BrokenBlockMovement;
		FadeObjectInOut fadeObjectInOut = brokenBlock.gameObject.AddComponent <FadeObjectInOut>() as FadeObjectInOut;
	}

	void resetScene()
	{
		oldBlock = blockBasePrefab;

		foreach(GameObject block in GameObject.FindGameObjectsWithTag("Block"))
		{
			Destroy (block);
		}
	}

	public void SaveData()
	{
        YandexGame.savesData.highestPosition = bestScore;
        YandexGame.SaveProgress();
    }

	public void LoadData()
	{
        bestScore = YandexGame.savesData.highestPosition;
    }
}