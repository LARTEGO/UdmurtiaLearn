namespace AccountModel
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public int Level { get; set; } = 1;
        public int Streack { get; set; } = 0;
        // public string password { get; set; } = ""

        public bool CanChangeName(string newName, out string changeError)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                changeError = "! Пустое поле";
                return false;
            }
            if (newName == Username)
            {
                changeError = "! Имя повторяется";
                return false;
            }
            changeError = null;
            return true;
        }

        public bool LvlSet (int LevelThatSets)
        {
            if (LevelThatSets >= 1)
            {
                Level = LevelThatSets;
                return true;
            }
            else return false;
        }
        
        public void ChangeName(string newName)
        {
            Username = newName;
        }
    }
}
