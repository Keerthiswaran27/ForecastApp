using Supabase;

namespace SCSService.Services
{
    public class SupabaseClientService
    {
        private readonly Client _client;

        public SupabaseClientService()
        {
            var supabaseUrl = "https://buvihfrcbabpgtxyepfy.supabase.co";
            var supabaseAnonKey = "<YOUR-ANON-KEY>"; // ⚠️ paste yours here

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = false
            };

            _client = new Client(supabaseUrl, supabaseAnonKey, options);
        }

        public Client Client => _client;
    }
}
