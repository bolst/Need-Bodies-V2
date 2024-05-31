import os
import json

class Validator():
    def __init__(self, fname:str):
        self.filename = fname

        if not os.path.isfile(fname):
            create_validator_file(fname)
        with open(fname,'r') as f:
            self.users = json.load(f)['users']

    def validate(self, name:str, pswd:str) -> bool:
        return any((user['username'] == name and user['password'] == pswd) for user in self.users)

    def add(self, name:str, pswd:str):
        print(self.users)
        if any(name in user['username'] for user in self.users):
            print('VALIDATOR: user already exists')
            return
        self.users.append({'username': name, 'password': pswd})
        with open(self.filename, 'w') as f:
            json.dump(self.users, f)


def create_validator_file(fname: str):
    with open(fname, 'x') as f:
        f_content = []
        json.dump(f_content, f)

if __name__ == '__main__':
    v = Validator('users.json')
    print(v.validate('nic', 'passwosrd'))