// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.10
// 

using Colyseus.Schema;

public partial class State : Schema {
	[Type(0, "ref", typeof(Player))]
	public Player player1 = new Player();

	[Type(1, "ref", typeof(Player))]
	public Player player2 = new Player();

	[Type(2, "int8")]
	public int winnerPlayerIndex = default(int);

	[Type(3, "string")]
	public string roomStatus = default(string);
}

