using Newtonsoft.Json;

namespace Cryptoverse
{
	public class Rules : Model<Rules>
	{
		[JsonProperty("difficulty_fudge")]
		public int DifficultyFudge;

		[JsonProperty("difficulty_duration")]
		public int DifficultyDuration;

		[JsonProperty("difficulty_interval")]
		public int DifficultyInterval;

		[JsonProperty("difficulty_start")]
		public int DifficultyStart;

		[JsonProperty("ship_reward")]
		public int ShipReward;

		[JsonProperty("cartesian_digits")]
		public int CartesianDigits;

		[JsonProperty("jump_cost_min")]
		public float JumpCostMinimum;

		[JsonProperty("jump_cost_max")]
		public float JumpCostMaximum;

		[JsonProperty("jump_distance_max")]
		public float JumpDistanceMaximum;

		[JsonProperty("star_logs_max_limit")]
		public int StarLogsLimitMaximum;

		[JsonProperty("events_max_limit")]
		public int EventsLimitMaximum;

		[JsonProperty("chains_max_limit")]
		public int ChainsLimitMaximum;
	}
}