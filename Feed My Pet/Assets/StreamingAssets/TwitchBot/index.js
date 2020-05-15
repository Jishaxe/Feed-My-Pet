const TwitchBot = require('twitch-bot')

var join = (channel) => {
  console.log(`Joined channel: ${channel}`)
}
 
var error = (err) => {
  if (err.code == "ECONNRESET") {
    console.log("Disconnected, reconnecting...")
    connect()
  }
}

var message = (chatter) => {
  if(chatter.message.toLowerCase().startsWith("!feed")) {
      if (chatter.message.split(" ").length != 2) {
        console.log("CMD feed 1")
        return
      }

      var num = chatter.message.split(" ")[1]
      num = parseInt(num)
      if (isNaN(num)) return
      if (num < 1 || num > 10) return

      console.log("CMD feed " + num);
  }
}

var Bot = null

var connect = () => {
  Bot = new TwitchBot({
    username: 'petcarebot',
    oauth: 'oauth:keunzlmrelfh2sw0b023jdwef3sq34',
    channels: ['jshxe']
  })  

  Bot.on("message", message)
  Bot.on('error', error)
  Bot.on('join', join)
}

connect()