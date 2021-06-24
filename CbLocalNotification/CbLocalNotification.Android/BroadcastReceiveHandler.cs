using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CbLocalNotification.Droid
{
    [BroadcastReceiver(Enabled = true)]
    public class BroadcastReceiveHandler : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.GetStringExtra(NotificationConstants.Title);
                string message = intent.GetStringExtra(NotificationConstants.Message);

                AndroidNotificationManager manager = AndroidNotificationManager.Instance ?? new AndroidNotificationManager();
                manager.Show(title, message);
            }
        }
    }
}