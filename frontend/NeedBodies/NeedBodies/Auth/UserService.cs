namespace NeedBodies.Auth
{
    public class UserService
    {
        private List<User> _users;

        public UserService() { }

        public UserService(List<User> users)
        {
            _users = users;
        }

        public User? GetByUsername(string username)
        {
            return _users.FirstOrDefault(x => x.Username == username);
        }

        public User? GetByID(string ID)
        {
            return _users.FirstOrDefault(x => x.ID.ToString() == ID);
        }

        public User? GetByEmail(string email)
        {
            return _users.FirstOrDefault(x => x.Email == email);
        }

        public void addUser(User newUser)
        {
            _users.Add(newUser);
        }

        public async Task InitAsync()
        {
            var users = await Api.Auth.GetUsers();
            _users = users;
        }

    }


}