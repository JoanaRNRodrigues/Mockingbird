using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockingBird.Models;
using MockingBird.Services;
using System.Diagnostics;

namespace MockingBird.Controllers
{
    public class HomeController : Controller
    {
        //Dependency Injection
        private readonly IConfiguration config;
        private readonly IJWTService tokenService;
        private readonly IChirpService userService;
        private readonly IWebHostEnvironment hostEnvironment;



        public HomeController(IConfiguration config, IJWTService tokenService, IChirpService userService, IWebHostEnvironment hostEnvironment)
        {
            this.config = config;
            this.tokenService = tokenService;
            this.userService = userService;
            this.hostEnvironment = hostEnvironment;
        }

        [AllowAnonymous]
        //returns the index view
        public IActionResult Index()
        {
            return View();
        }



        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(User userModel)
        {
            //verifica se existe um username e uma password, caso sejam nulas redireciona para a página de erro
            if (string.IsNullOrEmpty(userModel.UserName) || string.IsNullOrEmpty(userModel.Password))
            {
                return (RedirectToAction("Error"));
            }
            //passa o username e a password ao serviço getUser, para ir buscar o utilizador com essa informação e atribui-lo à variável user
            var user = userService.GetUser(userModel.UserName, userModel.Password);
            if (user == null)
            {
                ModelState.AddModelError("Failure", "Wrong Username and password combination !");
                return RedirectToAction("Index");
            }

            //instancia de UsersViewModel com os mesmos dados que o utilizador com o username e password pretendidos
            var validUser = new UsersViewModel { UserName = user.UserName, ID = user.ID, Name = user.Name, Email = user.Email };

            //Se o validUser não for nulo, um token vai ser gerado 
            if (validUser != null)
            {
                string generatedToken = tokenService.GenerateToken(
                    config["Jwt:Key"].ToString(),
                    config["Jwt:Issuer"].ToString(),
                    config["Jwt:Audience"].ToString(),
                validUser);

                //se o token for gerado corretamente, uma sessão é criada com esse token, o login é feito e a página é redirecionada para a userpage do usuário pretendido
                if (generatedToken != null)
                {
                    HttpContext.Session.SetString("Token", generatedToken);
                    return RedirectToAction("Feed", validUser);
                }

                //Caso alguma das condições anteriores não se verifique, a página de erro é devolvida
                else
                {
                    ModelState.AddModelError("Failure", "Wrong Username and password combination !");
                    return View();
                }
            }
            else
            {
                return View();
            }

        }


        //Feed com todos os chirps, protegido para apenas poderem aceder as pessoas que fizeram login
        [Authorize]
        [Route("feed")]
        [HttpGet]
        public IActionResult Feed()
        {
            //vai buscar o token a sessão
            string token = HttpContext.Session.GetString("Token");


            //caso o token não exista, o utilizador é redirecionado para a página do login
            if (token == null)
            {
                return (RedirectToAction("Index"));
            }
            //se a sessão tiver um token, é verificado se este é válido e caso não seja válido, o utilizador é redirecionado para a página do login
            if (!tokenService.IsTokenValid(
                config["Jwt:Key"].ToString(),
                config["Jwt:Issuer"].ToString(),
                config["Jwt:Audience"].ToString(),
                token))
            {
                return (RedirectToAction("Index"));
            }

            //se tudo estiver correto, a view do Feed é retornada e o user é passado para a view

            var chirps = userService.GetAll();
            ViewBag.Token = token;
            return View(new ChirpsViewModel { Chirps = chirps });
        }

        public IActionResult ChirpsByUser()
        {
            //vai buscar o token a sessão
            string token = HttpContext.Session.GetString("Token");
            int ID = int.Parse(tokenService.GetJWTTokenClaim(token));
            //com o id é possível obter o utilizador a partir do serviço GetUserByID
            User u = userService.GetUserByID(ID);

            //caso o token não exista, o utilizador é redirecionado para a página do login
            if (token == null)
            {
                return (RedirectToAction("Index"));
            }
            //se a sessão tiver um token, é verificado se este é válido e caso não seja válido, o utilizador é redirecionado para a página do login
            if (!tokenService.IsTokenValid(
                config["Jwt:Key"].ToString(),
                config["Jwt:Issuer"].ToString(),
                config["Jwt:Audience"].ToString(),
                token))
            {
                return (RedirectToAction("Index"));
            }

            //se tudo estiver correto, a view do Feed é retornada e o user é passado para a view

            var chirps = userService.ChirpsByUser(u);
            ViewBag.Token = token;
            return View(new ChirpsViewModel { Chirps = chirps });
        }

