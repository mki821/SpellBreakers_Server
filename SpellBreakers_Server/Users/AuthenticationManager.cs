using System.Net.Sockets;
using SpellBreakers_Server.DB;
using SpellBreakers_Server.Packet;
using SpellBreakers_Server.Tcp;

namespace SpellBreakers_Server.Users
{
    class AuthenticationManager
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

            if (!_userRepository.TryRegister(register.Nickname, register.Password))
            {
                response.Success = false;
                response.Message = "이미 존재하는 유저 닉네임입니다!";
            }
            else
            {
                response.Success = true;

                Console.WriteLine($"[서버] 새로운 유저가 회원가입했습니다! : {register.Nickname}");
            }

            await TcpPacketHelper.SendAsync(socket, response);
        }

        public async Task LoginAsync(Socket socket, LoginPacket login)
        {
            string? token = _userRepository.TryLogin(login.Nickname, login.Password);

            LoginResponsePacket response = new LoginResponsePacket();

            if (token == null)
            {
                response.Success = false;
                response.Message = "로그인 실패: 닉네임 혹은 비밀번호가 틀렸습니다!";

                await TcpPacketHelper.SendAsync(socket, response);

                Console.WriteLine($"[서버] 로그인 실패! : {login.Nickname}");

                return;
            }

            User user = new User(token, login.Nickname, socket);
            UserManager.Instance.AddOrUpdate(user);

            response.Success = true;
            response.IssuedToken = token;

            await TcpPacketHelper.SendAsync(socket, response);

            Console.WriteLine($"[서버] 로그인 성공! : {user.Nickname} ({token})");
        }

        public bool IsValidToken(string token) => _userRepository.IsValidToken(token);
    }
}
