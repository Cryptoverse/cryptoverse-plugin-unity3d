using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Cryptoverse
{
	public class Service
	{
		public bool Initialized { get; private set; }
		public string ApiUrl { get; private set; }

		string RulesUrl { get { return ApiUrl + "/rules"; } }
		string StarLogsUrl { get { return ApiUrl + "/star-logs"; } }

		MonoBehaviour Behaviour;
		ICache Cache;
		bool Verbose;
		bool Running;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Cryptoverse.Service"/> class.
		/// </summary>
		/// <param name="behaviour">Behaviour used for coroutines, make sure this does not get destroyed, or the service class will be unable to make requests.</param>
		/// <param name="apiUrl">API URL.</param>
		/// <param name="cache">Cache for storing results of syncronization.</param>
		/// <param name="verbose">If set to <c>true</c> silent.</param>
		public Service(MonoBehaviour behaviour, string apiUrl, ICache cache, bool verbose = false)
		{
			Behaviour = behaviour;
			ApiUrl = apiUrl;
			Cache = cache;
			Verbose = verbose;
		}

		public void Initialize(Action<Status> done = null)
		{
			if (Initialized)
			{
				Debug.LogWarning("Already initialized");
				return;
			}

			GetRules((status, rules) =>
			{
				if (status != Status.Success)
				{
					Debug.LogError("Failed to get rules");
					CompleteInitialize(status, done);
					return;
				}
				Cache.WriteRules(rules, writeRulesStatus =>
				{
					if (writeRulesStatus != Status.Success)
					{
						Debug.LogError("Failed to get write rules to cache");
						CompleteInitialize(writeRulesStatus, done);
						return;
					}
					Syncronize(synchronizeStatus =>
					{
						if (synchronizeStatus != Status.Success)
						{
							Debug.LogError("Failed to synchronize");
							CompleteInitialize(synchronizeStatus, done);
							return;
						}
						CompleteInitialize(status, done);
					});
				});
			});

		}

		public void Syncronize(Action<Status> done = null)
		{
			Cache.ReadStarLogLatest((status, latestResults) =>
			{
				if (status != Status.Success)
				{
					OnDone(status, done);
					return;
				}
				var latest = latestResults.Length == 0 ? 0 : latestResults[0].Time;
				Syncronize(done, latest, Cache.Rules.StarLogsLimitMaximum, 0);
			});
		}
		
		void Syncronize(Action<Status> done, int latest, int lastCount, int offset)
		{
			if (Cache.Rules.StarLogsLimitMaximum != lastCount)
			{
				OnDone(Status.Success, done);
				return;
			}
			GetStarLogs(
				(status, starlogs) =>
				{
					if (status != Status.Success)
					{
						OnDone(status, done);
						return;
					}
					Cache.WriteStarLogs(
						starLogWriteStatus => 
						{
							Syncronize(done, latest, starlogs.Length, offset + starlogs.Length);
						},
						starlogs
					);
				},
				latest,
				Cache.Rules.StarLogsLimitMaximum,
				offset
			);
		}

		#region Utility
		void OnDone(Status status, Action<Status> done)
		{
			if (done != null) done(status);
		}
		
		object Sanitize(int? parameter) { return parameter.HasValue ? (object)parameter.Value : null; }
		
		string BuildUrl(string url, params object[] parameters)
		{
			if (parameters.Length % 2 != 0) throw new ArgumentException("An even number of paremeters must be passed");
			var hasStarted = false;
			for (var i = 0; i < parameters.Length; i += 2)
			{
				var key = parameters[i];
				var value = parameters[i + 1];
				if (value == null) continue;
				if (!hasStarted)
				{
					url += "?";
					hasStarted = true;
				}
				url += key + "=" + value + "&";
			}
			return url.Trim('&');
		}
		#endregion

		#region Requests
		void GetRequest<T>(string url, Action<Status, T> done = null) where T : class
		{
			Behaviour.StartCoroutine(OnGetRequest(url, CompleteGetRequest, done));
		}
		#endregion
		
		#region Endpoints
		void GetRules(Action<Status, Rules> done = null)
		{
			GetRequest(RulesUrl, done);
		}
		
		void GetStarLogs(Action<Status, StarLog[]> done = null, int? sinceTime = null, int? limit = null, int? offset = null)
		{
			GetRequest(BuildUrl(StarLogsUrl, "since_time", Sanitize(sinceTime), "limit", Sanitize(limit), "offset", Sanitize(offset)), done);
		}
		#endregion
		
		#region Enumerators
		IEnumerator OnUpdate()
		{
			while (Running)
			{
				 // TODO: Update logic for threading.
				yield return new WaitForEndOfFrame();
			}
		}

		IEnumerator OnGetRequest<T>(string url, Action<string, Status, string, Action<Status, T>> converter, Action<Status, T> done = null)
		{
			var request = new WWW(url);
			yield return request;

			if (done != null)
			{
				if (string.IsNullOrEmpty(request.error)) converter(url, Status.Success, request.text, done);
				else converter(url, Status.Error, request.error, done);
			}
		}
		#endregion
		
		#region Complete
		void CompleteInitialize(Status status, Action<Status> done = null)
		{
			if (status != Status.Success)
			{
				Debug.LogError("Failed to initialize with result: " + status);
				if (done != null) done(status);
				return;
			}

			if (Verbose) Debug.Log("Initialized with result: " + status);
			Initialized = true;
			Running = true;
			Behaviour.StartCoroutine(OnUpdate());

			if (done != null) done(status);
		}

		void CompleteGetRequest<T>(string url, Status status, string result, Action<Status, T> done = null) where T : class
		{
			if (done == null) return;

			if (status != Status.Success)
			{
				Debug.LogError("Failed to make get request to \"" + url + "\", the following error occured:\n" + result);
				done(status, null);
				return;
			}
			if (string.IsNullOrEmpty(result))
			{
				done(status, null);
				return;
			}
			T deserialized = null;
			try { deserialized = JsonConvert.DeserializeObject<T>(result); }
			catch (Exception e)
			{
				Debug.LogException(e);
				done(Status.Error, null);
				return;
			}
			done(status, deserialized);
		}
		#endregion
	}
}