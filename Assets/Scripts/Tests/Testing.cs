using Assets.Scripts.Data;
using Assets.Scripts.Managers;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class Testing:MonoBehaviour
    {

        public void Start()
        {
            
        }

        public async void OnaddUser1()
        {
            Firebase.Auth.FirebaseAuth auth = FirebaseInitializer.auth;

            if (auth.CurrentUser != null)
            {
                auth.SignOut();
            }

            
            AuthFirebaseManager authFirebaseManager = FindFirstObjectByType<AuthFirebaseManager>();
            UserFirebaseManager userFirebaseManager = FindFirstObjectByType<UserFirebaseManager>();

            if (auth.CurrentUser == null || auth.CurrentUser.Email != "oarjones@gmail.com")
            {
                if (auth.CurrentUser != null)
                {
                    auth.SignOut();
                }

                authFirebaseManager.OnSignInWithEmailAndPasswordAsync("oarjones@gmail.com", "Am1lcarbarca");
            }

            //var user = await userFirebaseManager.getUser(auth.CurrentUser.UserId);
            
            PvPGameMode pvpGameMode = new PvPGameMode();
            //pvpGameMode.addToGameWaitRoom(auth.CurrentUser, "oarjones");
        }


        public void OnaddUser2()    
        {

            Firebase.Auth.FirebaseAuth auth = FirebaseInitializer.auth;
            AuthFirebaseManager authFirebaseManager = FindFirstObjectByType<AuthFirebaseManager>();

            if (auth.CurrentUser == null || auth.CurrentUser.Email != "manuelp@gmail.com")
            {
                if (auth.CurrentUser != null)
                {
                    auth.SignOut();
                }

                authFirebaseManager.OnSignInWithEmailAndPasswordAsync("manuelp@gmail.com", "Am1lcarbarca");
            }

            PvPGameMode pvpGameMode = new PvPGameMode();
            //pvpGameMode.addToGameWaitRoom(auth.CurrentUser, "Manolo");
        }

    }
}
