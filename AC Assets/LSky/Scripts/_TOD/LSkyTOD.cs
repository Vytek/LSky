

///////////////////////////
/// Simple Time of day. ///
///////////////////////////


using System;
using UnityEngine;


namespace AC.LSky
{

	[AddComponentMenu ("AC/LSky/Time Of Day")]
	[ExecuteInEditMode]	public class LSkyTOD : LSkyTime
	{

		private LSky m_SkyManager = null;
		private Transform m_Transform = null;

		public    bool  useSystemTime;
		[Range(-12f, 12f)] public int UTC = 0;

		public float Timeline{ get{return timeline + UTC; } }

		public float EVALUATE_TIME_BY_TIMELINE{ get { return timeline/(k_HoursPerDay); }}
		//-----------------------------------------------------------------------------------------

	

		[LSkyFloatAttribute(-180f, 180f, 0.0f, 0.0f, 1.0f, 360f, DefautlColors.yellow)]
		public LSkyFloat equatorPos = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.0f,
			curve        = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 0.0f),
			evaluateTime = 0.0f

		};

		[Range(-180f, 180f)] public float orientation = 0.0f;

		//-----------------------------------------------------------------------------------------

		[Tooltip("Rotate moon in the opposite sun direction")]
		public bool autoRotateMoon;

		[LSkyFloatAttribute(-180f, 180f, 0.0f, 0.0f, 1.0f, 360f, DefautlColors.yellow)]
		public LSkyFloat moonXRot = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.0f,
			curve        = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 0.0f),
			evaluateTime = 0.0f

		};


		[LSkyFloatAttribute(-180f, 180f, 0.0f, 0.0f, 1.0f, 360f, DefautlColors.yellow)]
		public LSkyFloat moonYRot = new LSkyFloat()
		{

			valueType    = LSkyValueType.Value,
			inputValue   = 0.0f,
			curve        = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 0.0f),
			evaluateTime = 0.0f

		};

		public float XRot{ get{ return Timeline * (360f / k_HoursPerDay); } }

		public Quaternion SunRotation
		{

			get 
			{

				equatorPos.evaluateTime = EVALUATE_TIME_BY_TIMELINE;


				float x = XRot - 90f;
				float y = equatorPos.OutputValue;

				return Quaternion.Euler(x, 0, 0) * Quaternion.Euler(0, y, 0);
			}
		}
		//-----------------------------------------------------------------------------------------


		public Quaternion MoonRotation
		{

			get 
			{

				moonXRot.evaluateTime = EVALUATE_TIME_BY_TIMELINE;
				moonYRot.evaluateTime = EVALUATE_TIME_BY_TIMELINE;

				float x = moonXRot.OutputValue + 90;
				float y = moonYRot.OutputValue;

				return (Quaternion.Euler(x, 0, 0) * Quaternion.Euler(0, y, 0));
			}
		}

		public Quaternion MoonRotationOpposiveSun
		{


			get 
			{

				float x = XRot - 270f;
				float y = -equatorPos.OutputValue;
				return Quaternion.Euler (x, 0, 0) * Quaternion.Euler (0, y, 0);

			}

		}

		//-----------------------------------------------------------------------------------------

	//	public DateTime SystemDateTime{ get{ return DateTime.Now; } }

		public int CurrentHour{ get{ return (int)Mathf.Floor(timeline); } }
		public int CurrentMinute{ get{ return (int)Mathf.Floor( (timeline - CurrentHour) * 60); } }
		//-----------------------------------------------------------------------------------------


		void Update()
		{

			if(!CheckComponents())
			{
				m_Transform  = this.transform;
				m_SkyManager = GetComponent<LSky>();
				return;
			}
				
			ProgressTime(useSystemTime);

			m_SkyManager.SunLightTransform.localRotation  = SunRotation;
			m_SkyManager.MoonLightTransform.localRotation = autoRotateMoon ? MoonRotationOpposiveSun : MoonRotation;
			m_Transform.localEulerAngles                  = new Vector3(0.0f, orientation, 0.0f);
		}
			
		bool CheckComponents()
		{


			if(m_Transform == null)
				return false;

			if(m_SkyManager == null)
				return false;

			if(!m_SkyManager.IsReady)
				return false;

			return true;
		}
	}
}