using Newtonsoft.Json;

namespace Cryptoverse
{
	public class StarLog : Model<StarLog>
	{
		[JsonProperty("log_header")]
		public string LogHeader;
		
		[JsonProperty("nonce")]
		public int Nonce;
		
		[JsonProperty("hash")]
		public string Hash;
		
		[JsonProperty("previous_hash")]
		public string PreviousHash;
		
		[JsonProperty("difficulty")]
		public int Difficulty;
		
		[JsonProperty("height")]
		public int Height;
		
		[JsonProperty("version")]
		public int Version;
		
		[JsonProperty("time")]
		public int Time;
		
		[JsonProperty("create_time")]
		public int CreateTime;
		
		[JsonProperty("events_hash")]
		public string EventsHash;
		
		[JsonProperty("events")]
		public Event[] Events;
	}
}