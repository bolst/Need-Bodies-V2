from flask import Flask, request, jsonify
import os
from os.path import join
import json
import numpy as np
import cv2
import base64
from validate import Validator

app = Flask(__name__)
dir = os.path.dirname(os.path.realpath(__file__))

MAX_TEAMS = 3

@app.route('/arenas')
def arenas():
    with open(join(dir,'arenas.json'), 'r') as f:
        arena_name = request.args.get('name')
        retval = json.load(f)['arenas']
        
        if arena_name != None and len(arena_name) != 0:
            return arenaByName(retval, arena_name)
        
        return jsonify(retval)
    
    
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
            retval = gameById(retval, game_id)
        
        if host_id != None and len(host_id) != 0 and host_id.isnumeric():
            host_id = int(host_id)
            print('host id', host_id)
            with open(join(dir, 'users.json'), 'r') as f:
                users = json.load(f)['users']
                retval = gameByHostId(retval, users, host_id)
            
        if user_id != None and len(user_id) != 0 and user_id.isnumeric():
            user_id = int(user_id)
            print('user id', user_id)
            with open(join(dir, 'users.json'), 'r') as f:
                users = json.load(f)['users']
                retval = gameByUserId(retval, users, user_id)

        return jsonify(retval)
    
@app.route('/users')
def users():
    game_id = request.args.get('gid')
    parent_id = request.args.get('pid')
    with open(join(dir, 'users.json'), 'r') as f:
        retval = json.load(f)['users']
        retval = [{k: v for k, v in d.items() if k != 'password'} for d in retval]
        
        if game_id != None and len(game_id) != 0 and game_id.isnumeric():
            retval = [user for user in retval if int(game_id) in user['games']]
            
        if parent_id != None and len(parent_id) != 0 and parent_id.isnumeric():
            for user in retval:
                if int(parent_id) == user['id']:
                    retval = user['non users']
                    break
        
        return jsonify(retval)
    
@app.route('/nonusers/<gameID>')
def nonUsers(gameID):
    retval = []
    if gameID.isnumeric():
        gameID = int(gameID)
    else:
        return 'success'
    
    with open(join(dir, 'users.json'), 'r') as f:
        user_data = json.load(f)
        
    for user in user_data['users']:
        for non_user in user['non users']:
            for game in non_user['games']:
                if game['game id'] == gameID:
                    retval.append(non_user)
                    break
    
    return jsonify(retval)

    
    
@app.route('/validate', methods=['POST'])
def validate():
    v = Validator(os.path.join(dir, 'users.json'))
    name = request.json['username']
    pswd = request.json['password']
    return 'success' if v.validate(name=name, pswd=pswd) else 'error'
    
@app.route('/adduser', methods=['POST'])
def addUser():
    data = request.json
    new_user = {}
    if len(data['parent id']) == 0:
        with open(join(dir, 'users.json'), 'r') as f:
            user_data = json.load(f)
        
        new_user['username'] = data['username']
        new_user['password'] = data['password']
        new_user['email'] = data['email']
        new_user['id'] = generateID(user_data['users'])
        new_user['games'] = []
        new_user['non users'] = []
        new_user['hosted games'] = []
        new_user['teams'] = []
        new_user['position'] = 'forward'
        
        user_data['users'].append(new_user)

        with open(join(dir, 'users.json'), 'w') as f:
            json.dump(user_data, f)
        return {'message': 'success', 'id': new_user['id']}
    else:
        with open(join(dir, 'users.json'), 'r') as f:
            user_data = json.load(f)
        
        new_user['name'] = data['username']
        new_user['games'] = []
        
        for i,user in enumerate(user_data['users']):
            if data['parent id'] == str(user['id']):
                user_data['users'][i]['non users'].append(new_user)

        with open(join(dir, 'users.json'), 'w') as f:
            json.dump(user_data, f)
        return {'message': 'success', 'id': -1}

@app.route('/joingame', methods=['POST'])
def joinGame():
    user_id = request.json['user id']
    game_id = request.json['game id']
    child_name = request.json['child name']
    
    print(user_id, 'joining', game_id)
    
    with open(join(dir, 'games.json'), 'r') as f:
        game_data = json.load(f)
        
    for i,game in enumerate(game_data['games']):
        if game['id'] == game_id:
            theGame = game
            break
    
    with open(join(dir, 'users.json'), 'r') as f:
        user_data = json.load(f)
        
    if len(child_name) == 0:
        for i,user in enumerate(user_data['users']):
            if user['id'] == user_id:
                if game_id in user['games']:
                    return 'user already in game'
                user_data['users'][i]['games'].append(theGame['id'])
                user_data['users'][i]['teams'].append({'game id': theGame['id'], 'team': 0})
                break
    else:
        for i,user in enumerate(user_data['users']):
            if user['id'] == user_id:
                for j,non_user in enumerate(user['non users']):
                    if non_user['name'] == child_name:
                        user_data['users'][i]['non users'][j]['games'].append({'game id': theGame['id'], 'team': 0})
                        break
                break
        
    with open(join(dir, 'users.json'), 'w') as f:
        json.dump(user_data, f)
            
    return 'success'

@app.route('/deletegame', methods=['POST'])
def deleteGame():
    user_id = request.json['user id']
    game_id = request.json['game id']
    
    with open(join(dir, 'users.json'), 'r') as f:
        user_data = json.load(f)
    
    user_data = removeAllGameReferences(user_data, game_id)
    
    with open(join(dir,'users.json'), 'w') as f:
        json.dump(user_data, f)
    
    with open(join(dir, 'games.json'), 'r') as f:
        game_data = json.load(f)
    
    game_data['games'] = [game for game in game_data['games'] if game['id'] != game_id]   
    
    with open(join(dir, 'games.json'), 'w') as f:
        json.dump(game_data, f) 
    
    return 'success'

