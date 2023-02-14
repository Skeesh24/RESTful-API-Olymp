using RESTful_API_Olymp.Domain;
using RESTful_API_Olymp.Domain.Entities;
using System.Text;
using System.Text.Json;

namespace RESTful_API_Olymp.Static_Helper
{
    public class Helper
    {
        public static T DeserializeJson<T> (HttpRequest request)
        {
            return JsonSerializer.Deserialize<T>(GetJSONBody(request.Body));
        }

        public static Stream SerializeAnimal(AnimalEntity animal)
        {
            var stream = new MemoryStream();
            JsonSerializer.Serialize(stream, animal);
            return stream;  
        }
        
        public static string GetJSONBody(Stream stream)
        {
            var bodyStream = new StreamReader(stream);
            var bodyText = bodyStream.ReadToEndAsync();
            return bodyText.Result;
        }

        public static bool Authenticate(HttpRequest Request, DataContext Db, out int exitCode)
        {
            // exit codes:
            // 0 - success auth, yeah
            // 1 - error
            // 2 - incorrect pass
            // 3 - incorrect email

            string input = Request.Headers["Authorization"];

            if (input != null && input.StartsWith("Basic"))
            {
                var userAndPassword = GetStringFromBase64(input);

                var index = userAndPassword.IndexOf(":");
                var email = userAndPassword.Substring(0, index);
                var password = userAndPassword[(index + 1)..];

                if (!Db.Accounts.Any(x => x.Email == email))
                {
                    exitCode = 3;
                    return false;
                }

                var accoundPass = Db.Accounts.Where(x => x.Email == email).FirstOrDefault().Password;

                if (password.Equals(accoundPass))
                {
                    exitCode = 0;
                    return true;
                }
                else
                {
                    exitCode = 2;
                    return false;
                }
            }
            else
            {
                exitCode = 1;
                return false;
            }
        }


        public static string GetStringFromBase64(string base64encodingstring)
        {
            var encoder = Encoding.UTF8;

            var userAndPassword = encoder.GetString(Convert.FromBase64String(base64encodingstring.Substring("Basic ".Length).Trim()));

            return userAndPassword;
        }
    }
}
