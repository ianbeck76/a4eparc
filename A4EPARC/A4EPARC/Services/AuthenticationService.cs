﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using A4EPARC.Models;
using A4EPARC.Constants;
using A4EPARC.Models;
using A4EPARC.Repositories;

namespace A4EPARC.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISecurityService _securityService;

        public const string IsAdmin = "IsAdmin";
        public const string IsViewer = "IsViewer";
        public const string IsSuperAdmin = "IsSuperAdmin";

        public AuthenticationService(ISecurityService securityService, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _securityService = securityService;
        }

        public int Login(string email, string password)
        {
            User user = _userRepository.SingleOrDefault("email=@Email", new { email });
            if (((user != null)))
            {
                var saltedPassword = password + user.Salt;
                var hash = FormsAuthentication.HashPasswordForStoringInConfigFile(saltedPassword, "SHA1");

                if (user.Password == hash)
                {
                    Login(user);
                    return user.Id;      
                }
            }
            return 0;
        }

        public void Login(User user)
        {
            var authenticationTicket = new FormsAuthenticationTicket(1, user.Email + ";" + user.Id.ToString(CultureInfo.InvariantCulture), DateTime.Now,
                                                                     DateTime.Now.AddHours(1),
                                                                     true, GetDeliminatedRoles(user, ","), "/");
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                                        FormsAuthentication.Encrypt(authenticationTicket));

            if (authenticationTicket.IsPersistent)
            {
                cookie.Expires = authenticationTicket.Expiration;
            }
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private static string GetDeliminatedRoles(IUser user, string delimiter)
        {
            string roles = "";
            if (user.IsSuperAdmin)
                roles = roles + UserRoles.IsSuperAdmin + delimiter;
            if (user.IsAdmin)
                roles = roles + UserRoles.IsAdmin + delimiter;
            if (user.IsViewer)
                roles = roles + UserRoles.IsViewer + delimiter;

            return roles;
        }

        public string CreateSalt(int size)
        {
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rngCryptoServiceProvider.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        public static int GetLoggedInId()
        {
            int loginId = 0;

            if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                int.TryParse(HttpContext.Current.User.Identity.Name.Split(Convert.ToChar(";"))[1], out loginId);

            return loginId;
        }

        public static string GetLoggedInEmail()
        {
            return HttpContext.Current.User.Identity.Name.Split(Convert.ToChar(";"))[0];
        }

        public bool LoginExists(string email)
        {
            return
                _userRepository.Query("SELECT Id FROM [dbo].[User] WHERE email=@Email", new { email })
                    .Any();
        }

        public User ValidatePassword(string email, string password)
        {
            var user = _userRepository.Query("SELECT * FROM [dbo].[User] WHERE Email = @Email", new {@Email = email}).FirstOrDefault();
            if (user != null)
            {
                var saltedPassword = password + user.Salt;
                var hash = FormsAuthentication.HashPasswordForStoringInConfigFile(saltedPassword, "SHA1");

                if ((user.Password == hash))
                {
                     return user;
                }
            }
            return null;
        }

        public void ChangePassword(string email, string password)
        {
            User user = _userRepository.SingleOrDefault("email=@Email", new { email });
            if (user != null) // Stopping new passwords from being encrypted when a user is created
            {
                var salt = CreateSalt(16);
                var saltedPassword = password + salt;
                var hash = FormsAuthentication.HashPasswordForStoringInConfigFile(saltedPassword, "SHA1");
                user.Password = hash.ToUpper();
                user.Salt = salt;
                _userRepository.UpdatePassword(user);
            }
        }

        public IUser ResetPassword(int id, string password)
        {
            var user = _userRepository.All().FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                var salt = _securityService.RandomString(10);
                string saltedPassword = password + user;
                string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(saltedPassword, "SHA1");
                user.Password = hash;
                user.Salt = salt;
               // _userRepository.Save(user);
            }
            return user;
        }
    }

    public interface IAuthenticationService
    {
        int Login(string email, string password);
        void Login(User userProfile);
        string CreateSalt(int size);
        bool LoginExists(string email);
        User ValidatePassword(string email, string password);
        void ChangePassword(string email, string password);
    }
}