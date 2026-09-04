namespace TobyFredson
{
	public static class TobyConstants
	{
		public const string SHADER_VAR_FLOAT_SEASON           = "_SeasonChangeGlobal";
		public const string SHADER_VAR_FLOAT_WIND_STRENGTH    = "_GlobalWindStrength";
		public const string SHADER_VAR_FLOAT_WIND_SPEED       = "_StrongWindSpeed";  // still used – rename later if desired
		public const string SHADER_VAR_FLOAT_WIND_MOTION      = "_WindMotion";
		public const string SHADER_VAR_VECTOR_WIND_DIRECTION  = "_WindDirection";

		public const string SHADER_WIND_TYPE_VALUE_GENTLEBREEZE = "_WINDTYPE_GENTLEBREEZE";
		public const string SHADER_WIND_TYPE_VALUE_WINDOFF     = "_WINDTYPE_WINDOFF";
	}
}