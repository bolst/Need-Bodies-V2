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
        host_id = request.args.get('hid')
        user_id = request.args.get('uid')
        retval = json.load(f)['games']
        
        if game_id != None and len(game_id) != 0 and game_id.isnumeric():
            game_id = int(game_id)
            print('game id', game_id)
            return gameById(retval, game_id)
        
        if host_id != None and len(host_id) != 0 and host_id.isnumeric():
            host_id = int(host_id)
            print('host id', host_id)
            with open(join(dir, 'users.json'), 'r') as f:
                users = json.load(f)['users']
                return gameByHostId(retval, users, host_id)
            
        if user_id != None and len(user_id) != 0 and user_id.isnumeric():
            user_id = int(user_id)
            print('user id', user_id)
            with open(join(dir, 'users.json'), 'r') as f:
                users = json.load(f)['users']
                return gameByUserId(retval, users, user_id)

        
        return retval
    
@app.route('/users')
def users():
    game_id = request.args.get('gid')
    with open(join(dir, 'users.json'), 'r') as f:
        retval = json.load(f)['users']
        retval = [{k: v for k, v in d.items() if k != 'password'} for d in retval]
        
        if game_id != None and len(game_id) != 0 and game_id.isnumeric():
            retval = [user for user in retval if int(game_id) in user['games']]
        
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

@app.route('/joingame', methods=['POST'])
def joinGame():
    user_id = request.json['user id']
    game_id = request.json['game id']
    print(user_id, 'joining', game_id)
    
    with open(join(dir, 'games.json'), 'r') as f:
        game_data = json.load(f)
        
    for i,game in enumerate(game_data['games']):
        if game['id'] == game_id:
            theGame = game
            break
    
    with open(join(dir, 'users.json'), 'r') as f:
        user_data = json.load(f)
        
    for i,user in enumerate(user_data['users']):
        if user['id'] == user_id:
            if game_id in user['games']:
                return 'user already in game'
            user_data['users'][i]['games'].append(theGame['id'])
            break
        
    with open(join(dir, 'users.json'), 'w') as f:
        json.dump(user_data, f)
            
    return 'success'

@app.route('/deletegame', methods=['POST'])
def deleteGame():
    user_id = request.json['user id']
    game_id = request.json['game id']
    
    with open('users.json', 'r') as f:
        user_data = json.load(f)
        
    user_data = removeUserFromGame(user_data, user_id, game_id)
    
    with open(join(dir,'users.json'), 'w') as f:
        json.dump(user_data, f)
    
    with open('games.json', 'r') as f:
        game_data = json.load(f)
    
    game_data['games'] = [game for game in game_data['games'] if game['id'] != game_id]   
    
    with open('games.json', 'w') as f:
        json.dump(game_data, f) 
    
    return 'success'

@app.route('/removeuser', methods=['POST'])
def removeUserFromGame():
    user_id = request.json['user id']
    game_id = request.json['game id']
    
    with open('users.json', 'r') as f:
        user_data = json.load(f)
        
    user_data = removeUserFromGame(user_data, user_id, game_id)
    
    with open(join(dir,'users.json'), 'w') as f:
        json.dump(user_data, f)
    
    return 'success'

def removeUserFromGame(user_data, user_id, game_id):
    for i,user in enumerate(user_data['users']):
        if user['id'] == user_id:
            user_data['users'][i]['games'] = [game for game in user['games'] if game != game_id]
    return user_data
            

def mapGameToHost(gameID, user_id):        
    with open(join(dir, 'users.json'), 'r') as f:
        user_data = json.load(f)
        
    for i,user in enumerate(user_data['users']):
        if user['id'] == user_id:
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

def gameByHostId(games: list, users:list, host_id: int):
    game_ids = []
    for user in users:
        if user['id'] == host_id:
            game_ids = user['hosted games']
    return [game for game in games if game['id'] in game_ids]

def gameByUserId(games: list, users:list, user_id: int):
    game_ids = []
    for user in users:
        if user['id'] == user_id:
            game_ids = user['games']
    return [game for game in games if game['id'] in game_ids]
        

def generateID(data: list) -> int:
    max_id = 0
    for dat in data:
        intID = int(dat['id'])
        if intID > max_id:
            max_id = intID
    return max_id + 1