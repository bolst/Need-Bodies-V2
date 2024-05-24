from flask import Flask, request
import os
from os.path import join
import json

app = Flask(__name__)
dir = os.path.dirname(os.path.realpath(__file__))

@app.route('/arenas')
def arenas():
    with open(join(dir,'arenas.json'), 'r') as f:
        arena_name = request.args.get('name')
        retval = json.load(f)['arenas']
        
        if arena_name != None and len(arena_name) != 0:
            return arenaByName(retval, arena_name)
        
        return retval
    
    
@app.route('/addgame', methods=['POST'])
def addGame():
    new_game = request.json
    with open(join(dir, 'games.json'), 'r') as f:
        game_data = json.load(f)
        new_game['id'] = generateID(game_data['games'])
    game_data['games'].append(new_game)
    print(game_data)
    with open(join(dir, 'games.json'), 'w') as f:
        json.dump(game_data, f)
    return 'success'

@app.route('/games')
def games():
    with open(join(dir, 'games.json'), 'r') as f:
        game_id = request.args.get('id')
        retval = json.load(f)['games']
        
        if game_id != None and len(game_id) != 0 and game_id.isnumeric():
            game_id = int(game_id)
            print('game id', game_id)
            return gameById(retval, game_id)
        
        return retval
    
    
def arenaByName(arenas: list, arena_name: str):
    for arena in arenas:
        if arena['arena'] == arena_name:
            return arena
    return arenas[0]

def gameById(games: list, game_id: int):
    for game in games:
        if game['id'] == game_id:
            return game
    return games[0]

def generateID(games: list) -> int:
    max_id = 0
    for game in games:
        intID = int(game['id'])
        if intID > max_id:
            max_id = intID
    return max_id + 1