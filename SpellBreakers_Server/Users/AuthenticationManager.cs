using SpellBreakers_Server.DB;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Tcp;
using System.Net.Sockets;

namespace SpellBreakers_Server.Users
{
    public class AuthenticationManager
    {
        private static Lazy<AuthenticationManager> _instance = new Lazy<AuthenticationManager>(() => new AuthenticationManager());
        public static AuthenticationManager Instance = _instance.Value;

        private readonly UserRepository _userRepository;

        public AuthenticationManager()
        {
            _userRepository = new UserRepository("users.db");
        }

        public async Task RegisterAsync(Socket socket, RegisterPacket register)
        {
            RegisterResponsePacket response = new RegisterResponsePacket();

            if (!_userRepository.TryRegister(register.UserID, register.Nickname, register.Password))
            {
                response.Success = false;
                response.Message = "이미 존재하는 아이디 혹은 닉네임입니다!";
            }
            else
            {
                response.Success = true;

                Console.WriteLine($"[서버] 새로운 유저가 회원가입했습니다! : {register.UserID}({register.Nickname})");
            }

            await TcpPacketHelper.SendAsync(socket, response);
        }

        public async Task LoginAsync(Socket socket, LoginPacket login)
        {
            LoginResponsePacket response = new LoginResponsePacket();

            if (UserManager.Instance.GetByID(login.UserID) != null)
            {
                response.Success = false;
                response.Message = "로그인 실패 : 이미 로그인 되어있는 계정입니다!";

                await TcpPacketHelper.SendAsync(socket, response);

                Console.WriteLine($"[서버] 로그인 실패 : 이미 로그인 되어있는 계정입니다! - {login.UserID}");

                return;
            }

            string message = "";
            string? token = _userRepository.TryLogin(login.UserID, login.Password, ref message);

            if (token == null)
            {
                response.Success = false;
                response.Message = $"로그인 실패: {message}";

                await TcpPacketHelper.SendAsync(socket, response);

                Console.WriteLine($"[서버] 로그인 실패 : {message} - {login.UserID}");

                return;
            }

            string nickname = _userRepository.GetNicknameByID(login.UserID) ?? string.Empty;

            User user = new User(token, nickname, login.UserID, socket);
            UserManager.Instance.AddOrUpdate(user);

            response.Success = true;
            response.IssuedToken = token;

            await TcpPacketHelper.SendAsync(socket, response);

            Console.WriteLine($"[서버] 로그인 성공! : {user.Nickname} ({token})");
        }

        public async Task AutoLoginAsync(Socket socket, AutoLoginPacket login)
        {
            LoginResponsePacket response = new LoginResponsePacket();

            if (UserManager.Instance.GetByToken(login.Token) != null)
            {
                response.Success = false;
                response.Message = "자동 로그인 실패 : 이미 로그인 되어있는 계정입니다!";

                await TcpPacketHelper.SendAsync(socket, response);

                Console.WriteLine($"[서버] 자동 로그인 실패 : 이미 로그인 되어있는 계정입니다! - {login.Token}");

                return;
            }

            string? token = _userRepository.AutoLogin(login.Token);

            if (token == null)
            {
                response.Success = false;
                response.Message = $"로그인 실패: 이미 만료된 토큰이거나 없는 토큰입니다!";

                await TcpPacketHelper.SendAsync(socket, response);

                Console.WriteLine($"[서버] 로그인 실패 : 이미 만료된 토큰이거나 없는 토큰입니다! - {login.Token}");

                return;
            }

            string id = _userRepository.GetIDByToken(login.Token) ?? string.Empty;
            string nickname = _userRepository.GetNicknameByID(id) ?? string.Empty;

            User user = new User(token, nickname, id, socket);
            UserManager.Instance.AddOrUpdate(user);

            response.Success = true;
            response.IssuedToken = token;

            await TcpPacketHelper.SendAsync(socket, response);

            Console.WriteLine($"[서버] 로그인 성공! : {user.Nickname} ({token})");
        }
    }
}
