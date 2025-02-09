using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class AuthFirebaseManager : MonoBehaviour
    {
        public static FirebaseUser user { get; set; }

        // Create a user with the email and password.
        public void OnCreateUserWithEmailAsync(string email, string password, string username)
        {

            Debug.Log(String.Format("Attempting to create user {0}...", email));

            FirebaseInitializer.auth.CreateUserWithEmailAndPasswordAsync(email, password)
              .ContinueWithOnMainThread((task) => {

                  user = task.Result.User;
                  //GameEvents.OnCreateUserMethod(user.User.UserId, username, email);

                  return task;
              }).Unwrap();
        }


        // SignIn a user with the email and password.
        public void OnSignInWithEmailAndPasswordAsync(string email, string password)
        {
            Debug.Log(String.Format("Attempting to signIn user {0}...", email));


            FirebaseInitializer.auth.SignInWithEmailAndPasswordAsync(email, password)
              .ContinueWithOnMainThread((task) => {

                  var user = task.Result.User;
                  //GameEvents.OnSignInMethod(user.User.UserId, email);

                  return task;
              }).Unwrap();
        }



    }
}