        public IActionResult UserPage()
        {
            //o token e o id do utilizador são obtidos atraves do token da sessão
            string token = HttpContext.Session.GetString("Token");
            int ID = int.Parse(tokenService.GetJWTTokenClaim(token));
            //com o id é possível obter o utilizador a partir do serviço GetUserByID
            User u = userService.GetUserByID(ID);
            
            //retorna a view, e cria uma instancia do tipo usersViewModel com oID, email, nome e username do utilizador encontrado pelo id no passo anterior
            return View(new UsersViewModel { ID = u.ID, Email = u.Email, Name = u.Name, UserName = u.UserName, ImagePath = u.ImagePath });

            return View();
        }



        //página do utilizador
       // [Authorize]
        //[Route("{user}")]
        [HttpPost]
        public IActionResult ProfileImageUpload(IFormFile file)
        {

            string token = HttpContext.Session.GetString("Token");
            int ID = int.Parse(tokenService.GetJWTTokenClaim(token));
            //com o id é possível obter o utilizador a partir do serviço GetUserByID
            User u = userService.GetUserByID(ID);

            string path = Path.Combine(this.hostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Path.GetFileName(file.FileName);


            userService.SaveImage(u.ID, fileName);

            using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                file.CopyTo(stream);
                return RedirectToAction("UserPage");
            }

            return RedirectToAction("Error");

        }
        [Authorize]
        public IActionResult ImageUpload(int id)
        {

            Chirp chirp = userService.GetChirpById(id);
            return View(new ChirpViewModel { Id = chirp.Id, Text = chirp.Text });
        }

        
        [Authorize]
        [HttpPost]
        public IActionResult ImageUpload(IFormFile file, ChirpViewModel chirp)
        {

            string token = HttpContext.Session.GetString("Token");

            string path = Path.Combine(this.hostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Path.GetFileName(file.FileName);


            userService.ChirpSaveImage(chirp.Id, fileName);

            using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                file.CopyTo(stream);
                
            }

            return RedirectToAction("Error");

        }

        /*[Authorize]
        [HttpPost]
        public IActionResult ImageUpload(IFormFile file)
        {
            string token = HttpContext.Session.GetString("Token");
            
            string path = Path.Combine(this.hostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Path.GetFileName(file.FileName);
            using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                file.CopyTo(stream);
                return RedirectToAction("UserPage", new User { ImagePath = file.FileName });
            }
            return RedirectToAction("Error");
        }
        */

        public IActionResult ChirpDetails(int id)
        {
            Chirp chirp = userService.GetChirpById(id);
            return View(new ChirpViewModel { Id = chirp.Id, Text = chirp.Text });
        }
        [Authorize]
        [HttpGet]
        public IActionResult ChirpDetail(ChirpViewModel chirp)
        {
            //o token é recuperado da sessão
            string token = HttpContext.Session.GetString("Token");

            //caso o token não exista, o utilizador é redirecionado para a página do login
            if (token == null)
            {
                return (RedirectToAction("Index"));
            }
            //se a sessão tiver um token, é verificado se este é válido e caso não seja válido, o utilizador é redirecionado para a página do login
            if (!tokenService.IsTokenValid(
                config["Jwt:Key"].ToString(),
                config["Jwt:Issuer"].ToString(),
                config["Jwt:Audience"].ToString(),
                token))
            {
                return (RedirectToAction("Index"));
            }

            //se tudo estiver correto, a view do userPage é retornada e o user é passado para a view, para que seja possível mostrar a sua informação no seu perfil
            ViewBag.Token = token;
            return View();
        }

        //returns the privacy view
        public IActionResult Privacy()
        {
            return View();
        }


