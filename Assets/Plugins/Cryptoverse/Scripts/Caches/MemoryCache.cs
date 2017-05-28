using System;
using System.Linq;
using System.Collections.Generic;

namespace Cryptoverse
{
	/// <summary>
	/// A bacis memory cache for storing information about the blockchain. 
	/// </summary>
	/// <remarks>
	/// This should be used lightly, since the blockchain may grow larger than can be stored in memory.
	/// </remarks>
	public class MemoryCache : Cache
	{
		List<StarLog> StarLogs = new List<StarLog>();

		public override void WriteRules(Rules rules, Action<Status> done = null)
		{
			Rules = rules;
			OnDone(Status.Success, done);
		}

		public override void WriteStarLogs(Action<Status> done = null, params StarLog[] starLogs)
		{
			// Get all existing star log hashes.
			var allHashes = StarLogs.Select(s => s.Hash);
			// Get all hashes that are already cached and remove them.
			foreach (var replacement in starLogs.Where(s => allHashes.Contains(s.Hash)))
			{
				StarLogs.Remove(StarLogs.First(s => s.Hash == replacement.Hash));
			}
			StarLogs.AddRange(starLogs);
			OnDone(Status.Success, done);
		}

		public override void ReadRules(Action<Status, Rules> done)
		{
			OnDone(Status.Success, Rules, done);
		}

		public override void ReadStarLogs(Action<Status, StarLog[]> done, params string[] systemHashes)
		{
			var matches = (systemHashes == null || systemHashes.Length == 0) ? StarLogs : StarLogs.Where(s => systemHashes.Contains(s.Hash));
			var starLogs = new List<StarLog>();
			foreach (var starLog in matches) starLogs.Add(starLog.Clone());
			OnDone(Status.Success, starLogs.ToArray(), done);
		}

		public override void ReadStarLogsAtHeight(Action<Status, StarLog[]> done, params int[] heights)
		{
			var matches = (heights == null || heights.Length == 0) ? StarLogs : StarLogs.Where(s => heights.Contains(s.Height));
			var starLogs = new List<StarLog>();
			foreach (var starLog in matches) starLogs.Add(starLog.Clone());
			OnDone(Status.Success, starLogs.ToArray(), done);
		}

		public override void ReadStarLogsHighest(Action<Status, StarLog[]> done)
		{
			var highest = StarLogs.Max(s => s.Height);
			var starLogs = new List<StarLog>();
			foreach (var starLog in StarLogs.Where(s => s.Height == highest)) starLogs.Add(starLog.Clone());
			OnDone(Status.Success, starLogs.ToArray(), done);
		}
		
		public override void ReadStarLogLatest(Action<Status, StarLog[]> done)
		{
			var latest = StarLogs.Count == 0 ? 0 : StarLogs.Max(s => s.Time);
			var starLogs = new List<StarLog>();
			foreach (var starLog in StarLogs.Where(s => s.Time == latest)) starLogs.Add(starLog.Clone());
			OnDone(Status.Success, starLogs.ToArray(), done);
		}
	}
}