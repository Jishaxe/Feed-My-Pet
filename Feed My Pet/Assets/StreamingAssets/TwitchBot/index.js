const TwitchBot = require('twitch-bot')
 
const Bot = new TwitchBot({
  username: 'petcarebot',
  oauth: 'oauth:keunzlmrelfh2sw0b023jdwef3sq34',
  channels: ['jshxe']
})
 
Bot.on('join', channel => {
  console.log(`Joined channel: ${channel}`)
})
 
Bot.on('error', err => {
  console.log(err)
})
 
Bot.on('message', chatter => {
  if(chatter.message === '!feed') {
      console.log("CMD feed");
  }
})