@app.route('/removeuser', methods=['POST'])
def removeUserFromGame():
    user_id = request.json['user id']
    game_id = request.json['game id']
    child_name = request.json['child name']
    
    with open(join(dir, 'users.json'), 'r') as f:
        user_data = json.load(f)
        
    if len(child_name) == 0:
        user_data = removeUserFrom('games', user_data, user_id, game_id)
    else:
        for i,user in enumerate(user_data['users']):
            if user['id'] == user_id:
                for j,non_user in enumerate(user['non users']):
                    if non_user['name'] == child_name:
                        user_data['users'][i]['non users'][j]['games'] = [x for x in non_user['games'] if x['game id'] != game_id]
                        break
                break  
    
    with open(join(dir,'users.json'), 'w') as f:
        json.dump(user_data, f)
    
    return 'success'

@app.route('/cycleut', methods=['POST'])
def cycleUserTeam():
    game_id = request.json['game id']
    user_id = request.json['user id']
    child_name = request.json['child name']
    
    with open(join(dir, 'users.json'), 'r') as f:
        user_data = json.load(f)
    
    
    if len(child_name) == 0:
        for i,user in enumerate(user_data['users']):
            if user['id'] == user_id:
                for j,team in enumerate(user['teams']):
                    if team['game id'] == game_id:
                        curr_team = team['team']
                        user_data['users'][i]['teams'][j]['team'] = (curr_team + 1) % MAX_TEAMS
                        break
                break
    else: # if user type is non user:
        for i,user in enumerate(user_data['users']):
            if user['id'] == user_id:
                for j,non_user in enumerate(user['non users']):
                    if non_user['name'] == child_name:
                        for k,game in enumerate(non_user['games']):
                            if game['game id'] == game_id:
                                curr_team = game['team']
                                user_data['users'][i]['non users'][j]['games'][k]['team'] = (curr_team + 1) % MAX_TEAMS
                                break
                        break
                break
    
    with open(join(dir, 'users.json'), 'w') as f:
        json.dump(user_data, f)
        
    return 'success'

@app.route('/setposition', methods=['POST'])
def setPosition():
    user_id = request.json['user id']
    position = request.json['position']
    
    with open(join(dir, 'users.json'), 'r') as f:
        user_data = json.load(f)
        
    if position in ['forward', 'defense', 'goalie']:
        for i,user in enumerate(user_data['users']):
            if user['id'] == user_id:
                user_data['users'][i]['position'] = position
                break
            
    with open(join(dir, 'users.json'), 'w') as f:
        json.dump(user_data, f)
    
    return 'success'

@app.route('/setheadshot', methods=['POST'])
def setHeadshot():
    user_id = request.args.get('uid')
    
    try:
        data = request.data
    except Exception as exc:
        print(exc)
        return ''
    
    arr = np.fromstring(data, np.uint8)
    img = cv2.imdecode(arr, cv2.IMREAD_COLOR)
    cv2.imwrite(join(dir, 'headshots', f'{user_id}.png'), img)
    
    return 'success'
    
@app.route('/headshot')
def headshot():
    user_id = request.args.get('id')
    
    if not os.path.isfile(join(dir,'headshots',f'{user_id}.png')):
        print('no image')
        return 'image does not exist'
    
    with open(join(dir,'headshots', f'{user_id}.png'), 'rb') as image:
        f = image.read()
        return base64.b64encode(f).decode('utf-8')
    
@app.route('/deletenonuser', methods=['POST'])
def deleteNonUser():
    user_id = request.json['user id']
    child_name = request.json['child name']
    
    with open(join(dir, 'users.json'), 'r') as f:
        user_data = json.load(f)
        
    for i,user in enumerate(user_data['users']):
        if user['id'] == user_id:
            user_data['users'][i]['non users'] = [nu for nu in user['non users'] if nu['name'] != child_name]
            break
            
    with open(join(dir, 'users.json'), 'w') as f:
        json.dump(user_data, f)
    
    return 'success'
    
                    

def removeUserFrom(key, user_data, user_id, game_id):
    for i,user in enumerate(user_data['users']):
        if user['id'] == user_id:
            user_data['users'][i][key] = [game for game in user[key] if game != game_id]
            if key in ['games', 'hosted games']:
                user_data['users'][i]['teams'] = [team for team in user['teams'] if team['game id'] != game_id]
    return user_data

def removeAllGameReferences(user_data, game_id):
    for i,user in enumerate(user_data['users']):
        user_data['users'][i]['games'] = [gid for gid in user['games'] if gid != game_id]
        user_data['users'][i]['hosted games'] = [gid for gid in user['hosted games'] if gid != game_id]
        for j,non_user in enumerate(user['non users']):
            user_data['users'][i]['non users'][j]['games'] = [game for game in non_user['games'] if game['game id'] != game_id]
        user_data['users'][i]['teams'] = [team for team in user['teams'] if team['game id'] != game_id]
    return user_data
            

def mapGameToHost(gameID, user_id):        
    with open(join(dir, 'users.json'), 'r') as f:
        user_data = json.load(f)
        
    for i,user in enumerate(user_data['users']):
        if user['id'] == user_id and gameID not in user['hosted games']:
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
