using Microsoft.EntityFrameworkCore;
using MockingBird.Models;

namespace MockingBird.Services
{
    public class ChirpService : IChirpService
    {
        private readonly MockingBirdContext context;    //Context


        DateTime time = DateTime.Now;


        //Context
        public ChirpService(MockingBirdContext context)
        {
            this.context = context;
        }


        //endpoint to create a new chirp, it receives a chirp and if the chirp's user exists, then the chirp will be added to the context.
        public Chirp Create(Chirp newChirp)
        {
            User user = context.Users.Find(newChirp.User.ID);

            if (user is null)
            {
                throw new NullReferenceException("User does not exist");
            }
            else
            {
                newChirp.Date = time;
                newChirp.User = user;
                
                context.Chirps.Add(newChirp);
                context.SaveChanges();
                return newChirp;
            }
        }


        //endpoint to get all chirps from the context
        public IEnumerable<Chirp> GetAll()
        {
            var chirp = context.Chirps
           .Include(p => p.User);
            return chirp;
        }

        //endpoint to get chirps by User from the context
        public IEnumerable<Chirp> ChirpsByUser(User user)
        {
            var chirps = context.Chirps
           .Include(c => c.User).Where(c => c.User.ID == user.ID);

            return chirps;
        }


        //endpoint para Get Chirp By Id
        public Chirp GetChirpById(int id)
        {
            return context.Chirps.Find(id);
        }


        //enpoint to delete chirp
        public void DeleteChirpById(int id)
        {
            var chirpToDelete = context.Chirps.Find(id);
            if (chirpToDelete is null)
            {
                throw new NullReferenceException("Chirp does not exist");
            }
            else
            {
                context.Chirps.Remove(chirpToDelete);
                context.SaveChanges();
            }
        }


        //enpoint to update chirp
        public void UpdateChirp(int id, ChirpViewModel chirp)
        {
            var chirpToUpdate = context.Chirps.Find(id);
            if (chirpToUpdate is null)
            {
                throw new NullReferenceException("Chirp does not exist");
            }
            else
            {
                chirpToUpdate.Text = chirp.Text;
                chirpToUpdate.Date = time;
                context.SaveChanges();
            }
        }


        //endpoint to Like a Chirp       
        public Chirp LikeChirp(int id, Chirp chirp)
        {
            var userWhoLiked = context.Users.Find(id);       //verifica se o Id do User existe 
            var userIsInList = false;

            for (int i = 0; i < chirp.LikeList.Count; i++)      //itera os likes 
            {
                if (userWhoLiked.ID == chirp.LikeList[i].ID)       //verifica se o id do User já consta na lista
                {
                    userIsInList = true;
                }
            }

            if (userWhoLiked != null && userIsInList == false)       //se o utilizador existir e não tiver na lista adiciona o id na lista de likes
            {
                chirp.LikeList.Add(userWhoLiked);
                context.SaveChanges();
                return chirp;
            }
            else
            {
                throw new NullReferenceException("You have already liked this Chirp");
            }
        }


        //endpoint para Get User
        public User GetUser(string userName, string password)
        {
            User user = context.Users
            .SingleOrDefault(u => u.UserName == userName && u.Password == password);
            return user;
        }


        //endpoint para Get User By Id
        public User GetUserByID(int id)
        {
            return context.Users.FirstOrDefault(x => x.ID == id);
        }


        //enpoint to create user
        public User CreateUser(User newUser)
        {
            if (newUser is null)
            {
                throw new NullReferenceException("User does not exist");
            }
            else
            {
                newUser.ImagePath = "Default_Image.jpg";
                context.Users.Add(newUser);
                context.SaveChanges();
                return newUser;
            }
        }
        //Find by UserName para Create User
        public User? FindByUserName(string userName)
        {
            return context.Users.FirstOrDefault(x => x.UserName == userName);
        }
        //Find by Email para Create User
        public User? FindByEmail(string email)
        {
            return context.Users.FirstOrDefault(x => x.Email == email);
        }


        //enpoint to delete User
        public void DeleteUserById(int id)
        {
            var userToDelete = context.Users.Find(id);
            if (userToDelete is not null)
            {
                context.Users.Remove(userToDelete);
                context.SaveChanges();
            }
        }

        public void SaveImage(int userId, string path) {

            var userToUpdateImage = context.Users.Find(userId);
            if (userToUpdateImage is not null) {
                userToUpdateImage.ImagePath = path;
                context.SaveChanges();
            }
        }
        public void ChirpSaveImage(int chirpId, string path)
        {

            var chirpToUpdateImage = context.Chirps.Find(chirpId);
            if (chirpToUpdateImage is not null)
            {
                chirpToUpdateImage.ImagePath = path;
                context.SaveChanges();
            }
        }
    }
}