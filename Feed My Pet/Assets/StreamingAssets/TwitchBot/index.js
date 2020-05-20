const TwitchBot = require('twitch-bot')
const Twitch = require('twitch-api-v5')

Twitch.clientID = "hv1m4yh63dydcpczm79m20rygyadsu"

var join = (channel) => {
  console.log(`Joined channel: ${channel}`)
}
 
var error = (err) => {
  if (err.code == "ECONNRESET") {
    console.log("Disconnected, reconnecting...")
    connect()
  }
}

var use_counter = 0

var message = (chatter) => {
  if(chatter.message.toLowerCase().startsWith("!feed")) {
      let feedAmount = 1

      if (chatter.message.split(" ").length > 1) {
        var num = chatter.message.split(" ")[1]
        num = parseInt(num)
        if (!isNaN(num) && num > 0 && num <= 10) {
          feedAmount = num
          
          Bot.say("Dropping in " + feedAmount + " beans, wait a moment...")
          use_counter++
          if (use_counter > 9) {
            use_counter = 0
            Bot.say("If you like weird experiments like this, you can follow me on Twitter at https://twitter.com/toficofi")
          }
          Twitch.users.userByID({userID: chatter.user_id}, (err, res) => {
            if (err) {
              console.error(err)
              return
            }

            // send profile picture
            console.log("CMD feed " + feedAmount + " " + res.logo)
          })
        } else {
          Bot.say("You can only feed between 1 and 10 beans at a time.")
        }
      } else {
        Bot.say("Use it like this: !feed [number]")
      }
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