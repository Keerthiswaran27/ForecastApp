namespace AuthService.wwwroot
{
    public class auth
    {
        window.googleSignIn = async () => {
            const { data, error } = await window.supabase.auth.signInWithOAuth({
                provider: 'google'
            });

            if (error) {
                console.error("OAuth error:", error.message);
            }
        };

window.getSupabaseUser = async () => {
            const { data, error } = await window.supabase.auth.getUser();
            if (error) return null;
            return data.user;
        };

    }
}