        //returns the error view
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //Create chirp
        [Authorize]
        public IActionResult Create()
        {
            string token = HttpContext.Session.GetString("Token");
            ViewBag.Token = token;
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Chirp chirp)
        {
            // token e o id do utilizador são obtidos atraves do token da sessão
            string token = HttpContext.Session.GetString("Token");
            int ID = int.Parse(tokenService.GetJWTTokenClaim(token));
            //com o id é possível obter o utilizador a partir do serviço GetUserByID
            User u = userService.GetUserByID(ID);
            //o chirp que é recebido após o preenchimento do formulário da view create é completado com o user obtido pelo id
            chirp.User = u;
            //o novo chirp é criado utilizando o serviço Create
            var newChirp = userService.Create(chirp);
            ViewBag.Token = token;
            if (newChirp is not null)
            {
                return RedirectToAction(nameof(Feed));
            }
            else
            {
                return RedirectToAction(nameof(Error));
            }
        }

        //Sign Up
        public IActionResult SignUp()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                var userExists = userService.FindByUserName(user.UserName);
                var emailExists = userService.FindByEmail(user.Email);
                if (userExists != null && emailExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "User already exists!");

                var newUser = userService.CreateUser(user);
                if (newUser is not null)
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction(nameof(Error));
            }
            else
            {
                return View("SignUp", user);
            }
        }


        //Update Chirp
        [Authorize]
        public IActionResult UpdateChirp(int id)
        {

            Chirp chirp = userService.GetChirpById(id);
            return View(new ChirpViewModel { Id = chirp.Id, Text = chirp.Text });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateChirp(ChirpViewModel chirp)
        {
            // token e o id do utilizador são obtidos atraves do token da sessão
            string token = HttpContext.Session.GetString("Token");
            ViewBag.Token = token;
            if (ModelState.IsValid)
            {
                userService.UpdateChirp(chirp.Id, chirp);
                return RedirectToAction(nameof(Feed));
            }
            else
            {
                return RedirectToAction(nameof(Error));
            }
        }


        //Delete Chirp
        [Authorize]
        public IActionResult DeleteChirp(int id)
        {

            Chirp chirp = userService.GetChirpById(id);
            return View(new ChirpViewModel { Id = chirp.Id, Text = chirp.Text });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteChirp(ChirpViewModel chirp)
        {
            // token e o id do utilizador são obtidos atraves do token da sessão
            string token = HttpContext.Session.GetString("Token");
            ViewBag.Token = token;
            userService.DeleteChirpById(chirp.Id);
            return RedirectToAction(nameof(Feed));
            
        }


        //Like Chirp
        public IActionResult LikesChirp()
        {
            return View();
        }

        [Authorize]
        [Route("feed")]
        [HttpPost]
        public async Task<IActionResult> LikesChirp(Chirp chirp)
        {
            // token e o id do utilizador são obtidos atraves do token da sessão
            string token = HttpContext.Session.GetString("Token");
            int id = int.Parse(tokenService.GetJWTTokenClaim(token));

            if (ModelState.IsValid)
            {
                var newChirp = userService.LikeChirp(id, chirp);
                int likes = newChirp.LikeList.Count();
                chirp.Likes = likes;
                return RedirectToAction(nameof(Feed));
            }
            else
            {
                return RedirectToAction(nameof(Error));
            }
        }


        //Delete User
        [Authorize]
        public IActionResult DeleteUser()
        {
            string token = HttpContext.Session.GetString("Token");
            int id = int.Parse(tokenService.GetJWTTokenClaim(token));
            User user = userService.GetUserByID(id);
            return View(new UsersViewModel { UserName = user.UserName, ID = user.ID, Name = user.Name, Email = user.Email });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(UsersViewModel user)
        {
            string token = HttpContext.Session.GetString("Token");
            int ID = int.Parse(tokenService.GetJWTTokenClaim(token));
            ViewBag.Token = token;
            userService.DeleteUserById(ID);
            return RedirectToAction(nameof(Index));
       
        }


        //Logout
        [Authorize]
        public IActionResult Logout()
        {
            string token = HttpContext.Session.GetString("Token");
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LogoutUser()
        {
            HttpContext.Session.Remove("Token");
            return (RedirectToAction("Index"));
        }

        //Upload Images
        
        public IActionResult ImageUpload(UsersViewModel imagePath)
        {
            return View(imagePath);
        }

       
    }
}
