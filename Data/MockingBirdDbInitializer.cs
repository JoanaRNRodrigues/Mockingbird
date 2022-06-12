using MockingBird.Models;

namespace MockingBird.Data
{
    public static class MockingBirdDbInitializer
    {//function to insert data into the context
        public static void InsertData(MockingBirdContext context)
        {
            // Adds a user
            var user = new User
            {
                Name = "Mocking Bird",
                UserName = "Mockingbird123",
                Password = "password",
                ConfirmPassword = "password",
                Email = "mockingbird123@gmail.com",
                ImagePath = "Admin_Image.jpg"
            };

            context.Users.Add(user);

            // Adds a chirp
            context.Chirps.Add(new Chirp
            {
                Id = 1,
                Text = "Hello world",
                User = user
            });


            // Saves changes
            context.SaveChanges();
        }
    }
}
