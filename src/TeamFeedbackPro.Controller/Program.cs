namespace TeamFeedbackPro.Controller
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            string url = "http://0.0.0.0:5001";
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(
                    webBuilder => { 
                        webBuilder 
                        .UseStartup<Startup>()
                        .UseUrls(url);
                    }
                );
        }
    }
}