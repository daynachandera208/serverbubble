import { Room, Client, generateId } from "colyseus";
import { Schema, MapSchema, ArraySchema, Context } from "@colyseus/schema";
import { verifyToken, User, IUser } from "@colyseus/social";
import { Console, debug } from "console";

// Create a context for this room's state data.
const type = Context.create();

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
  @type(Player) player1;
  @type(Player) player2;
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
  currentplayer=-1;
  onCreate (options: any) {
    console.log("GameRoom created.", options);

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
          console.log("Total scors="+this.state.player1.TotalScore+", Total Stars="+this.state.player1.TotalStars+"-/-/-/-/-/-/-/-/-/-/-/-/-/-//--/-111111");
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
          console.log("Current scors="+this.state.player2.currentLevelScore+", Total scores="+this.state.player2.TotalScore+"-/-/-/-/-/-/-/-/-/-/-/-/-/-//--/-2222222");
          console.log("current star="+this.state.player2.TotalStars+", Total Stars="+this.state.player2.currentStars+"-/-/-/-/-/-/-/-/-/-/-/-/-/-//--/-2222222");
          this.state.player2.currentLevelScore=0;
          this.state.player2.currentStars=0;
          this.state.player2.currentLevel+=1;
          this.state.player2.FinishedLevels+=1;
          client.send("LoadLevel",{lvl:this.state.player2.currentLevel});  
          console.log("Current scors="+this.state.player2.currentLevelScore+", Total scores="+this.state.player2.TotalScore+"-/-/-/-/-/-/-/-/-/-/-/-/-/-//--/-2222222");
          console.log("Total star="+this.state.player2.TotalStars+", Current Stars="+this.state.player2.currentStars+"-/-/-/-/-/-/-/-/-/-/-/-/-/-//--/-2222222");
          
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
    console.log("onAuth(), options!", options);
    return await User.findById(verifyToken(options.token)._id);
  }


  onJoin (client: Client, options: any, user: IUser) {
    console.log("client joined!", client.sessionId);
    
    if(this.state.player1==null && this.playercounter==0)
    {
      console.log("player 1");
      let player =new Player();
      player.sessionId=client.sessionId;
      player.currentLevelScore=0;
      player.TotalScore=0;
      player.FinishedLevels=0;
      player.currentLevel=1;
      player.totalGameOvers=0;
      player.TotalStars=0;
      player.currentStars=0;
      this.state.player1=player;
      this.state.roomStatus='waiting';
      this.playercounter++;
      this.clock.start();
      
      this.clock.setTimeout(()=>{
        let player =new Player();
      player.sessionId="BOT";
      player.currentLevelScore=0;
      player.TotalScore=0;
      player.FinishedLevels=0;
      player.currentLevel=1;
      player.totalGameOvers=0;
      player.TotalStars=0;
      player.currentStars=0;
      this.state.player2=player;
      this.state.roomStatus='waiting';
      this.playercounter++;
      this.startGame();
      },15000);
      
    }
    else if(this.state.player2==null && this.playercounter==1)
    {
      console.log("player 2");
      let player =new Player();
      player.sessionId=client.sessionId;
      player.currentLevelScore=0;
      player.TotalScore=0;
      player.FinishedLevels=0;
      player.currentLevel=1;
      player.totalGameOvers=0;
      player.TotalStars=0;
      player.currentStars=0;
      this.state.player2=player;
      this.state.roomStatus='waiting';
      this.playercounter++;
      this.clock.clear();
      this.clock.stop();
    }


  this.startGame();
    
    client.send("type", { hello: true });
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
      this.clock.setTimeout(()=>{
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
          
      },600000);
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
