import { Room, Client, generateId } from "colyseus";
import { Schema, MapSchema, ArraySchema, Context } from "@colyseus/schema";
import { verifyToken, User, IUser } from "@colyseus/social";
import { Console, debug } from "console";
import { stat } from "fs";

// Create a context for this room's state data.
const type = Context.create();
class Bot extends Schema{
  @type('float32') currentLevelMoveMinScore:number;
  @type('float32') currentLevelMoveMaxScore:number;
  @type('float32') currentLevelMoveMinTime:number;
  @type('float32') currentLevelMoveMaxTime:number;
  @type('float32') currentLevelHardness:number;
  @type('float32') currentLevelMinScore:number;
  @type('float32') currentLevelMinTime:number;
  @type('float32') currentLevelMaxScore:number;
  @type('float32') currentLevelMaxTime:number;
  @type('uint32') currentLevelmoves:number;
  @type('uint32') currentLevelStar1:number;
  @type('uint32') currentLevelStar2:number;
  @type('uint32') currentLevelStar3:number;
  @type('uint32') currentLevelStartTime:number;

}
class Player extends Schema{
  //@type("boolean") connected: boolean = true;
  //@type('int16') playerId:number;
  @type('string') sessionId:String;
  @type('uint16') currentLevel:number;
  @type('uint16') FinishedLevels:number;
  @type('uint32') currentLevelScore:number;
  @type('uint32') TotalScore:number;
  @type('uint32') TotalStars:number;
  @type('uint32') currentStars:number;
  @type('uint8') totalGameOvers:number;
  @type('uint16') secondsLeft:number;  
}

class State extends Schema {
  @type(Player) player1:Player;
  @type(Player) player2:Player;
  @type(Bot) botData:Bot;
  @type('boolean') isBot:boolean;
  @type('int8') winnerPlayerIndex=-1;
  @type('string') roomStatus='waiting';
}
/**
 * Demonstrate sending schema data types as messages
 */
class Message extends Schema {
  @type("number") num;
  @type("string") str;
}

