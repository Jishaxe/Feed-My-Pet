const config = require("./config.json")
const youtubeLive = require('youtube-chat').LiveChat
var yt = null

var start = () => {
  console.log(`Connected to YouTube Livestream!`)
}

var end = (reason) => {
  console.error("Disconnected from YouTube Livestream! Reason: " + reason)
}

var error = (err) => {
  console.error(err)
  if (err.code == "ECONNRESET" || err.code == "ETIMEDOUT" || err.code == "ENOTFOUND" || err.code == "ENETUNREACH") {
    console.log("Disconnected, reconnecting...")
    connect()
  }
}

var use_counter = 0

var message = (message) => {
  let chatter = {
    message: message.message[0].text,
    author: message.author
  }

  if(chatter.message.toLowerCase().startsWith("!feed")) {
      let feedAmount = 1

      if (chatter.message.split(" ").length > 1) {
        var num = chatter.message.split(" ")[1]
        num = parseInt(num)
        if (!isNaN(num) && num > 0 && num <= 10) {
          feedAmount = num
          
          //Bot.say("Dropping in " + feedAmount + " beans, wait a moment...")
          use_counter++
          if (use_counter > 9) {
            use_counter = 0
            //Bot.say("If you like weird experiments like this, you can follow me on Twitter at https://twitter.com/toficofi")
          }
            
          console.log("CMD feed " + feedAmount + " " + chatter.author.thumbnail.url)

        } else {
         // Bot.say("You can only feed between 1 and 10 beans at a time.")
        }
      } else {
        //Bot.say("Use it like this: !feed [number]")
      }
  }
}

var connect = () => {
  yt = new youtubeLive({channelId: config.channelId})

  yt.on('comment', message)
  yt.on('start', start)
  yt.on('end', end)
  yt.on('error', error)

  yt.start()

}

process.on('unhandledRejection', error)
connect()