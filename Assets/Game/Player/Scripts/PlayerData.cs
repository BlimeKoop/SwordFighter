using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMatchData
{
	public int lives;
	public PlayerUI ui;
	public bool eliminated;
	
	public PlayerMatchData(int lives, PlayerUI ui)
	{
		this.lives = lives;
		this.ui = ui;
	}
}

public class PlayerData : MonoBehaviourPunCallbacks
{
	private PhotonView photonView;
	private static Dictionary<int, PlayerMatchData> playerMatchData = new Dictionary<int, PlayerMatchData>();
	
	private static int eliminatedPlayerCount = 0;
	
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		photonView = GetComponent<PhotonView>();
	}

	[PunRPC]
	public void AddPlayer(Player player)
	{
		PlayerUI ui = PlayerUIController.AddPlayerUI(player);
		
		if (!playerMatchData.ContainsKey(player.ActorNumber))
		{
			playerMatchData[player.ActorNumber] = new PlayerMatchData(3, ui);
		}
		else
		{
			playerMatchData[player.ActorNumber].ui = ui;
			PlayerUIController.UpdatePlayerLives(playerMatchData[player.ActorNumber].lives, playerMatchData[player.ActorNumber].ui);
		}
	}
	
	[PunRPC]
	public void RemovePlayer(Player player)
	{
		Debug.Log("Haven't implemented this yet");
		// PlayerUIController.RemovePlayerUI(playerMatchData[player.ActorNumber].ui);
	}

	[PunRPC]
	public void IncreaseLives(int actorNumber)
	{
		SetLives(playerMatchData[actorNumber].lives + 1, actorNumber);
	}
	
	[PunRPC]
	public void DecreaseLives(int actorNumber)
	{
		SetLives(playerMatchData[actorNumber].lives - 1, actorNumber);
	}
	
	[PunRPC]
	public void SetLives(int setTo, int actorNumber)
	{
		playerMatchData[actorNumber].lives = Mathf.Max(0, setTo);
		
		if (playerMatchData[actorNumber].lives == 0)
		{
			playerMatchData[actorNumber].eliminated = true;
			
			eliminatedPlayerCount++;
			
			if (MatchFinished())
				MatchManager.EndMatch();
		}
		
		UpdatePlayerLifeUI(playerMatchData[actorNumber]);
	}
	
	private void UpdatePlayerLifeUI(PlayerMatchData matchData)
	{
		PlayerUIController.UpdatePlayerLives(matchData.lives, matchData.ui);
	}
	
	public static bool MatchFinished()
	{
		return eliminatedPlayerCount >= playerMatchData.Keys.Count - 1;
	}
}
