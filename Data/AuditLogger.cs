namespace InsureZenAPI.Data
{
    public static class AuditLogger
    {
        public static List<string> Logs = new List<string>();

        public static void Add(string message)
        {
            Logs.Add($"{DateTime.Now:dd-MM-yyyy HH:mm:ss}: {message}");
        }
    }
}