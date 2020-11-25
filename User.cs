namespace wssserver
{
    public class User : IUser
    {
        public string name {set;get;}

        public User(string name)
        {
            this.name = name;
        }

        public string getUsername()
        {
            return name;
        }
    }

}