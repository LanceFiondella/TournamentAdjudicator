#RestfulClient.py

import requests

import json
from time import sleep

# Replace with the correct URL
url = "http://localhost:62027/api/"

def joinGame():

    myResponse = requests.get(url+"user")


    #print (myResponse.status_code)

    # For successful API call, response code will be 200 (OK)
    if(myResponse.ok):

        # Loading the response data into a dict variable
        # json.loads takes in only binary or string variables so using content to fetch binary content
        # Loads (Load String) takes a Json file and converts into python data structure (dict or list, depending on JSON)
        jData = json.loads(myResponse.content)

        #print("The response contains {0} properties".format(len(jData)))

        for key in jData:
            print str(key) + " : " + str(jData[key])
    else:
      # If response code is not ok (200), print the resulting http error code with description
        myResponse.raise_for_status()
    return jData["Hash"],jData["ID"]

def getGameState(hash,ID):

    myResponse = requests.get(str(url+"game/"+str(ID)) , headers={"Hash":str(hash)})

    #print (myResponse.status_code)

    # For successful API call, response code will be 200 (OK)
    if(myResponse.ok):

        # Loading the response data into a dict variable
        # json.loads takes in only binary or string variables so using content to fetch binary content
        # Loads (Load String) takes a Json file and converts into python data structure (dict or list, depending on JSON)
        jData = json.loads(myResponse.content)

        #print("The response contains {0} properties".format(len(jData)))

        for key in jData:
            print str(key) + " : " + str(jData[key])
    else:
      # If response code is not ok (200), print the resulting http error code with description
        myResponse.raise_for_status()
    return jData["Board"],jData["Letters"],jData["Turn"]

def sendMove(ID, hash, Board, Letters, Turn):

    head = {"Hash": str(hash), "Board":str(Board)}

    myResponse = requests.post(str(url + "game/" + str(ID)), headers=head)

    # print (myResponse.status_code)

    # For successful API call, response code will be 200 (OK)
    if (myResponse.ok):

        # Loading the response data into a dict variable
        # json.loads takes in only binary or string variables so using content to fetch binary content
        # Loads (Load String) takes a Json file and converts into python data structure (dict or list, depending on JSON)
        jData = json.loads(myResponse.content)

        # print("The response contains {0} properties".format(len(jData)))

        #for key in jData:
         #   print str(key) + " : " + str(jData[key])
    else:
        # If response code is not ok (200), print the resulting http error code with description
        myResponse.raise_for_status()
    #return jData["Board"], jData["Letters"], jData["Turn"]

def main():
    hash,ID = joinGame()
    print hash,ID
    Turn=0
    while(Turn!=ID):
        Board,Letters,Turn = getGameState(hash,ID)
        sleep(0.1)

    print(Letters)
    play =[]
    letter = raw_input("Type a letter, an X, and a Y in form \"A 4 5\"")
    while (letter != ""):
        play = letter.split(' ')
        Board[0][int(play[1])][int(play[2])] = play[0]
        Board[1][int(play[1])][int(play[2])] = str(int(Board[1][int(play[1])][int(play[2])]) + 1)
        letter = raw_input("Type a letter, an X, and a Y in form \"A 4 5\"")
    i = 4
    for letter in word:

        i=i+1
    print Board
    raw_input()
    sendMove(ID, hash, Board, Letters, Turn)


main()
