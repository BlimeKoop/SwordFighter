using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerUIController : MonoBehaviour
{
	public GameObject _playerInfoPrefab;
	private static GameObject playerInfoPrefab;
	private static Transform transform;
	
	private static int playerCount;

	private void Awake()
	{
		playerInfoPrefab = _playerInfoPrefab;
		PlayerUIController.transform = gameObject.transform;
	}
	
	public static PlayerUI AddPlayerUI(Player player)
	{
		GameObject obj = Instantiate(playerInfoPrefab);
		
		obj.transform.parent = transform;
		obj.transform.localScale = Vector3.one * 0.6f;
		
		PlayerUI ui = obj.GetComponent<PlayerUI>();
		ui.SetName(player.NickName);
		
		playerCount++;
		
		return ui;
	}
	
	public static void RemovePlayerUI(PlayerUI ui)
	{
		Destroy(ui.gameObject);
	}
	
	public static void UpdatePlayerLives(int lives, PlayerUI ui)
	{
		ui.SetLives(lives);
	}
}
