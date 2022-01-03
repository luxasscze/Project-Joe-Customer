﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomerMobile.Droid
{

    [Activity(Theme = "@style/SplashTheme", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        
        
            

            public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
            {
                base.OnCreate(savedInstanceState, persistentState);
                
            }

            // Launches the startup task
            protected override void OnResume()
            {
                base.OnResume();

                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }

            
            
        
    }
}