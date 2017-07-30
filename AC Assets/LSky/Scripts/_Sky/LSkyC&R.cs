
/////////////////////////////////
/// Resources and components. ///
/////////////////////////////////

using System;
using UnityEngine;


namespace AC.LSky
{

	public partial class LSky : MonoBehaviour 
	{

		public bool      applySkybox    = true;  // Send skybox material to Lighting window.
		public Material  skyboxMaterial = null;  // Skybox material.
		public Texture2D moonTexture    = null;  // Moon texture.
		public Cubemap   outerSpaceCube = null;  // RGB: Nebula, Alpha: Stars field.
		public Cubemap   starsNoiseCube = null;  // Stars noise texture.
		//------------------------------------------------------------------------------------

		[SerializeField] private Light m_SunLight = null;        // Sun light component.
		private Transform m_SunLightTransform;                   // Sun light transform component.
		public  Transform SunLightTransform
		{ 
			get{ return m_SunLightTransform; } 
			set 
			{
				m_SunLightTransform.eulerAngles      = value.eulerAngles;
				m_SunLightTransform.localEulerAngles = value.localEulerAngles;
				m_SunLightTransform.rotation         = value.rotation;
				m_SunLightTransform.localRotation    = value.localRotation;
			}
		}

		[SerializeField] private Light m_MoonLight = null;       // Moon light component.
		private Transform m_MoonLightTransform;                  // Moon light transform component.
		public  Transform MoonLightTransform
		{ 
			get{ return m_MoonLightTransform; } 
			set 
			{
				m_MoonLightTransform.eulerAngles      = value.eulerAngles;
				m_MoonLightTransform.localEulerAngles = value.localEulerAngles;
				m_MoonLightTransform.rotation         = value.rotation;
				m_MoonLightTransform.localRotation    = value.localRotation;
			}
		}
		//------------------------------------------------------------------------------------

		// Cache necessary components.
		void CacheComponents()
		{
			if(m_SunLight  != null) 
				m_SunLightTransform  = m_SunLight.transform;
			else
				m_SunLightTransform  = null;

			if(m_MoonLight != null) 
				m_MoonLightTransform = m_MoonLight.transform;
			else
				m_MoonLightTransform = null;

			if(!IsReady)
				enabled = false;
		}
		//------------------------------------------------------------------------------------


		// Check components and resources.
		bool CheckResources()
		{

			if(m_SunLight == null)
				return false;

			if(m_SunLightTransform == null)
				return false;

			if(m_MoonLight == null)
				return false;

			if(m_MoonLightTransform == null)
				return false;

			if(moonTexture == null)
				return false;

			if(skyboxMaterial == null)
				return false;

			if(outerSpaceCube == null)
				return false;

			if(starsNoiseCube == null)
				return false;

			return true;
		}
		//------------------------------------------------------------------------------------

		public bool IsReady
		{
			get{ return CheckResources(); }
		}
		//------------------------------------------------------------------------------------
	}
}
