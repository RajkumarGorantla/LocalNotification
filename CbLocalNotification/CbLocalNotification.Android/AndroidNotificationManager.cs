using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(CbLocalNotification.Droid.AndroidNotificationManager))]
namespace CbLocalNotification.Droid
{
    public class AndroidNotificationManager : INotificationManager
    {
        const string channelId = "default";
        const string channelName = "Default";
        const string channelDescription = "The default channel for notifications.";

        public static AndroidNotificationManager Instance { get; private set; }

        bool channelInitialized = false;
        int messageId = 0;
        int pendingIntentId = 0;

        NotificationManager manager;
        public event EventHandler NotificationReceived;

        public AndroidNotificationManager()
        {
            manager = (NotificationManager)Android.App.Application.Context.GetSystemService(Android.App.Application.NotificationService);
            CreateNotificationChannel();
            Instance = this;
        }

        public void SendNotification(string title, string message, DateTime? notifyTime = null)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            if (notifyTime != null)
            {
                Intent intent = new Intent(Android.App.Application.Context, typeof(BroadcastReceiveHandler));
                intent.PutExtra(NotificationConstants.Title, title);
                intent.PutExtra(NotificationConstants.Message, message);

                PendingIntent pendingIntent = PendingIntent.GetBroadcast(Android.App.Application.Context, pendingIntentId++, intent, PendingIntentFlags.CancelCurrent);
                long triggerTime = GetNotifyTime(notifyTime.Value); // milli seconds
                Android.App.Application.Context.StartService(intent);
                AlarmManager alarmManager = Android.App.Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
                alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
            }
            else
            {
                Show(title, message);
            }
        }

        public void ReceiveNotification(string title, string message)
        {
            NotificationReceived?.Invoke(null, new EventArgs());
        }

        public void Show(string title, string message)
        {
            Intent intent = new Intent(Android.App.Application.Context, typeof(MainActivity));
            intent.PutExtra(NotificationConstants.Title, title);
            intent.PutExtra(NotificationConstants.Message, message);

            PendingIntent pendingIntent = PendingIntent.GetActivity(Android.App.Application.Context, pendingIntentId++, intent, PendingIntentFlags.UpdateCurrent);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(Android.App.Application.Context, channelId)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetLargeIcon(BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, Resource.Drawable.ic_mtrl_chip_close_circle))
                .SetSmallIcon(Resource.Drawable.ic_mtrl_chip_close_circle)
                .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);

            Notification notification = builder.Build();
            manager.Notify(messageId++, notification);
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = channelDescription
                };
                manager.CreateNotificationChannel(channel);
            }

            channelInitialized = true;
        }

        long GetNotifyTime(DateTime notifyTime)
        {
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
            long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
            return utcAlarmTime; // milliseconds
        }
    }
}