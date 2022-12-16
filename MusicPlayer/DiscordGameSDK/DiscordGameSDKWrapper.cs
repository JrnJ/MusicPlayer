using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicPlayer.DiscordGameSDK
{
    internal class DiscordGameSDKWrapper
    {
        // Maybe even private tbh
        public Discord.Discord DiscordInstance { get; private set; }

        private Thread _discordInstanceThread { get; set; }

        private bool _keepThreadRunning { get; set; } = true;

        public DiscordGameSDKWrapper(string clientId)
        {
            DiscordInstance = new(Int64.Parse(clientId), (UInt64)Discord.CreateFlags.Default);
            if (DiscordInstance == null)
                return;

            // Setup Discord
            DiscordInstance.SetLogHook(Discord.LogLevel.Debug, (level, message) =>
            {
                Console.WriteLine("Log[{0}] {1}", level, message);
            });

            var applicationManager = DiscordInstance.GetApplicationManager();

            // Get the current locale. This can be used to determine what text or audio the user wants.
            Console.WriteLine("Current Locale: {0}", applicationManager.GetCurrentLocale());

            // Get the current branch. For example alpha or beta.
            Console.WriteLine("Current Branch: {0}", applicationManager.GetCurrentBranch());


            // GetCurrentUser will error until this fires once.
            var userManager = DiscordInstance.GetUserManager();
            userManager.OnCurrentUserUpdate += () =>
            {
                var currentUser = userManager.GetCurrentUser();
                Console.WriteLine(currentUser.Username);
                Console.WriteLine(currentUser.Id);
            };

            StoreManager storeManager = DiscordInstance.GetStoreManager();

            // Get all SKUs.
            storeManager.FetchSkus(result =>
            {
                if (result == Discord.Result.Ok)
                {
                    foreach (var sku in storeManager.GetSkus())
                    {
                        Console.WriteLine("sku: {0} - {1} {2}", sku.Name, sku.Price.Amount, sku.Price.Currency);
                    }
                }
            });

            // Create a Thread
            _discordInstanceThread = new(new ThreadStart(DiscordInstanceThread));
            _discordInstanceThread.Start();
        }

        ~DiscordGameSDKWrapper()
        {
            DisposeInstance();
        }

        public void UpdateActivity(string songName, string artistName)
        {
            if (DiscordInstance == null)
                return;

            Discord.Activity activity = new()
            {
                State = songName,
                Details = artistName,
            };

            DiscordInstance.GetActivityManager().UpdateActivity(activity, result =>
            {
                //Console.WriteLine("Update Activity {0}", result);

                // Send an invite to another user for this activity.
                // Receiver should see an invite in their DM.
                // Use a relationship user's ID for this.
                // activityManager
                //   .SendInvite(
                //       364843917537050624,
                //       Discord.ActivityActionType.Join,
                //       "",
                //       inviteResult =>
                //       {
                //           Console.WriteLine("Invite {0}", inviteResult);
                //       }
                //   );
            });
        }

        public void DisposeInstance()
        {
            if (DiscordInstance == null || _discordInstanceThread == null)
                return;

            // Close Thread
            _keepThreadRunning = false;

            // Dispose op the Discord instance
            DiscordInstance.Dispose();
        }

        private void DiscordInstanceThread()
        {
            try
            {
                while (_keepThreadRunning && DiscordInstance != null)
                {
                    DiscordInstance.RunCallbacks();
                    Thread.Sleep(1000 / 60);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex);
                this.DisposeInstance();
            }
        }
    }
}
