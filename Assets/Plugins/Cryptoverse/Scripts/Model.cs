namespace Cryptoverse
{
	public abstract class Model<T> where T : class
	{
		public T Clone() { return MemberwiseClone() as T; }
	}
}