using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour
{
    [SerializeField] private Text statusText;
	[SerializeField] private Color32 winnerColor;
	[SerializeField] private Color32 loserColor;

    void Start()
    {
		bool isWinner = PlayerState.Instance.winnerIndex == PlayerState.Instance.localIndex;

		if (isWinner)
		{
			statusText.text = "WINNER";
			statusText.color = winnerColor;
		}
		else
		{
			statusText.text = "LOSER";
			statusText.color = loserColor;
		}
    }
}
