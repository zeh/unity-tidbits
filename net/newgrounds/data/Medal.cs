using System;
using UnityEngine;

public class Medal {

    //public static const ICON_WIDTH:uint = 50;
    //public static const ICON_HEIGHT:uint = 50;
    public const string DIFFICULTY_EASY = "Easy";
    public const string DIFFICULTY_MODERATE = "Moderate";
    public const string DIFFICULTY_CHALLENGING = "Challenging";
    public const string DIFFICULTY_DIFFICULT = "Difficult";
    public const string DIFFICULTY_BRUTAL = "Brutal";
    private static string[] DIFFICULTIES = new string[] {DIFFICULTY_EASY, DIFFICULTY_MODERATE, DIFFICULTY_CHALLENGING, DIFFICULTY_DIFFICULT, DIFFICULTY_BRUTAL};
    //public static const DEFAULT_ICON:BitmapData = new DefaultMedalIcon(ICON_WIDTH, ICON_HEIGHT);


	// ================================================================================================================
	// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

	public Medal(Action __successCallback) {
	}

	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

}