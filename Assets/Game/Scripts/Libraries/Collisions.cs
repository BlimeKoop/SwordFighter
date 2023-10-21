using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisions
{
	public static int PlayerLayer = 3, SwordLayer = 6, IgnoreSwordLayer = 7, OnlySwordLayer = 8, IgnoreSelfLayer = 9, PhaseLayer = 10;
	
	public static Vector3 InterpolatedNormal(Collision collision, int contactLimit = 10)
	{
		ContactPoint[] contacts = collision.contacts;
		Vector3 normalR = Vector3.zero;
		
		int counter = 0;
		
		foreach(var contact in contacts)
		{
			counter++;
			
			normalR += contact.normal;
			
			if (counter == contactLimit)
				break;
		}
		
		normalR = (normalR / contacts.Length).normalized;
		
		return normalR;
	}
}
