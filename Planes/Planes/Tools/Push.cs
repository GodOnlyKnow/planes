using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using cn.jpush.api;
using cn.jpush.api.push.mode;

namespace Planes.Tools
{
    public class Push
    {
        public static string AppKey = "5c8df7cbe4ea9764b9bc0898";
        public static string MasterSecret = "b1c29b7b5ed0f5935b968e67";

        public static void PushToAll(string title,string body)
        {
            JPushClient client = new JPushClient(AppKey, MasterSecret);
            PushPayload payload = new PushPayload();
            payload.platform = Platform.android_ios();
            payload.audience = Audience.all();
            var notification = new Notification().setAlert(body);
            notification.AndroidNotification = new cn.jpush.api.push.notification.AndroidNotification().setTitle(title);
            notification.IosNotification = new cn.jpush.api.push.notification.IosNotification();
            payload.notification = notification.Check();
            payload.options.apns_production = true;

            client.SendPush(payload);
        }

        public static void PushToAndroid(string title,string body)
        {
            JPushClient client = new JPushClient(AppKey, MasterSecret);
            PushPayload payload = new PushPayload();
            payload.platform = Platform.android();
            payload.audience = Audience.all();
            var notification = new Notification().setAlert(body);
            notification.AndroidNotification = new cn.jpush.api.push.notification.AndroidNotification().setTitle(title);
            payload.notification = notification.Check();

            client.SendPush(payload);
        }

        public static void PushToIOS(string body)
        {
            JPushClient client = new JPushClient(AppKey, MasterSecret);
            PushPayload payload = new PushPayload();
            payload.platform = Platform.ios();
            payload.audience = Audience.all();
            var notification = new Notification().setAlert(body);
            notification.IosNotification = new cn.jpush.api.push.notification.IosNotification();
            
            payload.notification = notification.Check();
            payload.options.apns_production = true;

            client.SendPush(payload);
        }

        public static void PushToId(string id,string body)
        {
            JPushClient client = new JPushClient(AppKey, MasterSecret);
            PushPayload payload = new PushPayload();
            payload.platform = Platform.all();
            payload.audience = Audience.s_registrationId(id);
            var notification = new Notification().setAlert(body);
            notification.AndroidNotification = new cn.jpush.api.push.notification.AndroidNotification().setTitle("有人回复了你");
            notification.IosNotification = new cn.jpush.api.push.notification.IosNotification();
            payload.options.apns_production = true;
            payload.notification = notification.Check();

            client.SendPush(payload);
        }
    }
}