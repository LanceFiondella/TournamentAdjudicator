# TournamentAdjudicator
The adjudicator for ECE577 upwords tournament
</br>
</br>	Team Roles:
</br>	Game Board, Letter Assignment to user, & Letter-AI Tracking – George
</br>	AI Interface (TCP/IP)-Dan
</br>	Move Analysis (move interpretation, checking validity)- Keith
</br>	Score Keeping, Statistics, and Game Recording - Vidhya
</br>	Tester/System Architect (has to make a test AI)-Bobby


</br>	In order to run the program, we run it using visual studio which launches a web server at http://localhost:62027 
</br>	We use a chrome extension called postman (https://www.getpostman.com/) on chrome to input example data
</br> Note: The team now recommends the Tournament Player Example Code due to its ease of use. Postman will still work.
</br>	The first message sent is a GET request for each of 4 users to http://localhost:62027/api/user
</br>	The user should save their ID and Hash.
</br>	To get the board state or send a move, send a POST request to http://localhost:62027/api/game/IDNumber where IDNumber is the ID number taht was saved earlier. in the header should include the Hash connected to the ID number.  
</br> To update the board state include the updated game board of your new move


</br></br> Troubleshooting:
</br> If you cant get visual studio to launch the program in a browser: Close and reopen visual studio(possibly multiple time).
</br> If you cant get the game going as a single person: Currently all games must have 4 players and this will be updated in the future.

</br></br>Final Tournament
</br>For the final, we will be deploying the system to azure at  hhtp://adjudicator.azurewebsites.net/ on Port 80 (DUH!). Make sure to point your final AI there.
