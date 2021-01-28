// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.10
// 

using Colyseus.Schema;

public partial class Bot : Schema {
	[Type(0, "float32")]
	public float currentLevelMoveMinScore = default(float);

	[Type(1, "float32")]
	public float currentLevelMoveMaxScore = default(float);

	[Type(2, "float32")]
	public float currentLevelMoveMinTime = default(float);

	[Type(3, "float32")]
	public float currentLevelMoveMaxTime = default(float);

	[Type(4, "float32")]
	public float currentLevelHardness = default(float);

	[Type(5, "float32")]
	public float currentLevelMinScore = default(float);

	[Type(6, "float32")]
	public float currentLevelMinTime = default(float);

	[Type(7, "float32")]
	public float currentLevelMaxScore = default(float);

	[Type(8, "float32")]
	public float currentLevelMaxTime = default(float);

	[Type(9, "uint32")]
	public uint currentLevelmoves = default(uint);

	[Type(10, "uint32")]
	public uint currentLevelStar1 = default(uint);

	[Type(11, "uint32")]
	public uint currentLevelStar2 = default(uint);

	[Type(12, "uint32")]
	public uint currentLevelStar3 = default(uint);

	[Type(13, "uint32")]
	public uint currentLevelStartTime = default(uint);
}

