# Travel telegram bot
---
### *The bot was created to help tourists travel*
>The bot works in tandem with the API for travel services. Click to go to the API project on [github](https://github.com/EgorP51/travel_bot_api).

## Used in project
- Telegram API 
- Console application on .NET Core 5
- Project deployment on Hiroku

## Features
- Studying information about the city
  - Photo
  - View on the map
  - Brief information from wikipedia
- Search for hotels by parameters (check-in date, check-out date, number of people)
  - It is possible to see photos
  - See the hotel on the map
  - Go to the site for booking a room
  - Find useful places near the hotel
- View the weather
  - Minimum and maximum temperature 
  - Short description
- Saving and deleting routes as desired
  - The bot has the ability to save routes, you can delete one route or all at once
> As routes, I use links in the format: https://www.google.com/maps/dir/?api=1&origin={latitude},{longitude}&destination={Lat},{Long}&travelmode=walking to get a ready route in google maps
>> *{latitude}* and *{longitude}* these are the starting coordinates<br/>
>> *{Lat}* and *{Long}* are the coordinates of the end point




- - -
# Examples of work with bot

#### *Start window*
<img src="https://github.com/EgorP51/travel_bot/blob/master/Screenshots/0.jpg" width=250 height=334>

#### Getting started with the bot: Initial command /start
<img src="https://user-images.githubusercontent.com/93947345/181045760-68dfa72b-3608-46ea-a0c0-b73a925883cc.png" width=450 height=350>

#### View all commands via the /help command
<img src="https://user-images.githubusercontent.com/93947345/181045762-2ae9183e-5530-4038-938f-79338c0aed8d.png" width=440 height=170>

#### Start of work with the city
<img src="https://user-images.githubusercontent.com/93947345/181045765-d1027ccf-2796-438e-8d10-30a2919e3763.png" width=500 height=505>

<img src="https://user-images.githubusercontent.com/93947345/181045785-c5490173-4c3d-4746-928d-f56c9589e71d.png" width=500 height=280>
