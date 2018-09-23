public sealed class PlayerState
{
	private static readonly PlayerState instance = new PlayerState();

	public int playerIndex = 0;

	static PlayerState()
	{
	}

	private PlayerState()
	{
	}

	public static PlayerState Instance
	{
		get
		{
			return instance;
		}
	}
}