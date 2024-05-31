from flask import Flask, request
import os
from os.path import join
import json
from validate import Validator

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
    new_game = request.json['game']
    user_id = request.json['user id']
    print(user_id)
    with open(join(dir, 'games.json'), 'r') as f:
        game_data = json.load(f)
        new_game['id'] = generateID(game_data['games'])
    game_data['games'].append(new_game)
    print(game_data)
    with open(join(dir, 'games.json'), 'w') as f:
        json.dump(game_data, f)
        
    mapGameToHost(new_game['id'], user_id)
    
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
    
@app.route('/users')
def users():
    with open(join(dir, 'users.json'), 'r') as f:
        retval = json.load(f)['users']
        retval = [{k: v for k, v in d.items() if k != 'password'} for d in retval]
        return retval
    
    
@app.route('/validate', methods=['POST'])
def validate():
    v = Validator(os.path.join(dir, 'users.json'))
    name = request.json['username']
    pswd = request.json['password']
    return 'success' if v.validate(name=name, pswd=pswd) else 'error'
    
@app.route('/adduser', methods=['POST'])
def addUser():
    new_user = request.json
    with open(join(dir, 'users.json'), 'r') as f:
        user_data = json.load(f)
    
    new_user['id'] = generateID(user_data['users'])
    new_user['games'] = []
    new_user['hosted games'] = []
    
    user_data['users'].append(new_user)

    with open(join(dir, 'users.json'), 'w') as f:
        json.dump(user_data, f)
    return {'message': 'success', 'id': new_user['id']}

def mapGameToHost(gameID, userID):        
    with open(join(dir, 'users.json'), 'r') as f:
        user_data = json.load(f)
        
    for i,user in enumerate(user_data['users']):
        if user['id'] == userID:
            user_data['users'][i]['hosted games'].append(gameID)
    
    with open(join(dir, 'users.json'), 'w') as f:
        json.dump(user_data, f)

    return 'success'
    
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

def generateID(data: list) -> int:
    max_id = 0
    for dat in data:
        intID = int(dat['id'])
        if intID > max_id:
            max_id = intID
    return max_id + 1