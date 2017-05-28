using UnityEngine;

namespace Cryptoverse.Examples
{
	public class ServiceExample : MonoBehaviour
	{
		public string ApiUrl = "http://api.cryptoverse.io";

		ICache Cache;
		Service Service;
		
		void Awake()
		{
			// This GameObject will be used to run the coroutines required by the Service class,
			// so we make sure it doesn't get destroyed! Any GameObject that doesn't get destroyed
			// will work though.
			DontDestroyOnLoad(gameObject);
			// Create a cache, which is just a way to store data pulled from the server. Eventually
			// there should be a SqliteCache, since storing the blockchain in the memory will become
			// unfeasable.
			Cache = new MemoryCache();
			// Create the Service, passing the API's URL, the cache, and turn on verbose logging, so
			// we can debug and see what's happening. This will create a lot of console spam though!
			Service = new Service(this, ApiUrl, Cache, true);
			// Initialize Service, this only needs to be done once and will automatically syncronize
			// the latest star logs. Since initialization is done asyncronously, you can provide an
			// event for it to call when it is done syncronizing.
			Service.Initialize(OnInitialized);
		}

		#region Events
		void OnInitialized(Status status)
		{
			// The status variable tells us whether initialization succeeded or failed.
			if (status != Status.Success)
			{
				Debug.LogError("Example initialize failed with result: "+status);
				return;
			}
			Debug.Log("Example initialize completed with result: "+status);
			// Now that the Service class has been initialized, we can make calls to the cache to
			// retrieve the information that was syncronized. All the calls are asyncronous so they
			// won't block Unity's main thread and cause crazy lag. Right now it's not an issue, but
			// it will be once we're using a Sqlite database and pulling megabytes of data at a time!
			Cache.ReadStarLogs(OnReadAllStarLogs);
			// No need to wait for the other task to finish, we can trigger another request to the
			// cache, and it will resolve it in whatever order is fastest.
			Cache.ReadStarLogsHighest(OnReadHighestStarLogs);
			// You can easily chain these commands together as well, using lambda statements or
			// regular methods. Enjoy!
		}
		
		void OnReadAllStarLogs(Status status, StarLog[] starLogs)
		{
			if (status != Status.Success)
			{
				Debug.LogError("Reading star logs from cache failed with result: "+status);
				return;
			}
			Debug.Log("There are " + starLogs.Length + " star logs in the cache");
		}
		
		void OnReadHighestStarLogs(Status status, StarLog[] starLogs)
		{
			if (status != Status.Success)
			{
				Debug.LogError("Reading the highest star log from the cache failed with result: " + status);
				return;
			}
			if (starLogs.Length == 0) 
			{
				Debug.Log("There are zero star logs in the cache");
			}
			else
			{
				var first = starLogs[0];
				if (starLogs.Length == 1) Debug.Log("There is one star log at height " + first.Height + " with a hash of " + first.Hash);
				else Debug.Log("There are multiple star logs at height " + first.Height + ", one of them has a hash of " + first.Hash);
			}
		}
		#endregion
	}
}