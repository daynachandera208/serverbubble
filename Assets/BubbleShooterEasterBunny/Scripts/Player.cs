// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.10
// 

using Colyseus.Schema;

public partial class Player : Schema {
	[Type(0, "string")]
	public string sessionId = default(string);

	[Type(1, "uint16")]
	public ushort currentLevel = default(ushort);

	[Type(2, "uint16")]
	public ushort FinishedLevels = default(ushort);

	[Type(3, "uint32")]
	public uint currentLevelScore = default(uint);

	[Type(4, "uint32")]
	public uint TotalScore = default(uint);

	[Type(5, "uint32")]
	public uint TotalStars = default(uint);

	[Type(6, "uint32")]
	public uint currentStars = default(uint);

	[Type(7, "uint8")]
	public uint totalGameOvers = default(uint);

	[Type(8, "uint16")]
	public ushort secondsLeft = default(ushort);
}

