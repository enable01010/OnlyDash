using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : Singleton<GameData>
{
	//ÉKÅ[Éhêﬂópä÷êî
	public static bool G_isMenu() { return MenuManager.GetIsMenu(); }
	public static bool G_isTimeLine() { return TimeLineManager.GetIsTimeLine(); }
	public static bool G_isLoad() { return LibSceneManager.GetIsLoad(); }
	public static bool G_AllCheck() { return G_isMenu() || G_isTimeLine() || G_isLoad(); }
}