export class GameRoom extends Room {
  maxClients=2;
  playercounter=0;
  isAvailable=false;
  onCreate (options: any) {
    console.log("GameRoom created.", options);
    this.isAvailable=false;
    this.setState(new State());
   
    this.setMetadata({
      str: "hello",
      number: 10
    });

    this.setPatchRate(1000 / 20);
    this.setSimulationInterval((dt) => this.update(dt));

    this.onMessage(0, (client, message) => {
      client.send(0, message);
    });

    this.onMessage("schema", (client) => {
      const message = new Message();
      message.num = Math.floor(Math.random() * 100);
      message.str = "sending to a single client";
      client.send(message);
    })
    this.onMessage("BotCurrentLevelDataData",(client,data)=>{
      
      let obj = new Bot();
      obj.currentLevelMoveMinTime=data[0];
      obj.currentLevelMoveMaxTime=data[1];
      obj.currentLevelMoveMinScore=data[2];
      obj.currentLevelMoveMaxScore=data[3];
      obj.currentLevelMinTime=data[4];
      obj.currentLevelMaxTime=data[5];
      obj.currentLevelMinScore=data[6];
      obj.currentLevelMaxScore=data[7];
      obj.currentLevelHardness=data[8];
      obj.currentLevelmoves=Number( data[9]);
      obj.currentLevelStar1=Number( data[10]);
      obj.currentLevelStar2=Number( data[11]);
      obj.currentLevelStar3=Number( data[12]);
      obj.currentLevelStartTime=this.state.player2.secondsLeft;
      console.log("lvl no-"+this.state.player2.currentLevel+" started at:"+obj.currentLevelStartTime);
this.state.botData=obj;
this.isAvailable=true;
//console.log("--------"+this.state.botData.currentLevelStar1+"---"+this.state.botData.currentLevelStar2);
    });
    this.onMessage("GameControl", (client,cmd) => {
      if(client.sessionId==this.state.player1.sessionId)
      {//player1
        if(cmd=="GameLevelOver")
        {
          this.state.player1.totalGameOvers+=1;
          this.state.player1.currentLevelScore=0;
          this.state.player1.currentStars=0;
          client.send("LoadLevel",{lvl:this.state.player1.currentLevel});   
          
        }
        if(cmd.indexOf("NewScore:-")!=-1)
        {
          this.state.player1.currentLevelScore=Number(cmd.replace("NewScore:-",""));
         // console.log("new score="+this.state.player1.currentLevelScore+" Input is"+Number(cmd.replace("NewScore:-",""))+"------------11111111");
        }
        if(cmd.indexOf("NewStar:-")!=-1)
        {
          this.state.player1.currentStars=Number(cmd.replace("NewStar:-",""));
        }
        if(cmd=="GameLevelWin")
        {
          this.state.player1.TotalScore+=this.state.player1.currentLevelScore;
          this.state.player1.TotalStars+=this.state.player1.currentStars;
          this.state.player1.currentLevelScore=0;
          this.state.player1.currentStars=0;
          this.state.player1.currentLevel+=1;
          this.state.player1.FinishedLevels+=1;
          client.send("LoadLevel",{lvl:this.state.player1.currentLevel});  
         // console.log("Total scors="+this.state.player1.TotalScore+", Total Stars="+this.state.player1.TotalStars+"-/-/-/-/-/-/-/-/-/-/-/-/-/-//--/-111111");
        }
        if(cmd=="GameQuit")
        {
          this.state.player2.TotalScore+=this.state.player2.currentLevelScore;
          this.state.player2.TotalStars+=this.state.player2.currentStars;
          this.state.player2.currentLevelScore=0;
          this.state.player2.currentStars=0;
          this.state.player1.TotalScore+=this.state.player1.currentLevelScore;
          this.state.player1.TotalStars+=this.state.player1.currentStars;
          this.state.player1.currentLevelScore=0;
          this.state.player1.currentStars=0;
          this.state.winnerPlayerIndex=2;
          this.clock.clear();
          this.clock.stop();
          this.state.roomStatus="GameFinished";
          //client.send("GameFinished",{msg:"Quit Won By ",player:2});
        }
      }
      else
      {//player2
        if(cmd=="GameLevelOver")
        {
          this.state.player2.totalGameOvers+=1;
          this.state.player2.currentLevelScore=0;
          this.state.player2.currentStars=0;
          client.send("LoadLevel",{lvl:this.state.player2.currentLevel});   
          
        }
        if(cmd.indexOf("NewScore:-")!=-1)
        {
          this.state.player2.currentLevelScore=Number(cmd.replace("NewScore:-",""));
        //  console.log("new score="+this.state.player1.currentLevelScore+" Input is"+Number(cmd.replace("NewScore:-",""))+"------------");
        }
        if(cmd.indexOf("NewStar:-")!=-1)
        {
          this.state.player2.currentStars=Number(cmd.replace("NewStar:-",""));
        }
        if(cmd=="GameLevelWin")
        {
          this.state.player2.TotalScore+=this.state.player2.currentLevelScore;
          this.state.player2.TotalStars+=this.state.player2.currentStars;
         // console.log("Current scors="+this.state.player2.currentLevelScore+", Total scores="+this.state.player2.TotalScore+"-/-/-/-/-/-/-/-/-/-/-/-/-/-//--/-2222222");
          //console.log("current star="+this.state.player2.TotalStars+", Total Stars="+this.state.player2.currentStars+"-/-/-/-/-/-/-/-/-/-/-/-/-/-//--/-2222222");
          this.state.player2.currentLevelScore=0;
          this.state.player2.currentStars=0;
          this.state.player2.currentLevel+=1;
          this.state.player2.FinishedLevels+=1;
          client.send("LoadLevel",{lvl:this.state.player2.currentLevel});  
         // console.log("Current scors="+this.state.player2.currentLevelScore+", Total scores="+this.state.player2.TotalScore+"-/-/-/-/-/-/-/-/-/-/-/-/-/-//--/-2222222");
          //console.log("Total star="+this.state.player2.TotalStars+", Current Stars="+this.state.player2.currentStars+"-/-/-/-/-/-/-/-/-/-/-/-/-/-//--/-2222222");
          
        }
        if(cmd=="GameQuit")
        {
          this.state.player2.TotalScore+=this.state.player2.currentLevelScore;
          this.state.player2.TotalStars+=this.state.player2.currentStars;
          this.state.player2.currentLevelScore=0;
          this.state.player2.currentStars=0;
          this.state.player1.TotalScore+=this.state.player1.currentLevelScore;
          this.state.player1.TotalStars+=this.state.player1.currentStars;
          this.state.player1.currentLevelScore=0;
          this.state.player1.currentStars=0;
          this.state.winnerPlayerIndex=1;
          this.clock.clear();
          this.clock.stop();
          this.state.roomStatus="GameFinished";
         // client.send("GameFinished",{msg:"Quit Won By ",player:1});
        }
      }
    //  console.log(" Custome Call-/-/-//-/-/-/-/-/-/-/-/-/-/-/-/-"+cmd +"by ==="+client.sessionId);
      
      
      //client.send("powerup", { type: "ammo" });//e.g.
      //this.broadcast("TestBrodcast",{msg:" sent Message for Testing..!"});
    });

    this.onMessage("*", (client, type, message) => {
      console.log(`received message "${type}" from ${client.sessionId}:`, message);
    });
  }

