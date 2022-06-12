using MockingBird.Models;

namespace MockingBird.Data
{
    public static class MockingBirdExtention
    {//if the database doesn't exist, then it will be created and data will be inserted with the MockingbirdDbInitializer
        public static void CreateDbIfNotExists(this IHost host)
        {
            {
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<MockingBirdContext>();
                    // Creates the database if not exists
                    if (context.Database.EnsureCreated())
                    {
                        MockingBirdDbInitializer.InsertData(context);
                    }
                }
            }
        }
    }
}
