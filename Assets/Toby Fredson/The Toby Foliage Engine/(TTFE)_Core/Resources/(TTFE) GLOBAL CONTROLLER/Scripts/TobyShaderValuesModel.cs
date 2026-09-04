using UnityEngine;

namespace TobyFredson
{
	[System.Serializable]
	public struct TobyShaderValuesModel
	{
		public float season;
		public TobyWindType windType;
		public float windStrength;
		public float windSpeed;
		public float windMotion;
		public Vector3 windDirection;

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) return false;
			var other = (TobyShaderValuesModel)obj;
			return windType == other.windType &&
				Mathf.Approximately(windStrength, other.windStrength) &&
				Mathf.Approximately(windSpeed, other.windSpeed) &&
				Mathf.Approximately(windMotion, other.windMotion) &&
				Mathf.Approximately(season, other.season) &&
				windDirection == other.windDirection;
		}

		public override int GetHashCode()
		{
			return System.HashCode.Combine(
				windType, windStrength, windSpeed, windMotion, season, windDirection);
		}
	}

	public enum TobyWindType
	{
		GentleBreeze,
		WindOff
		// StrongWind was removed
	}
}