  async onAuth (client, options) {
    /*console.log("onAuth(), options!", options);
    return await User.findById(verifyToken(options.token)._id);*/
    return true;
  }


  onJoin (client: Client, options: any, user: IUser) {
    console.log("client joined!", client.sessionId);
    
    if(this.state.player1==null && this.playercounter==0)
    {
      console.log("player 1");
      let player =new Player();
      player.sessionId=client.sessionId;
      player.currentLevelScore=0;
      this.state.isBot=false;
      player.TotalScore=0;
      player.FinishedLevels=0;
      player.currentLevel=1;
      player.totalGameOvers=0;
      player.TotalStars=0;
      player.currentStars=0;
      this.state.botData=null;
      this.state.player1=player;
      this.state.roomStatus='waiting';
      this.playercounter++;
      this.clock.start();
      
      this.clock.setTimeout(()=>{
        let player =new Player();
      player.sessionId=Math.random().toString(36).substr(2, 9);;//"patiya_bot";
      console.log("playing with patiya bot");
      this.state.isBot=true;
      player.currentLevelScore=0;
      player.TotalScore=0;
      player.FinishedLevels=0;
      player.currentLevel=1;
      player.totalGameOvers=0;
      player.TotalStars=0;
      player.currentStars=0;
      this.state.botData=null;
      this.state.player2=player;
      this.state.roomStatus='waiting';
      this.playercounter++;
      this.startGame();
      },2000);
      
    }
    else if(this.state.player2==null && this.playercounter==1)
    {
      console.log("player 2");
      let player =new Player();
      player.sessionId=client.sessionId;
      player.currentLevelScore=0;
      player.TotalScore=0;
      this.state.isBot=false;
      player.FinishedLevels=0;
      player.currentLevel=1;
      player.totalGameOvers=0;
      player.TotalStars=0;
      player.currentStars=0;
      this.state.botData=null;
      this.state.player2=player;
      this.state.roomStatus='waiting';
      this.playercounter++;
      this.clock.clear();
      this.clock.stop();
    }


  this.startGame();
    
    client.send("type", { hello: true });
  }
  public getBotData()
  {
    this.broadcast("GetBotLevelData",{lvl:this.state.player2.currentLevel});
   
  }
  public  getBotMove()
  {
    if(this.isAvailable){
    this.checkBotLevelState();
    
    if(this.state.botData.currentLevelmoves!=0)
    {
      
      
      this.clock.setTimeout(()=>{
        this.state.botData.currentLevelmoves-=1;
       this.state.player2.currentLevelScore+=this.getBotScore();
       if(this.state.player2.currentLevelScore>=this.state.botData.currentLevelStar3)
       this.state.player2.currentStars=3;
       else if(this.state.player2.currentLevelScore>=this.state.botData.currentLevelStar2)
       this.state.player2.currentStars=2;
       else if(this.state.player2.currentLevelScore>=this.state.botData.currentLevelStar1)
       this.state.player2.currentStars=1;
       this.getBotMove();
       console.log("Bot cs="+this.state.player2.currentLevelScore+"----"+this.state.player2.currentStars+"////-"+this.state.botData.currentLevelmoves);
      },Math.floor(((Math.random() * (this.state.botData.currentLevelMoveMaxTime - this.state.botData.currentLevelMoveMinTime + 1) )+ this.state.botData.currentLevelMoveMinTime)/this.state.botData.currentLevelHardness)*1000);
    }
  }
  else
  {
    this.clock.setTimeout(()=>{this.getBotMove();},500);
  }
  }
  public  getBotScore()
  {
    //Math.floor(Math.random() * (max - min + 1)) + min;
    if(this.isAvailable)
    return Math.floor((((Math.random() * ( this.state.botData.currentLevelMoveMaxScore- this.state.botData.currentLevelMoveMinScore + 1) )+ this.state.botData.currentLevelMoveMinScore )*this.state.botData.currentLevelHardness));
  else 
  return 0;
  }
          public checkBotLevelState()
          {
            console.log("Checking ...ct="+(this.state.botData.currentLevelStartTime-this.state.player2.secondsLeft)+" mint="+this.state.botData.currentLevelMinTime+" maxt ="+this.state.botData.currentLevelMaxTime);
            if(this.state.botData.currentLevelMinTime<=(this.state.botData.currentLevelStartTime-this.state.player2.secondsLeft) && this.state.botData.currentLevelMaxTime>(this.state.botData.currentLevelStartTime-this.state.player2.secondsLeft))
            {
              console.log("Checking ... for valid time");
              if(this.state.botData.currentLevelMinScore<=this.state.player2.currentLevelScore && this.state.botData.currentLevelMaxScore>this.state.player2.currentLevelScore)
              {
                //level clear logic goes here
                console.log("call to win");
                this.BotLevelClear();
                return null;
              }
              else if(this.state.botData.currentLevelMaxScore<=this.state.player2.currentLevelScore){
                //this.state.player2.currentLevelScore-=((this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2);
                this.state.player2.currentLevelScore=Math.floor(Math.random() * (this.state.botData.currentLevelMaxScore - (this.state.botData.currentLevelMinScore+(this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2 )+ 1)) + (this.state.botData.currentLevelMinScore+(this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2);
                //level clear logic goes here
                console.log("Call to more score win");
                this.BotLevelClear();
                return null;
              }
              else
              {
                //do nothing may be useful to define some parameters
                console.log("continue");
              }
            }
            else if(this.state.botData.currentLevelMaxTime<=(this.state.botData.currentLevelStartTime-this.state.player2.secondsLeft))
            {
              console.log("Checking ... for timelimit over");
              if(this.state.botData.currentLevelMinScore<=this.state.player2.currentLevelScore && this.state.botData.currentLevelMaxScore>this.state.player2.currentLevelScore)
              {
                //level clear logic goes here
                console.log("call to win");
                this.BotLevelClear();
                return null;
              }
              else if(this.state.botData.currentLevelMaxScore<=this.state.player2.currentLevelScore){
                //this.state.player2.currentLevelScore-=((this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2);
                this.state.player2.currentLevelScore=Math.floor(Math.random() * (this.state.botData.currentLevelMaxScore - (this.state.botData.currentLevelMinScore+(this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2 )+ 1)) + (this.state.botData.currentLevelMinScore+(this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2);
                //level clear logic goes here
                console.log("Call to more score win");
                this.BotLevelClear();
                return null;
              }
              /*if(this.state.botData.currentLevelMinScore<=this.state.player2.currentLevelScore && this.state.botData.currentLevelMaxScore>this.state.player2.currentLevelScore)
              {
                //level clear logic goes here
                console.log("Calling win");
                this.BotLevelClear();
                return null;
              }
              else if(this.state.botData.currentLevelMaxScore<=this.state.player2.currentLevelScore){
                //this.state.player2.currentLevelScore-=((this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2);
                this.state.player2.currentLevelScore=Math.floor(Math.random() * (this.state.botData.currentLevelMaxScore - (this.state.botData.currentLevelMinScore+(this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2 )+ 1)) + (this.state.botData.currentLevelMinScore+(this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2);
                //level clear logic goes here
                console.log("Calling with reduction win");
                this.BotLevelClear();
                return null;
              }*/
              else
              {
                //level over logic comes here
                console.log("Lost Level");
                this.BotLevelLoose();
                return null;
              }
            }
            if(this.state.botData.currentLevelmoves<=0)
            {
              console.log("Checking ... for Limit amt");
              if(this.state.botData.currentLevelMinTime>(this.state.botData.currentLevelStartTime-this.state.player2.secondsLeft))
              {
                console.log("Checking ... for time in limit");
                if(this.state.botData.currentLevelMinScore<=this.state.player2.currentLevelScore )//&& this.state.botData.currentLevelMaxScore<=this.state.player2.currentLevelScore)
                {
                  // logic goes here for add moves 
                  console.log("add moves");
                  this.BotMovesAdd();
                  return null;
                }
                /*else if(this.state.botData.currentLevelMaxScore<=this.state.player2.currentLevelScore){
                  //this.state.player2.currentLevelScore-=((this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2);
                  this.state.player2.currentLevelScore=Math.floor(Math.random() * (this.state.botData.currentLevelMaxScore - (this.state.botData.currentLevelMinScore+(this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2 )+ 1)) + (this.state.botData.currentLevelMinScore+(this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2);
                  // logic goes here for add moves
                }*/
                else
                {
                  //level over logic comes here
                  console.log("lost by moves");
                  this.BotLevelLoose();
                  return null;
                }
              }
              else
                {
                  //level over logic comes here
                  console.log("lost by moves time out");
                  this.BotLevelLoose();
                  return null;
                }
             /* else if(this.state.botData.currentLevelMinTime<=(this.state.botData.currentLevelStartTime-this.state.player2.secondsLeft) && this.state.botData.currentLevelMaxTime>=(this.state.botData.currentLevelStartTime-this.state.player2.secondsLeft))
              {
                if(this.state.botData.currentLevelMinScore<=this.state.player2.currentLevelScore && this.state.botData.currentLevelMaxScore<=this.state.player2.currentLevelScore)
                {
                  // win logic
                }
                else if(this.state.botData.currentLevelMaxScore<=this.state.player2.currentLevelScore){
                  //this.state.player2.currentLevelScore-=((this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2);
                  this.state.player2.currentLevelScore=Math.floor(Math.random() * (this.state.botData.currentLevelMaxScore - (this.state.botData.currentLevelMinScore+(this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2 )+ 1)) + (this.state.botData.currentLevelMinScore+(this.state.botData.currentLevelMaxScore-this.state.botData.currentLevelMinScore)/2);
                  // win logic
                }
                else
                {
                  //level over logic comes here
                }
              }*/
            }

          }
  public BotLevelClear()
  {
    this.isAvailable=false;
    this.state.botData.currentLevelmoves=0;
    this.clock.setTimeout(()=>{
      console.log("Level Won");
      this.state.player2.TotalScore+=this.state.player2.currentLevelScore;
          this.state.player2.TotalStars+=this.state.player2.currentStars;

          this.state.player2.currentLevelScore=0;
          this.state.player2.currentStars=0;
          this.state.player2.currentLevel+=1;
          this.state.player2.FinishedLevels+=1;
          this.getBotData();
         //this.state.botData.currentLevelStartTime=this.state.player2.secondsLeft;
          this.getBotMove();
    },4000);     
  }
  public BotLevelLoose()
  {
    this.isAvailable=false;
    this.state.botData.currentLevelmoves=0;
    this.clock.setTimeout(()=>{
      console.log("Level Lost");
          this.state.player2.totalGameOvers+=1;
          this.state.player2.currentLevelScore=0;
          this.state.player2.currentStars=0;
          this.getBotData();
         // this.state.botData.currentLevelStartTime=this.state.player2.secondsLeft;
          this.getBotMove();
        },1500);   
  }
  public BotMovesAdd()
  {
    console.log("Level Moves Add");
    this.state.botData.currentLevelmoves+=10;
  }
  public  startGame()
  {
    if(this.playercounter==2&& this.state.player1!=null && this.state.player2!=null)
    {
      this.state.player1.currentLevel=1;
      this.state.player1.secondsLeft=600;
      this.state.player2.currentLevel=1;
      this.state.player2.secondsLeft=600;
      this.state.winnerPlayerIndex=-1;
      this.lock();
      if(this.state.isBot)
      {
        this.getBotData();
       
      }
      this.broadcast("LoadLevel",{lvl:1});   
      this.state.roomStatus='startplaying';
      this.clock.clear();
      this.clock.stop();
      this.clock.start();
      this.clock.setInterval(()=>{
        this.state.player1.secondsLeft-=1;
        this.state.player2.secondsLeft-=1;
        
        //console.log("Timer-----Player 1---------"+this.state.player1.secondsLeft);
        //console.log("Timer-----Player 2---------"+this.state.player2.secondsLeft);

      },1000); 
     // this.state.botData.currentLevelStartTime=this.state.player2.secondsLeft;
      this.getBotMove();
      this.clock.setTimeout(()=>{
        this.checkBotLevelState();
        this.state.player2.TotalScore+=this.state.player2.currentLevelScore;
          this.state.player2.TotalStars+=this.state.player2.currentStars;
          this.state.player2.currentLevelScore=0;
          this.state.player2.currentStars=0;
          this.state.player1.TotalScore+=this.state.player1.currentLevelScore;
          this.state.player1.TotalStars+=this.state.player1.currentStars;
          this.state.player1.currentLevelScore=0;
          this.state.player1.currentStars=0;
          this.state.player1.secondsLeft=0;
          this.state.player2.secondsLeft=0;
          if(this.state.player1.FinishedLevels>this.state.player2.FinishedLevels)
          {
            this.state.winnerPlayerIndex=1;
          }
          else if(this.state.player1.FinishedLevels<this.state.player2.FinishedLevels)
          {
            this.state.winnerPlayerIndex=2;
          }
          else
          {
            if(this.state.player1.TotalScore>this.state.player2.TotalScore){this.state.winnerPlayerIndex=1;}
            else if(this.state.player1.TotalScore<this.state.player2.TotalScore){this.state.winnerPlayerIndex=2;}
            else
            {
              if(this.state.player1.TotalStars>this.state.player2.TotalStars){this.state.winnerPlayerIndex=1;}
              else if(this.state.player1.TotalStars<this.state.player2.TotalStars){this.state.winnerPlayerIndex=2;}
              else
              {
                if(this.state.player1.totalGameOvers<this.state.player2.totalGameOvers){this.state.winnerPlayerIndex=1;}
              else if(this.state.player1.totalGameOvers>this.state.player2.totalGameOvers){this.state.winnerPlayerIndex=2;}
              }
            }
          }
          this.state.roomStatus="GameFinished";
          this.clock.clear();
          this.clock.stop();
          
      }, 600000);
      if(this.state.botData!=null)
      {

      }
      //this.state.roomStatus='playing';
    }
  }


  async onLeave (client: Client, consented: boolean) {
    
    if(client.sessionId==this.state.player1.sessionId)
    {
      this.state.player2.TotalScore+=this.state.player2.currentLevelScore;
      this.state.player2.TotalStars+=this.state.player2.currentStars;
      this.state.player2.currentLevelScore=0;
      this.state.player2.currentStars=0;
      this.state.player1.TotalScore+=this.state.player1.currentLevelScore;
      this.state.player1.TotalStars+=this.state.player1.currentStars;
      this.state.player1.currentLevelScore=0;
      this.state.player1.currentStars=0;
      this.state.winnerPlayerIndex=2;
      this.clock.clear();
      this.clock.stop();
      this.state.roomStatus="GameFinished";
     // client.send("GameFinished",{msg:"Quit Won By ",player:2});
          console.log("Player Left Room having id:"+client.sessionId);
    }
    else
    {
      this.state.player2.TotalScore+=this.state.player2.currentLevelScore;
      this.state.player2.TotalStars+=this.state.player2.currentStars;
      this.state.player2.currentLevelScore=0;
      this.state.player2.currentStars=0;
      this.state.player1.TotalScore+=this.state.player1.currentLevelScore;
      this.state.player1.TotalStars+=this.state.player1.currentStars;
      this.state.player1.currentLevelScore=0;
      this.state.player1.currentStars=0;
      this.state.winnerPlayerIndex=1;
      this.clock.clear();
      this.clock.stop();
      this.state.roomStatus="GameFinished";
      //client.send("GameFinished",{msg:"Quit Won By ",player:1});
      console.log("Player Left Room having id:"+client.sessionId);
    }
  }


  update (dt?: number) {
    // console.log("num clients:", Object.keys(this.clients).length);
  }

  onDispose () {
    console.log("DemoRoom disposed.");
    if(this.state.roomStatus!="GameFinished")
    {
      this.state.player2.TotalScore+=this.state.player2.currentLevelScore;
      this.state.player2.TotalStars+=this.state.player2.currentStars;
      this.state.player2.currentLevelScore=0;
      this.state.player2.currentStars=0;
      this.state.player1.TotalScore+=this.state.player1.currentLevelScore;
      this.state.player1.TotalStars+=this.state.player1.currentStars;
      this.state.player1.currentLevelScore=0;
      this.state.player1.currentStars=0;
      this.state.player1.secondsLeft=0;
      this.state.player2.secondsLeft=0;
      if(this.state.player1.FinishedLevels>this.state.player2.FinishedLevels)
      {
        this.state.winnerPlayerIndex=1;
      }
      else if(this.state.player1.FinishedLevels<this.state.player2.FinishedLevels)
      {
        this.state.winnerPlayerIndex=2;
      }
      else
      {
        if(this.state.player1.TotalScore>this.state.player2.TotalScore){this.state.winnerPlayerIndex=1;}
        else if(this.state.player1.TotalScore<this.state.player2.TotalScore){this.state.winnerPlayerIndex=2;}
        else
        {
          if(this.state.player1.TotalStars>this.state.player2.TotalStars){this.state.winnerPlayerIndex=1;}
          else if(this.state.player1.TotalStars<this.state.player2.TotalStars){this.state.winnerPlayerIndex=2;}
          else
          {
            if(this.state.player1.totalGameOvers<this.state.player2.totalGameOvers){this.state.winnerPlayerIndex=1;}
          else if(this.state.player1.totalGameOvers>this.state.player2.totalGameOvers){this.state.winnerPlayerIndex=2;}
          }
        }
      }
      this.state.roomStatus="GameFinished";
      this.clock.clear();
      this.clock.stop();
    }
  }

}
