# TournamentAdjudicator
The adjudicator for ECE577 upwords tournament
</br>
</br>	Team Roles:
</br>	Game Board, Letter Assignment to user, & Letter-AI Tracking – George
</br>	AI Interface (TCP/IP)-Dan
</br>	Move Analysis (move interpretation, checking validity)- Keith
</br>	Score Keeping, Statistics, and Game Recording - Vidhya
</br>	Tester/Player Example -Bobby


</br>	In order to run the program, we run it using visual studio which launches a web server at http://localhost:62027 
</br>	We use a chrome extension called postman (https://www.getpostman.com/) on chrome to input example data
</br> Note: The team now recommends the Tournament Player Example Code due to its ease of use. Postman will still work.
</br>	The first message sent is a GET request for each of 4 users to http://localhost:62027/api/user
</br>	The user should save their ID and Hash.
</br>	To get the board state or send a move, send a POST request to http://localhost:62027/api/game/IDNumber where IDNumber is the ID number taht was saved earlier. in the header should include the Hash connected to the ID number.  
</br> To update the board state include the updated game board of your new move


</br></br> Troubleshooting:
</br> If you cant get visual studio to launch the program in a browser: Close and reopen visual studio(possibly multiple time).
</br> If you cant get the game going as a single person: Games require at least 2 players and then there is a 10 second window for up to 4 players to join. It will still play with 2 players

</br></br>Regarding Issues:
</br>Please post your issue here. Telling us in person would also work but here we can track it and support more people with the same issue. If you post here we would like to see:
* A brief but helpful description of your issue
* The context in which you are able to reproduce your issue
* A screenshot of any console or debugging information you can find. If we can reproduce your issue, we can't fix it

Side Note: If someone already posted your issue please do not open a new one but please indicate you have the same problem with a +1

</br></br>Final Tournament
</br>For the final, we will be deploying the system to azure at  hhtp://adjudicator.azurewebsites.net/ on Port 80 (DUH!). Make sure to point your final AI there.
