using System;

namespace Cryptoverse
{
	public interface ICache
	{
		Rules Rules { get; }
		/// <summary>
		/// Writes the rules to the cache.
		/// </summary>
		/// <param name="done">Done.</param>
		/// <param name="rules">Rules.</param>
		void WriteRules(Rules rules, Action<Status> done = null);
		/// <summary>
		/// Writes the star logs to the cache.
		/// </summary>
		/// <param name="done">Done.</param>
		/// <param name="starLogs">Star logs.</param>
		void WriteStarLogs(Action<Status> done = null, params StarLog[] starLogs);

		/// <summary>
		/// Reads the rules from the cache.
		/// </summary>
		/// <param name="done">Done.</param>
		void ReadRules(Action<Status, Rules> done);
		/// <summary>
		/// Reads all star logs with the matching system hashes from the cache.
		/// </summary>
		/// <param name="done">Done.</param>
		/// <param name="systemHashes">System hashes.</param>
		void ReadStarLogs(Action<Status, StarLog[]> done, params string[] systemHashes);
		/// <summary>
		/// Reads all star logs with the matching heights from the cache.
		/// </summary>
		/// <param name="done">Done.</param>
		/// <param name="heights">Heights.</param>
		void ReadStarLogsAtHeight(Action<Status, StarLog[]> done, params int[] heights);
		/// <summary>
		/// Reads the highest star logs from the cache.
		/// </summary>
		/// <param name="done">Done.</param>
		void ReadStarLogsHighest(Action<Status, StarLog[]> done);
		/// <summary>
		/// Reads the star log with the most recent time from the cache.
		/// </summary>
		/// <param name="done">Done.</param>
		void ReadStarLogLatest(Action<Status, StarLog[]> done);
	}

	public abstract class Cache : ICache
	{
		protected void OnDone(Status status, Action<Status> done)
		{
			if (done != null) done(status);
		}

		protected void OnDone<T>(Status status, T result, Action<Status, T> done)
		{
			if (done != null) done(status, result);
		}

		public Rules Rules { get; protected set; }

		public abstract void WriteRules(Rules rules, Action<Status> done = null);
		public abstract void WriteStarLogs(Action<Status> done = null, params StarLog[] starLogs);

		public abstract void ReadRules(Action<Status, Rules> done);
		public abstract void ReadStarLogs(Action<Status, StarLog[]> done, params string[] systemHashes);
		public abstract void ReadStarLogsAtHeight(Action<Status, StarLog[]> done, params int[] heights);
		public abstract void ReadStarLogsHighest(Action<Status, StarLog[]> done);
		public abstract void ReadStarLogLatest(Action<Status, StarLog[]> done);
	}
}