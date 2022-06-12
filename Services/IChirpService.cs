using MockingBird.Models;

namespace MockingBird.Services
{
    public interface IChirpService
    {
        public abstract IEnumerable<Chirp> GetAll();
        public abstract Chirp Create(Chirp newChirp);
        public abstract Chirp GetChirpById(int id);
        public abstract Chirp LikeChirp(int id, Chirp chirp);
        public abstract void DeleteChirpById(int id);
        public abstract void UpdateChirp(int id, ChirpViewModel chirp);
        public abstract User GetUser(string userName, string password);
        public abstract User GetUserByID(int id);
        public abstract User CreateUser(User newUser);
        public abstract User? FindByUserName(string userName);
        public abstract User? FindByEmail(string email);
        public abstract void DeleteUserById(int id);
        public abstract IEnumerable<Chirp> ChirpsByUser(User user);
        public void ChirpSaveImage(int chirpId, string path);
        public void SaveImage(int userId, string path);

    }
}
