# Bike-Demonstrator
In this study project I combined multiple hardware components using ANT+ and created a immersive and responsive virtual scenario in UNITY.

## What did I do?
I connected indoor cycling trainer (https://www.garmin.com/en-US/c/sports-fitness/indoor-trainers/), air fan (https://eu.wahoofitness.com/devices/indoor-cycling/accessories/kickr-headwind-buy-eu) and few other components. The data between all the components are transmitted using ANT+ protocol, which is similar technology to Bluetooth. I also used some free assets from Unity Store to create a basic virtual scenario - track for the bike with hills and curves.

## How it works?
You simply ride a bike on the TacX cycling trainer and the bike measures your power output and few other variables. Those data are sent to the PC using ANT+ system. There speed and trainer resistance is calculated based on your power output and slope in the scenario (if you ride uphill or downhill).

The virtual avatar on the screen moves based on the calculated speed (and based on the position of the avatar the resistance is calculated).
The resistance value is sent back to the trainer, so you feel if you ride uphill (pedaling gets harder) or downhill (you dont have to pedal and bike speeds up).
The speed value is also sent to the Wahoo air fan - the faster the bike goes, the stronger wind is blowing into your face.

Those features provide high level of haptic feedback and immersivity.


This project was first step on a way to create a fully immersive and responsive virtual reality CAVE scenario. The main goal was to simply connect all the components and make sure they work well together. People from TUC then continued to develop this system further, you can read about the result here: https://www.tu-chemnitz.de/tu/pressestelle/aktuell/10594/en

### Depencencies:
According to the Licence, redistribution of source code containing the ANT+ Network Key is prohibited, therefore:
- get "ANT PC SDK" - download from https://www.thisisant.com/developer/resources/downloads after registration. Place the C# .NET API libraries into /Assets/Scripts
