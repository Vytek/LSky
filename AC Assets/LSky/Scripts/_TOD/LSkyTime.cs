

////////////////////
/// Time method. ///
////////////////////


using System;
using UnityEngine;


namespace AC.LSky
{

	public abstract class LSkyTime : MonoBehaviour 
	{


		public    bool  playTime      = true; // Progress time.
		public    bool  useSystemTime;
		public    float dayInSeconds  = 900;  // 60*15 = 900 (15 minutes).
		protected int   k_HoursPerDay = 24;   
		//-------------------------------------------------------------------

		[Range(0.0f, 24f)] public float timeline = 7.0f;
		//-------------------------------------------------------------------

		protected void ProgressTime()
		{


			if(useSystemTime) 
			{
				if(playTime) 
				{
					DateTime dateTime = DateTime.Now;
					timeline = (float)dateTime.Hour + (float)dateTime.Minute / 60;
				}
			} 
			else 
			{
				timeline = Mathf.Repeat (timeline, k_HoursPerDay);

				// Add time in timeline.
				if (playTime && Application.isPlaying && dayInSeconds != 0)
				{
					timeline += (Time.deltaTime / dayInSeconds) * k_HoursPerDay; 
				}
			}

		}
		//-------------------------------------------------------------------
	}
}